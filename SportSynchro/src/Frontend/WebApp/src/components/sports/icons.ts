import type { ElementType } from "react";
import {
  FaFutbol,
  FaBasketballBall,
  FaFootballBall,
  FaHockeyPuck,
  FaGolfBall,
  FaRunning,
  FaTableTennis,
} from "react-icons/fa";

import {
  FaBaseballBatBall
} from "react-icons/fa6";

import {
  GiBoxingGlove,
  GiGamepad,
  GiCycling,
  GiDart,
  GiShuttlecock,
  GiMountainClimbing,
  GiHorseHead,
  GiPistolGun,
  GiSkateboard,
  GiWaterSplash,
  GiWeightLiftingUp,
  GiSkis,
  GiIceSkate,
  GiWinterHat,
  GiCardJoker,
  GiAmericanFootballHelmet,
} from "react-icons/gi";

import {
  MdSportsMotorsports,
  MdSportsTennis,
  MdSports,
} from "react-icons/md";

// Coppling: sport name from backend -> icon component
export const sportIcons: Record<string, ElementType> = {
  Soccer: FaFutbol,
  Motorsport: MdSportsMotorsports,
  Fighting: GiBoxingGlove,
  Baseball: FaBaseballBatBall,
  Basketball: FaBasketballBall,
  "American Football": FaFootballBall,
  "Ice Hockey": FaHockeyPuck,
  Golf: FaGolfBall,
  Rugby: FaFootballBall,
  Tennis: MdSportsTennis,
  Cricket: FaBaseballBatBall,
  Cycling: GiCycling,
  "Australian Football": FaFootballBall,
  ESports: GiGamepad,
  Volleyball: FaBasketballBall,
  Netball: FaBasketballBall,
  Handball: FaBaseballBatBall,
  Snooker: MdSportsTennis,
  "Field Hockey": FaHockeyPuck,
  Darts: GiDart,
  Athletics: FaRunning,
  Badminton: GiShuttlecock,
  Climbing: GiMountainClimbing,
  Equestrian: GiHorseHead,
  Gymnastics: MdSports,
  Shooting: GiPistolGun,
  "Extreme Sports": GiSkateboard,
  "Table Tennis": FaTableTennis,
  "Multi Sports": MdSports,
  Watersports: GiWaterSplash,
  Weightlifting: GiWeightLiftingUp,
  Skiing: GiSkis,
  Skating: GiIceSkate,
  Wintersports: GiWinterHat,
  Lacrosse: GiAmericanFootballHelmet,
  Gambling: GiCardJoker,
  Gaelic: FaFootballBall,
};
