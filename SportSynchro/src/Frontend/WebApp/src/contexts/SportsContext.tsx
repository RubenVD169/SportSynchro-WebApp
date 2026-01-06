import {
  createContext,
  useCallback,
  useState,
  type ReactNode,
} from "react";

import {
  fetchAdminSports,
  updateSportVisibility,
  fetchUserSports
} from "../services/sportService";
import { hasRole } from "../auth/hasRole";
import { useAuth } from "react-oidc-context";

interface SportsContextType {
  sports: Sport[];
  selectedSportId: number | null;

  loadingSports: boolean;

  selectSport: (id: number) => void;
  toggleSportVisibility: (id: number, current: boolean) => Promise<void>;

  refreshSports: () => Promise<void>;
}

const SportsContext = createContext<SportsContextType | undefined>(undefined);

export function SportsProvider({ children }: { children: ReactNode }) {
  const [sports, setSports] = useState<Sport[]>([]);
  const [selectedSportId, setSelectedSportId] = useState<number | null>(null);
  const { user } = useAuth();
  const isAdmin = hasRole(user, "Admin");
  const [loadingSports, setLoadingSports] = useState(false);

  const refreshSports = useCallback(async () => {
    if (!user) return;

    setLoadingSports(true);
    try {
      const data = isAdmin
        ? await fetchAdminSports()
        : await fetchUserSports();

      setSports(data);
    } finally {
      setLoadingSports(false);
    }
  }, [isAdmin, user]);

  function selectSport(id: number) {
    setSelectedSportId(id);
  }

  async function toggleSportVisibility(id: number, current: boolean) {
    if (!hasRole(user, "Admin")) return;

    const newValue = !current;

    setSports((prev) =>
      prev.map((sport) =>
        sport.id === id ? { ...sport, visible: newValue } : sport
      )
    );

    try {
      await updateSportVisibility(id, newValue);
    } catch {
      setSports((prev) =>
        prev.map((sport) =>
          sport.id === id ? { ...sport, visible: current } : sport
        )
      );
    }
  }

  return (
    <SportsContext.Provider
      value={{
        sports,
        selectedSportId,
        loadingSports,
        selectSport,
        toggleSportVisibility,
        refreshSports,
      }}
    >
      {children}
    </SportsContext.Provider>
  );
}

export default SportsContext;
