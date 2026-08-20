import React from "react";
import type { AuthUser, DatasetListItem } from "../../types";
import { UserProfileDropdown } from "./UserProfileDropdown";

interface NavbarProps {
    theme: "dark" | "pink";
    setTheme: (theme: "dark" | "pink") => void;
    user: AuthUser | null;
    setShowAuthModal: (show: boolean) => void;
    handleLogout: () => void;
    isSignalRConnected: boolean;
    datasetsList: DatasetListItem[];
    activeDatasetId: string;
    setActiveDatasetId: (id: string) => void;
    setShowUploadModal: (show: boolean) => void;
}

export const Navbar: React.FC<NavbarProps> = ({
    theme,
    setTheme,
    user,
    setShowAuthModal,
    handleLogout,
    isSignalRConnected,
    datasetsList,
    activeDatasetId,
    setActiveDatasetId,
    setShowUploadModal
}) => {
    return (
        <header className="navbar">
            <div className="navbar-brand">
                <span className="brand-logo">📊</span>
                <div className="brand-text">
                    <h1>InsightHub</h1>
                    <span className="brand-badge">Multi-Tenant Analytics v2.0</span>
                </div>
            </div>

            <div className="dataset-selector-wrapper">
                <label>Aktif Veri Seti:</label>
                <select
                    className="dataset-select"
                    value={activeDatasetId}
                    onChange={(e) => setActiveDatasetId(e.target.value)}
                >
                    {datasetsList.map((d) => (
                        <option key={d.id} value={d.id}>
                            {d.name} ({d.totalRows} satır)
                        </option>
                    ))}
                </select>
                <button
                    className="btn btn-sm btn-primary upload-btn"
                    onClick={() => setShowUploadModal(true)}
                >
                    📁 + Yeni Yükle
                </button>
            </div>

            <div className="navbar-controls">
                <div className="signalr-indicator" title={isSignalRConnected ? "SignalR Bağlı" : "SignalR Bağlantısı Kesildi"}>
                    <span className={`status-dot ${isSignalRConnected ? "connected" : "disconnected"}`} />
                    <span className="status-label">{isSignalRConnected ? "Canlı Soket" : "Çevrimdışı"}</span>
                </div>

                <button
                    className="theme-toggle-btn"
                    onClick={() => setTheme(theme === "pink" ? "dark" : "pink")}
                    title="Tema Değiştir"
                >
                    {theme === "pink" ? "🌙 Koyu Tema" : "🌸 Pembe Tema"}
                </button>

                {user ? (
                    <UserProfileDropdown
                        user={user}
                        onLogout={handleLogout}
                        onNavigate={(tab) => {
                            if (typeof (window as any).onSidebarTabChange === "function") {
                                (window as any).onSidebarTabChange(tab);
                            }
                        }}
                    />
                ) : (
                    <button className="btn btn-primary login-nav-btn" onClick={() => setShowAuthModal(true)}>
                        🔐 Giriş Yap / Kaydol
                    </button>
                )}
            </div>
        </header>
    );
};
