import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import LeagueCard from "../../components/sports/LeagueCard";
import { FullscreenLoader } from "../../components/ui/FullscreenLoader";
import { getHasLiveAccess } from "../../services/subscriptionService";
import { IoArrowBack } from "react-icons/io5";
import useRecentMatchesBySport from "../../hooks/useRecentMatchesBySport";
import { useUserLeagues } from "../../hooks/useUserLeagues";


export default function SportDetailsPage() {
    const { id } = useParams();
    const navigate = useNavigate();
    const sportId = Number(id);

    const { leagues:userLeagues, loading: loadingLeagues } = useUserLeagues(sportId);
    const { matches: recentMatches, loading: loadingMatches } =
        useRecentMatchesBySport(sportId);

    const [hasLiveAccess, setHasLiveAccess] = useState<boolean>(false);
    const [loadingSubscription, setLoadingSubscription] = useState(true);

    useEffect(() => {
        getHasLiveAccess()
            .then(setHasLiveAccess)
            .finally(() => setLoadingSubscription(false));
    }, []);

    if (loadingLeagues || loadingSubscription || loadingMatches) {
        return <FullscreenLoader text="Loading Leagues ..." />;
    }

    const matchesByLeagueName = recentMatches.reduce<Record<string, Match[]>>(
        (acc, match) => {
            acc[match.leagueName] ??= [];
            acc[match.leagueName].push(match);
            return acc;
        },
        {}
    );

    return (
        <div>
            <button
                onClick={() => navigate("/")}
                className="mb-4 flex items-center gap-2 text-gray-400 hover:text-white transition hover:cursor-pointer"
            >
                <IoArrowBack className="text-lg" />
                <span className="text-sm">Back to sports</span>
            </button>
            <h1 className="text-3xl font-bold mb-6 text-gray-100">
                Leagues
            </h1>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {userLeagues.map((league: League) => (
                    <LeagueCard
                        key={league.id}
                        league={league}
                        hasLiveAccess={hasLiveAccess}
                        recentMatches={matchesByLeagueName[league.name] ?? []}
                    />
                ))}
            </div>
        </div>
    );
}
