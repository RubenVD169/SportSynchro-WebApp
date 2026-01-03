import useSports from "../../hooks/useSports";
import SportCard from "../../components/sports/SportCard";
import { FullscreenLoader } from "../../components/ui/FullscreenLoader";
import { useEffect } from "react";

export default function HomePage() {
  const { sports, loadingSports, refreshUserSports } = useSports();

  useEffect(() => {
    refreshUserSports();
  }, [refreshUserSports]);
  
  
  if (loadingSports) {
    return <FullscreenLoader text="Loading Sports ..." />;
  }

  return (
    <div>
      <h1 className="text-3xl font-bold mb-6 text-gray-100">
        Available Sports
      </h1>

      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
        {Array.isArray(sports) &&
          sports.map((sport) => (
            <SportCard key={sport.id} sport={sport} />
          ))}
      </div>
    </div>
  );
}
