import useSports from "../../hooks/useSports";
import useLeagues from "../../hooks/useLeagues";
import { updateLeagueVisibility } from "../../services/leagueService";
import LeagueCardAdmin from "../../components/sports/LeagueCardAdmin";

export default function LeagueVisibilityPage() {
    const { selectedSportId } = useSports();
    const { leagues, loadingLeagues } = useLeagues(selectedSportId);

    async function handleToggle(leagueId: number, currentVisible: boolean) {
        await updateLeagueVisibility(leagueId, !currentVisible);
    }

    if (!selectedSportId) return <p>Select a sport first.</p>;
    if (loadingLeagues) return <p>Loading leagues...</p>;

    return (
        <div>
            <h1 className="text-2xl text-white mb-4">Manage Leagues</h1>

            {leagues.map((league) => (
                <LeagueCardAdmin
                    key={league.id}
                    league={league}
                    onToggleVisibility={handleToggle}
                />
            ))}
        </div>
    );
}
