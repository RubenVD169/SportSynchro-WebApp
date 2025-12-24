import useSports from "../../hooks/useSports";
import SportCardAdmin from "../../components/sports/SportCardAdmin";
import { FullscreenLoader } from "../../components/ui/FullscreenLoader";
import { useEffect } from "react";
import { useAuth } from "react-oidc-context";

export default function SportsVisibilityPage() {
   const auth = useAuth();
  const { sports, loadingSports, refreshSports } = useSports();

  useEffect(() => {
    if (auth.isLoading) return;
    if (!auth.isAuthenticated) return;

    refreshSports();
  }, [auth.isLoading, auth.isAuthenticated, refreshSports]);

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
