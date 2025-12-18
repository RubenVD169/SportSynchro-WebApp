import { useEffect } from "react";
import { useParams } from "react-router-dom";
import useSports from "../../hooks/useSports";
import useLeagues from "../../hooks/useLeagues";
import LeagueCard from "../../components/sports/LeagueCard";

export default function SportDetailsPage() {
    const { id } = useParams();
    const sportId = Number(id);

    const { selectedSportId, selectSport } = useSports();
    const { leagues, loadingLeagues } = useLeagues(selectedSportId);


    useEffect(() => {
        if (sportId) selectSport(sportId);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [sportId]);

    if (loadingLeagues) return <p className="text-white">Loading leagues…</p>;

    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-gray-100">
                Leagues for this sport
            </h1>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {leagues.map((league: League) => (
                    <LeagueCard key={league.id} league={league} />
                ))}
            </div>
        </div>
    );
}
