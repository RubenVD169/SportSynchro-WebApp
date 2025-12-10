import { FaBaseballBatBall } from "react-icons/fa6";
import SportCard from "../../components/sports/SportCard";
import {
  FaFutbol, FaBasketballBall, FaFootballBall,
  FaHockeyPuck, FaGolfBall, FaTableTennis, FaRunning
} from "react-icons/fa";

import { GiBoxingGlove, GiGamepad } from "react-icons/gi";
import { MdSportsMotorsports, MdSportsTennis } from "react-icons/md";

const sports = [
  { name: "Soccer", Icon: FaFutbol },
  { name: "Motorsport", Icon: MdSportsMotorsports },
  { name: "Fighting", Icon: GiBoxingGlove },
  { name: "Baseball", Icon: FaBaseballBatBall },
  { name: "Basketball", Icon: FaBasketballBall },
  { name: "American Football", Icon: FaFootballBall },
  { name: "Ice Hockey", Icon: FaHockeyPuck },
  { name: "Golf", Icon: FaGolfBall },
  { name: "Tennis", Icon: MdSportsTennis  },
  { name: "Cricket", Icon: FaTableTennis },
  { name: "Athletics", Icon: FaRunning },
  { name: "ESports", Icon: GiGamepad },
];

export default function HomePage() {
  return (
    <div>
      <h1 className="text-3xl font-bold mb-6 text-gray-100">
        Available Sports
      </h1>

      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
        {sports.map((sport) => (
          <SportCard key={sport.name} name={sport.name} Icon={sport.Icon} />
        ))}
      </div>
    </div>
  );
}
