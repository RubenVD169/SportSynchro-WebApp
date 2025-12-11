import { useNavigate } from "react-router-dom";
import { sportIcons } from "./icons";

interface SportCardProps {
  sport: {
    id: number;
    name: string;
  };
}

export default function SportCard({ sport }: SportCardProps) {
  const navigate = useNavigate();

  // search icon for sport name
  const Icon = sportIcons[sport.name] || null;

  return (
    <div
      onClick={() => navigate(`/sports/${sport.id}`)}
      className="p-4 bg-gray-800 hover:bg-gray-700 cursor-pointer 
                 rounded-lg shadow text-center flex flex-col items-center 
                 transition"
    >
      {Icon && <Icon className="text-4xl text-blue-500 mb-3" />}
      <h2 className="text-lg font-semibold text-white">{sport.name}</h2>
    </div>
  );
}
