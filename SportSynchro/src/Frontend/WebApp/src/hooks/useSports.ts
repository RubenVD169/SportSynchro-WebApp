import { useContext } from "react";
import SportsContext from "../contexts/SportsContext";

export default function useSports() {
  const ctx = useContext(SportsContext);
  if (!ctx) {
    throw new Error("useSports must be used inside <SportsProvider>");
  }
  return ctx;
}
