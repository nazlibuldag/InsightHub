import React, { useState, useEffect } from "react";
import type { AdminUserDto, AdminStatsDto } from "../../types";
import * as api from "../../services/api";

interface AdminPanelProps {
    token: string | null;
}

export const AdminPanel: React.FC<AdminPanelProps> = ({ token }) => {
    const [stats, setStats] = useState<AdminStatsDto | null>(null);
    const [users, setUsers] = useState<AdminUserDto[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [isUpdating, setIsUpdating] = useState<boolean>(false);
    const [searchQuery, setSearchQuery] = useState<string>("");
    const [feedbackMsg, setFeedbackMsg] = useState<{ type: "success" | "error"; text: string } | null>(null);

    const loadAdminData = async () => {
        if (!token) return;
        setIsLoading(true);
        try {
            const [statsData, usersData] = await Promise.all([
                api.fetchAdminStats(token),
                api.fetchAdminUsers(token)
            ]);
            setStats(statsData);
            setUsers(usersData);
            setFeedbackMsg(null);
        } catch (err: any) {
            console.error("Admin data error", err);
            // Only set error if we don't have stats yet
            setFeedbackMsg({ type: "error", text: "Yönetici verileri yüklenirken hata: " + err.message });
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        if (token) {
            loadAdminData();
        }
    }, [token]);

    const handleRoleChange = async (userId: string, newRole: number) => {
        setIsUpdating(true);
        setFeedbackMsg(null);
        try {
            await api.updateUserRole(userId, newRole, token);
            setFeedbackMsg({ type: "success", text: "Kullanıcı rolü başarıyla güncellendi." });
            await loadAdminData();
        } catch (err: any) {
            setFeedbackMsg({ type: "error", text: "Rol güncellenirken hata: " + err.message });
        } finally {
            setIsUpdating(false);
        }
    };

    const handleToggleStatus = async (userId: string) => {
        setIsUpdating(true);
        setFeedbackMsg(null);
        try {
            await api.toggleUserStatus(userId, token);
            setFeedbackMsg({ type: "success", text: "Kullanıcı durumu başarıyla değiştirildi." });
            await loadAdminData();
        } catch (err: any) {
            setFeedbackMsg({ type: "error", text: "Durum değiştirilirken hata: " + err.message });
        } finally {
            setIsUpdating(false);
        }
    };

    const filteredUsers = users.filter(u =>
        `${u.firstName} ${u.lastName} ${u.email}`.toLowerCase().includes(searchQuery.toLowerCase())
    );

    return (
        <div style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
            {/* Header */}
            <div className="panel">
                <div className="panel-header">
                    <div>
                        <h3 className="desc-stat-column-title">🛡️ Sistem Yönetim & Admin Paneli</h3>
                        <p className="desc-stat-desc">Kullanıcı yönetimi, rol atama, hesap aktiflik kontrolü ve sistem istatistikleri</p>
                    </div>

                    <button
                        className="upload-button"
                        onClick={loadAdminData}
                        disabled={isLoading || isUpdating}
                        style={{ padding: "8px 16px", fontSize: "13px" }}
                    >
                        🔄 Verileri Yenile
                    </button>
                </div>
            </div>

            {feedbackMsg && (
                <div
                    style={{
                        padding: "12px 16px",
                        borderRadius: "10px",
                        background: feedbackMsg.type === "success" ? "rgba(52, 211, 153, 0.15)" : "rgba(239, 68, 68, 0.15)",
                        color: feedbackMsg.type === "success" ? "#059669" : "#ef4444",
                        border: `1px solid ${feedbackMsg.type === "success" ? "rgba(52, 211, 153, 0.3)" : "rgba(239, 68, 68, 0.3)"}`,
                        fontSize: "13px",
                        fontWeight: 600,
                        display: "flex",
                        justifyContent: "space-between",
                        alignItems: "center"
                    }}
                >
                    <span>{feedbackMsg.type === "success" ? "✅ " : "⚠️ "} {feedbackMsg.text}</span>
                    <button
                        onClick={() => setFeedbackMsg(null)}
                        style={{
                            background: "transparent",
                            border: "none",
                            cursor: "pointer",
                            fontSize: "14px",
                            fontWeight: 800,
                            color: "inherit",
                            marginLeft: "12px"
                        }}
                    >
                        ✕
                    </button>
                </div>
            )}

            {/* System Overview KPI Cards */}
            {stats && (
                <div className="stats-grid">
                    <div className="stat-card">
                        <span className="stat-title">👥 Toplam Kullanıcı</span>
                        <div className="stat-value">{stats.totalUsers}</div>
                        <div className="stat-subtitle">{stats.activeUsersCount} Aktif / {stats.adminUsersCount} Yönetici</div>
                    </div>

                    <div className="stat-card">
                        <span className="stat-title">📁 Toplam Veri Seti</span>
                        <div className="stat-value">{stats.totalDatasets}</div>
                        <div className="stat-subtitle">Sistem geneli yüklenen datasetler</div>
                    </div>

                    <div className="stat-card">
                        <span className="stat-title">📊 İşlenen Satır Sayısı</span>
                        <div className="stat-value">{stats.totalRows.toLocaleString("tr-TR")}</div>
                        <div className="stat-subtitle">Toplam kayıt hacmi</div>
                    </div>

                    <div className="stat-card">
                        <span className="stat-title">💾 Kaydedilmiş Analizler</span>
                        <div className="stat-value">{stats.totalSavedAnalyses}</div>
                        <div className="stat-subtitle">Kullanıcıların kaydettiği analizler</div>
                    </div>
                </div>
            )}

            {/* Users Management Section */}
            <div className="panel">
                <div className="panel-header" style={{ flexWrap: "wrap", gap: "12px" }}>
                    <div>
                        <h4 className="desc-stat-column-title" style={{ fontSize: "16px", margin: 0 }}>
                            👤 Kullanıcı Yönetimi & Yetkilendirme ({filteredUsers.length})
                        </h4>
                        <p className="desc-stat-desc" style={{ marginTop: "4px" }}>
                            Kullanıcıların rollerini değiştirebilir, hesaplarını aktif/pasif duruma getirebilirsiniz.
                        </p>
                    </div>

                    <input
                        type="text"
                        className="input-field"
                        placeholder="🔍 İsim veya e-posta ile ara..."
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        style={{ maxWidth: "260px", padding: "8px 12px" }}
                    />
                </div>

                <div className="forecast-table-container" style={{ marginTop: "16px", maxHeight: "420px" }}>
                    <table className="heatmap-table" style={{ width: "100%", borderCollapse: "collapse" }}>
                        <thead className="forecast-table-head">
                            <tr>
                                <th style={{ textAlign: "left", padding: "12px 16px", width: "22%" }}>Ad Soyad & E-posta</th>
                                <th style={{ textAlign: "left", padding: "12px 16px", width: "18%" }}>Rol</th>
                                <th style={{ textAlign: "left", padding: "12px 16px", width: "14%" }}>Durum</th>
                                <th style={{ textAlign: "left", padding: "12px 16px", width: "14%" }}>Datasetler</th>
                                <th style={{ textAlign: "left", padding: "12px 16px", width: "14%" }}>Analizler</th>
                                <th style={{ textAlign: "left", padding: "12px 16px", width: "18%" }}>İşlem</th>
                            </tr>
                        </thead>
                        <tbody>
                            {isLoading ? (
                                <tr>
                                    <td colSpan={6} style={{ textAlign: "center", padding: "24px", color: "var(--text-muted)" }}>
                                        Kullanıcı listesi yükleniyor...
                                    </td>
                                </tr>
                            ) : filteredUsers.length === 0 ? (
                                <tr>
                                    <td colSpan={6} style={{ textAlign: "center", padding: "24px", color: "var(--text-muted)" }}>
                                        Kullanıcı bulunamadı.
                                    </td>
                                </tr>
                            ) : (
                                filteredUsers.map((u) => (
                                    <tr key={u.id}>
                                        <td style={{ textAlign: "left", padding: "12px 16px" }}>
                                            <div style={{ fontWeight: 700, color: "var(--text-main)" }}>
                                                {u.firstName} {u.lastName}
                                            </div>
                                            <div style={{ fontSize: "11px", color: "var(--text-muted)" }}>
                                                {u.email}
                                            </div>
                                        </td>

                                        <td style={{ textAlign: "left", padding: "12px 16px" }}>
                                            <select
                                                className="chart-select"
                                                value={u.role}
                                                onChange={(e) => handleRoleChange(u.id, Number(e.target.value))}
                                                disabled={isUpdating}
                                                style={{ padding: "4px 8px", fontSize: "12px", fontWeight: 700 }}
                                            >
                                                <option value={1}>🛡️ Admin</option>
                                                <option value={2}>📊 Analyst</option>
                                                <option value={3}>💼 Manager</option>
                                            </select>
                                        </td>

                                        <td style={{ textAlign: "left", padding: "12px 16px" }}>
                                            <span
                                                style={{
                                                    padding: "4px 10px",
                                                    borderRadius: "12px",
                                                    fontSize: "11px",
                                                    fontWeight: 700,
                                                    background: u.isActive ? "rgba(52, 211, 153, 0.15)" : "rgba(239, 68, 68, 0.15)",
                                                    color: u.isActive ? "#059669" : "#ef4444",
                                                    border: `1px solid ${u.isActive ? "rgba(52, 211, 153, 0.3)" : "rgba(239, 68, 68, 0.3)"}`
                                                }}
                                            >
                                                {u.isActive ? "🟢 Aktif" : "🔴 Pasif"}
                                            </span>
                                        </td>

                                        <td style={{ textAlign: "left", padding: "12px 16px", fontWeight: 700 }}>
                                            {u.datasetCount} Dataset
                                        </td>

                                        <td style={{ textAlign: "left", padding: "12px 16px", fontWeight: 700 }}>
                                            {u.savedAnalysisCount} Analiz
                                        </td>

                                        <td style={{ textAlign: "left", padding: "12px 16px" }}>
                                            <button
                                                className="expand-collapse-btn"
                                                onClick={() => handleToggleStatus(u.id)}
                                                disabled={isUpdating}
                                                style={{
                                                    padding: "4px 10px",
                                                    fontSize: "11px",
                                                    color: u.isActive ? "#ef4444" : "#059669",
                                                    borderColor: u.isActive ? "rgba(239, 68, 68, 0.4)" : "rgba(52, 211, 153, 0.4)"
                                                }}
                                            >
                                                {u.isActive ? "⛔ Pasife Al" : "✅ Aktif Et"}
                                            </button>
                                        </td>
                                    </tr>
                                ))
                            )}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};
