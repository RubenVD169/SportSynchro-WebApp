interface ToggleSwitchProps {
    enabled: boolean;
    onToggle?: (value: boolean) => void;
    disabled?: boolean;
}

export default function ToggleSwitch({ enabled, onToggle, disabled = false }: ToggleSwitchProps) {
    function handleToggle() {
        if (!disabled && onToggle) {
            onToggle(!enabled);
        }
    }

    return (
        <button
            type="button"
            onClick={handleToggle}
            onKeyDown={(e) => {
                if (e.key === "Enter" || e.key === " ") {
                    e.preventDefault();
                    handleToggle();
                }
            }}
            className={`
                relative w-12 h-6 rounded-full px-1 flex items-center 
                transition-colors duration-300 
                ${enabled ? "bg-blue-500" : "bg-gray-700"}
                ${disabled ? "opacity-50 cursor-not-allowed" : "cursor-pointer"}
            `}
        >
            <span
                className={`
                    w-5 h-5 rounded-full bg-white shadow transform
                    transition-transform duration-300
                    ${enabled ? "translate-x-6" : "translate-x-0"}
                `}
            />
        </button>
    );
}
