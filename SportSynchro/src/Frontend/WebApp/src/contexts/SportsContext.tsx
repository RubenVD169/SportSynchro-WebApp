import {
  createContext,
  useCallback,
  useState,
  type ReactNode,
} from "react";

import {
  fetchSports,
  updateSportVisibility,
} from "../services/sportService";

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

  const [loadingSports, setLoadingSports] = useState(false);

  const refreshSports = useCallback(async () => {
    setLoadingSports(true);
    try {
      const data = await fetchSports();
      setSports(data);
    } finally {
      setLoadingSports(false);
    }
  }, []);

  function selectSport(id: number) {
    setSelectedSportId(id);
  }

  async function toggleSportVisibility(id: number, current: boolean) {
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
