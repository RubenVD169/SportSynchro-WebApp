import useSports from "../../hooks/useSports";
import SportCardAdmin from "../../components/sports/SportCardAdmin";
import { FullscreenLoader } from "../../components/ui/FullscreenLoader";

export default function SportsVisibilityPage() {
  const { sports, loadingSports } = useSports();

  if (loadingSports) {
    return <FullscreenLoader text="Loading Sports ..." />;
  }

  return (
    <div className="grid gap-4">
      {sports.map((sport) => (
        <SportCardAdmin key={sport.id} sport={sport} />
      ))}
    </div>
  );
}
