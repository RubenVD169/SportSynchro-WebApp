import type { IconType } from "react-icons";

interface SportCardProps {
  name: string;
  Icon: IconType;
}

export default function SportCard({ name, Icon }: SportCardProps) {
  return (
    <div className="flex flex-col items-center justify-center p-6 
                    bg-gray-800 hover:bg-gray-750 rounded-lg 
                    shadow-md hover:shadow-lg 
                    transition-all cursor-pointer">

      <Icon className="text-4xl mb-3 text-blue-400" />

      <div className="text-lg font-medium text-gray-200 text-center">
        {name}
      </div>
    </div>
  );
}
