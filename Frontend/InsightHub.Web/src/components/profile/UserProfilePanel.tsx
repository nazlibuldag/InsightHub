import React from "react";
import type { AuthUser } from "../../types";

interface UserProfilePanelProps {
    user: AuthUser;
    totalDatasets: number;
    totalSavedAnalyses: number;
    theme: "dark" | "pink";
    setTheme: (theme: "dark" | "pink") => void;
    onLogout: () => void;
}

export const UserProfilePanel: React.FC<UserProfilePanelProps> = ({
    user,
    totalDatasets,
    totalSavedAnalyses,
    theme,
    setTheme,
    onLogout
}) => {
    const isAdmin = user.role === 1 || user.role === 0 || (user as any).roleName === "Admin";

    return (
        <div style={{ display: "flex", flexDirection: "column", gap: "24px", maxWidth: "800px" }}>
            <div className="panel">
                <div className="panel-header">
                    <div>
                        <h3 className="desc-stat-column-title">👤 Profilim & Hesap Bilgileri</h3>
                        <p className="desc-stat-desc">Kullanıcı kimliği, rolü ve sistem tercihleri</p>
                    </div>
                </div>

                <div style={{ display: "flex", alignItems: "center", gap: "20px", marginTop: "20px", padding: "20px", borderRadius: "14px", background: "rgba(236, 72, 153, 0.08)", border: "1px solid rgba(244, 114, 182, 0.2)" }}>
                    <div
                        style={{
                            width: "72px",
                            height: "72px",
                            borderRadius: "50%",
                            background: "linear-gradient(135deg, #ec4899, #8b5cf6)",
                            color: "#fff",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            fontSize: "28px",
                            fontWeight: 800,
                            boxShadow: "0 6px 16px rgba(236, 72, 153, 0.4)"
                        }}
                    >
                        {user.firstName ? user.firstName[0].toUpperCase() : "U"}
                    </div>

                    <div>
                        <h2 className="desc-stat-column-title" style={{ fontSize: "20px", margin: 0 }}>
                            {user.firstName} {user.lastName}
                        </h2>
                        <div style={{ color: "var(--text-muted)", fontSize: "13px", marginTop: "2px" }}>
                            {user.email}
                        </div>
                        <div style={{ marginTop: "8px" }}>
                            <span
                                style={{
                                    padding: "4px 12px",
                                    borderRadius: "14px",
                                    fontSize: "12px",
                                    fontWeight: 700,
                                    background: isAdmin ? "rgba(236, 72, 153, 0.2)" : "rgba(99, 102, 241, 0.2)",
                                    color: isAdmin ? "#ec4899" : "#818cf8"
                                }}
                            >
                                {isAdmin ? "🛡️ Sistem Yöneticisi (Admin)" : "📊 Analist Kullanıcı"}
                            </span>
                        </div>
                    </div>
                </div>

                {/* User stats */}
                <div className="stats-grid" style={{ marginTop: "20px" }}>
                    <div className="stat-card">
                        <span className="stat-title">📁 Yüklenen Veri Setleri</span>
                        <div className="stat-value">{totalDatasets}</div>
                        <div className="stat-subtitle">Hesabınıza ait datasetler</div>
                    </div>

                    <div className="stat-card">
                        <span className="stat-title">💾 Kaydedilmiş Analizler</span>
                        <div className="stat-value">{totalSavedAnalyses}</div>
                        <div className="stat-subtitle">Kayıtlı analiz raporları</div>
                    </div>
                </div>

                {/* Theme & Actions */}
                <div style={{ marginTop: "24px", display: "flex", justifyContent: "space-between", alignItems: "center", borderTop: "1px solid var(--border-card)", paddingTop: "20px" }}>
                    <button
                        className="theme-toggle-btn"
                        onClick={() => setTheme(theme === "dark" ? "pink" : "dark")}
                        style={{ padding: "8px 16px", borderRadius: "10px", fontSize: "13px" }}
                    >
                        {theme === "pink" ? "🌙 Koyu Gece Teması" : "🌸 Pembeli Açık Tema"}
                    </button>

                    <button
                        className="upload-button"
                        onClick={onLogout}
                        style={{ padding: "8px 18px", fontSize: "13px", background: "linear-gradient(135deg, #ef4444, #dc2626)" }}
                    >
                        🚪 Oturumu Kapat (Çıkış Yap)
                    </button>
                </div>
            </div>
        </div>
    );
};
