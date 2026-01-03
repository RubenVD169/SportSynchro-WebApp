import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import useLeagues from "../../hooks/useLeagues";
import LeagueCard from "../../components/sports/LeagueCard";
import { FullscreenLoader } from "../../components/ui/FullscreenLoader";
import { getHasLiveAccess } from "../../services/subscriptionService";
import { IoArrowBack } from "react-icons/io5";

export default function SportDetailsPage() {
    const { id } = useParams();
    const navigate = useNavigate();
    const sportId = Number(id);

    const { userLeagues, loadingLeagues } = useLeagues(sportId);

    const [hasLiveAccess, setHasLiveAccess] = useState<boolean>(false);
    const [loadingSubscription, setLoadingSubscription] = useState(true);

    useEffect(() => {
        getHasLiveAccess()
            .then(setHasLiveAccess)
            .finally(() => setLoadingSubscription(false));
    }, []);

    if (loadingLeagues || loadingSubscription) {
        return <FullscreenLoader text="Loading Leagues ..." />;
    }

    return (
        <div>
            <button
                onClick={() => navigate("/")}
                className="mb-4 flex items-center gap-2 text-gray-400 hover:text-white transition"
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
                    />
                ))}
            </div>
        </div>
    );
}
