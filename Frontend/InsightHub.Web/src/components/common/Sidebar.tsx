import React from "react";

export type ActiveTabType =
    | "dashboard"
    | "datasets"
    | "general-analysis"
    | "analysis"
    | "ml-forecast"
    | "ai-prediction"
    | "saved-analysis"
    | "saved-analysis-detail"
    | "profile"
    | "admin";

interface SidebarProps {
    activeTab: ActiveTabType;
    setActiveTab: (tab: ActiveTabType) => void;
    user: any;
    handleLogout: () => void;
}

export const Sidebar: React.FC<SidebarProps> = ({
    activeTab,
    setActiveTab,
    user,
    handleLogout
}) => {
    const isAdmin = Boolean(
        user && (user.role === 1 || user.role === 0 || user.roleName === "Admin" || user.role === "Admin")
    );

    return (
        <aside className="sidebar">
            <div className="logo">
                <div className="logo-icon">📊</div>
                <span>InsightHub</span>
            </div>

            <nav className="nav">
                <a
                    className={`nav-item ${activeTab === "dashboard" ? "active" : ""}`}
                    onClick={() => setActiveTab("dashboard")}
                    style={{ cursor: "pointer" }}
                >
                    <span>📈</span> Dashboard
                </a>
                <a
                    className={`nav-item ${activeTab === "datasets" ? "active" : ""}`}
                    onClick={() => setActiveTab("datasets")}
                    style={{ cursor: "pointer" }}
                >
                    <span>📁</span> Veri Seti Yönetimi
                </a>
                <a
                    className={`nav-item ${activeTab === "general-analysis" ? "active" : ""}`}
                    onClick={() => setActiveTab("general-analysis")}
                    style={{ cursor: "pointer" }}
                >
                    <span>📊</span> Genel Analiz
                </a>
                <a
                    className={`nav-item ${activeTab === "analysis" ? "active" : ""}`}
                    onClick={() => setActiveTab("analysis")}
                    style={{ cursor: "pointer" }}
                >
                    <span>🔬</span> Detaylı Analiz
                </a>
                <a
                    className={`nav-item ${activeTab === "ml-forecast" ? "active" : ""}`}
                    onClick={() => setActiveTab("ml-forecast")}
                    style={{ cursor: "pointer" }}
                >
                    <span>🤖</span> ML Tahminleme & Trendler
                </a>
                <a
                    className={`nav-item ${activeTab === "ai-prediction" ? "active" : ""}`}
                    onClick={() => setActiveTab("ai-prediction")}
                    style={{ cursor: "pointer" }}
                >
                    <span>🧠</span> AI ile Tahmin Et
                </a>
                <a
                    className={`nav-item ${activeTab === "saved-analysis" || activeTab === "saved-analysis-detail" ? "active" : ""}`}
                    onClick={() => setActiveTab("saved-analysis")}
                    style={{ cursor: "pointer" }}
                >
                    <span>💾</span> Kaydedilmiş Analizler
                </a>
                <a
                    className={`nav-item ${activeTab === "profile" ? "active" : ""}`}
                    onClick={() => setActiveTab("profile")}
                    style={{ cursor: "pointer" }}
                >
                    <span>👤</span> Profilim
                </a>
                {isAdmin && (
                    <a
                        className={`nav-item ${activeTab === "admin" ? "active" : ""}`}
                        onClick={() => setActiveTab("admin")}
                        style={{ cursor: "pointer", color: "#ec4899", fontWeight: 700 }}
                    >
                        <span>🛡️</span> Admin Paneli
                    </a>
                )}
            </nav>

            <div className="sidebar-bottom">
                <div className="user">
                    <div className="user-avatar">
                        {user && user.firstName ? user.firstName[0].toUpperCase() : "U"}
                    </div>
                    <div style={{ flex: 1, minWidth: 0, overflow: "hidden" }}>
                        <strong style={{ whiteSpace: "nowrap", overflow: "hidden", textOverflow: "ellipsis" }}>
                            {user ? `${user.firstName} ${user.lastName}` : "Kullanıcı"}
                        </strong>
                        <small style={{ color: isAdmin ? "#ec4899" : "var(--text-muted)", fontWeight: 600 }}>
                            {isAdmin ? "🛡️ Yönetici" : "Analist"}
                        </small>
                    </div>
                    <button
                        type="button"
                        onClick={handleLogout}
                        className="btn-secondary"
                        style={{ padding: "6px 10px", fontSize: "11px", color: "#ef4444", borderColor: "#fecaca" }}
                        title="Oturumu Kapat"
                    >
                        Çıkış
                    </button>
                </div>
            </div>
        </aside>
    );
};
