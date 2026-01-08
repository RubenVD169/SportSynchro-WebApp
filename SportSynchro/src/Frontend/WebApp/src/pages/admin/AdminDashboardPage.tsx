import DashboardCard from "../../components/admin/DashboardCard";

export default function AdminDashboardPage() {
    //Dashboard cards so that it's easy to add more in the future
    const cards = [
        {
            to: "/admin/sports",
            title: "Manage Sports",
            description: "Enable or disable sports and configure visibility.",
        },
    ];

    return (
        <main>
            <h1 className="text-3xl font-bold mb-8 text-white">Admin Dashboard</h1>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                {cards.map((card) => (
                    <DashboardCard
                        key={card.to}
                        to={card.to}
                        title={card.title}
                        description={card.description}
                    />
                ))}
            </div>
        </main>
    );
}
