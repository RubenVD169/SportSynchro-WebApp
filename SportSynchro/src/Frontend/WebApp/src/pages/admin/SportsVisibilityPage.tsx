import useSports from "../../hooks/useSports";
import SportCardAdmin from "../../components/sports/SportCardAdmin";

export default function SportsVisibilityPage() {
  const { sports, loadingSports } = useSports();

  if (loadingSports) return <p>Loading…</p>;

  return (
    <div className="grid gap-4">
      {sports.map((sport) => (
        <SportCardAdmin key={sport.id} sport={sport} />
      ))}
    </div>
  );
}
