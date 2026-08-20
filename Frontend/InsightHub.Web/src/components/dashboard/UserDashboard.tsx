import React, { useState, useEffect } from "react";
import type { AuthUser, UserDashboardSummaryDto } from "../../types";
import * as api from "../../services/api";

interface UserDashboardProps {
    user: AuthUser;
    token: string | null;
    onNavigate: (tab: string, datasetId?: string, analysisId?: string) => void;
    onOpenUploadModal: () => void;
}

export const UserDashboard: React.FC<UserDashboardProps> = ({
    user,
    token,
    onNavigate,
    onOpenUploadModal
}) => {
    const [summary, setSummary] = useState<UserDashboardSummaryDto | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(true);

    const loadUserSummary = async () => {
        setIsLoading(true);
        try {
            const data = await api.fetchUserDashboardSummary(token);
            setSummary(data);
        } catch (err) {
            console.error("User summary load error", err);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        if (token) {
            loadUserSummary();
        }
    }, [token]);

    return (
        <div style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
            {/* WELCOME BANNER */}
            <div
                className="panel"
                style={{
                    background: "linear-gradient(135deg, rgba(236, 72, 153, 0.12) 0%, rgba(139, 92, 246, 0.12) 100%)",
                    border: "1px solid rgba(244, 114, 182, 0.3)",
                    padding: "24px 28px"
                }}
            >
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", flexWrap: "wrap", gap: "16px" }}>
                    <div>
                        <h2 className="desc-stat-column-title" style={{ fontSize: "24px", margin: 0 }}>
                            Hoş geldin, {user.firstName} {user.lastName} 👋
                        </h2>
                        <p className="desc-stat-desc" style={{ marginTop: "6px", fontSize: "14px" }}>
                            InsightHub İş Zekası ve Analitik Platformundasınız. Verilerinizi yükleyebilir, genel veya detaylı analizler oluşturabilir, yapay zeka modelleriyle tahminler yapabilirsiniz.
                        </p>
                    </div>

                    <div style={{ display: "flex", gap: "10px" }}>
                        <button
                            className="upload-button"
                            onClick={onOpenUploadModal}
                            style={{ padding: "10px 18px", fontSize: "13px" }}
                        >
                            📁 + Yeni Dataset Yükle
                        </button>
                    </div>
                </div>
            </div>

            {/* KPI METRIC CARDS */}
            <div className="stats-grid">
                <div className="stat-card">
                    <span className="stat-title">📁 Datasetlerim</span>
                    <div className="stat-value">{isLoading ? "..." : summary?.totalDatasets ?? 0}</div>
                    <div className="stat-subtitle">Yüklediğiniz veri setleri</div>
                </div>

                <div className="stat-card">
                    <span className="stat-title">💾 Kaydedilmiş Analizlerim</span>
                    <div className="stat-value">{isLoading ? "..." : summary?.totalSavedAnalyses ?? 0}</div>
                    <div className="stat-subtitle">Saklanan analiz snapshot'ları</div>
                </div>

                <div className="stat-card">
                    <span className="stat-title">📊 Toplam Satır Sayısı</span>
                    <div className="stat-value">{isLoading ? "..." : (summary?.totalRows ?? 0).toLocaleString("tr-TR")}</div>
                    <div className="stat-subtitle">İşlenen toplam veri hacmi</div>
                </div>

                <div className="stat-card">
                    <span className="stat-title">🕒 Son Yüklenen Dataset</span>
                    <div className="stat-value" style={{ fontSize: "18px", whiteSpace: "nowrap", overflow: "hidden", textOverflow: "ellipsis" }}>
                        {isLoading ? "..." : summary?.recentDatasetName || "Henüz Yok"}
                    </div>
                    <div className="stat-subtitle">
                        {summary?.recentDatasetUploadedAt ? new Date(summary.recentDatasetUploadedAt).toLocaleDateString("tr-TR") : "Yükleme bekleniyor"}
                    </div>
                </div>
            </div>

            {/* QUICK ACTIONS GRID */}
            <div className="panel">
                <div className="panel-header">
                    <div>
                        <h3 className="desc-stat-column-title">⚡ Hızlı İşlemler</h3>
                        <p className="desc-stat-desc">Sık kullanılan analitik ve yapay zeka modüllerine doğrudan geçiş yapın</p>
                    </div>
                </div>

                <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))", gap: "16px", marginTop: "16px" }}>
                    <div
                        className="stats-card-item"
                        style={{ cursor: "pointer", padding: "18px", borderRadius: "14px", border: "1px solid rgba(244, 114, 182, 0.3)" }}
                        onClick={() => onNavigate("general-analysis")}
                    >
                        <div style={{ fontSize: "28px", marginBottom: "8px" }}>📊</div>
                        <strong style={{ fontSize: "14px" }}>Genel Analiz</strong>
                        <p style={{ margin: "4px 0 0 0", fontSize: "12px", color: "var(--text-muted)" }}>Bar, Pie, Line ve Scatter grafikleriyle görsel keşif yapın.</p>
                    </div>

                    <div
                        className="stats-card-item"
                        style={{ cursor: "pointer", padding: "18px", borderRadius: "14px", border: "1px solid rgba(244, 114, 182, 0.3)" }}
                        onClick={() => onNavigate("analysis")}
                    >
                        <div style={{ fontSize: "28px", marginBottom: "8px" }}>🔬</div>
                        <strong style={{ fontSize: "14px" }}>Detaylı Analiz</strong>
                        <p style={{ margin: "4px 0 0 0", fontSize: "12px", color: "var(--text-muted)" }}>Korelasyon ısı haritası, IQR outlier ve histogram dağılımları.</p>
                    </div>

                    <div
                        className="stats-card-item"
                        style={{ cursor: "pointer", padding: "18px", borderRadius: "14px", border: "1px solid rgba(244, 114, 182, 0.3)" }}
                        onClick={() => onNavigate("ml-forecast")}
                    >
                        <div style={{ fontSize: "28px", marginBottom: "8px" }}>🤖</div>
                        <strong style={{ fontSize: "14px" }}>ML Tahminleme & Trendler</strong>
                        <p style={{ margin: "4px 0 0 0", fontSize: "12px", color: "var(--text-muted)" }}>ML.NET zaman serisi ile gelecek adımları öngörün.</p>
                    </div>

                    <div
                        className="stats-card-item"
                        style={{ cursor: "pointer", padding: "18px", borderRadius: "14px", border: "1px solid rgba(244, 114, 182, 0.3)" }}
                        onClick={() => onNavigate("ai-prediction")}
                    >
                        <div style={{ fontSize: "28px", marginBottom: "8px" }}>🧠</div>
                        <strong style={{ fontSize: "14px" }}>AI ile Tahmin Et</strong>
                        <p style={{ margin: "4px 0 0 0", fontSize: "12px", color: "var(--text-muted)" }}>Çoklu değişkenli yapay zeka modeli eğitip anında tahmin alın.</p>
                    </div>
                </div>
            </div>

            {/* 2 TABLES: SON DATASETLERİM & SON ANALİZLERİM */}
            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "24px" }}>
                {/* Son Datasetlerim */}
                <div className="panel">
                    <div className="panel-header">
                        <div>
                            <h4 className="desc-stat-column-title" style={{ fontSize: "16px", margin: 0 }}>📁 Son Datasetlerim</h4>
                        </div>
                        <button
                            className="expand-collapse-btn"
                            onClick={() => onNavigate("datasets")}
                            style={{ fontSize: "11px", padding: "4px 10px" }}
                        >
                            Tümünü Gör
                        </button>
                    </div>

                    {summary?.recentDatasets.length === 0 ? (
                        <p style={{ color: "var(--text-muted)", fontSize: "13px", padding: "20px 0" }}>Henüz veri seti yüklenmedi.</p>
                    ) : (
                        <div style={{ display: "flex", flexDirection: "column", gap: "10px", marginTop: "14px" }}>
                            {summary?.recentDatasets.map((d) => (
                                <div
                                    key={d.id}
                                    style={{
                                        display: "flex",
                                        justifyContent: "space-between",
                                        alignItems: "center",
                                        padding: "10px 14px",
                                        borderRadius: "10px",
                                        background: "var(--bg-app)",
                                        border: "1px solid var(--border-card)"
                                    }}
                                >
                                    <div>
                                        <div style={{ fontWeight: 700, fontSize: "13px" }}>{d.name}</div>
                                        <small style={{ color: "var(--text-muted)", fontSize: "11px" }}>
                                            {d.totalRows} satır · {d.totalColumns} sütun · {new Date(d.uploadedAt).toLocaleDateString("tr-TR")}
                                        </small>
                                    </div>
                                    <button
                                        className="upload-button"
                                        onClick={() => onNavigate("general-analysis", d.id)}
                                        style={{ padding: "4px 10px", fontSize: "11px" }}
                                    >
                                        Analiz Et
                                    </button>
                                </div>
                            ))}
                        </div>
                    )}
                </div>

                {/* Son Analizlerim */}
                <div className="panel">
                    <div className="panel-header">
                        <div>
                            <h4 className="desc-stat-column-title" style={{ fontSize: "16px", margin: 0 }}>💾 Son Analizlerim</h4>
                        </div>
                        <button
                            className="expand-collapse-btn"
                            onClick={() => onNavigate("saved-analysis")}
                            style={{ fontSize: "11px", padding: "4px 10px" }}
                        >
                            Tümünü Gör
                        </button>
                    </div>

                    {summary?.recentAnalyses.length === 0 ? (
                        <p style={{ color: "var(--text-muted)", fontSize: "13px", padding: "20px 0" }}>Henüz kaydedilmiş analiz bulunmuyor.</p>
                    ) : (
                        <div style={{ display: "flex", flexDirection: "column", gap: "10px", marginTop: "14px" }}>
                            {summary?.recentAnalyses.map((a) => (
                                <div
                                    key={a.id}
                                    style={{
                                        display: "flex",
                                        justifyContent: "space-between",
                                        alignItems: "center",
                                        padding: "10px 14px",
                                        borderRadius: "10px",
                                        background: "var(--bg-app)",
                                        border: "1px solid var(--border-card)"
                                    }}
                                >
                                    <div>
                                        <div style={{ fontWeight: 700, fontSize: "13px" }}>{a.title}</div>
                                        <small style={{ color: "var(--text-muted)", fontSize: "11px" }}>
                                            {a.datasetName} · {a.analysisType} · {new Date(a.createdDate).toLocaleDateString("tr-TR")}
                                        </small>
                                    </div>
                                    <button
                                        className="expand-collapse-btn"
                                        onClick={() => onNavigate("saved-analysis-detail", undefined, a.id)}
                                        style={{ padding: "4px 10px", fontSize: "11px", borderColor: "rgba(236, 72, 153, 0.4)", color: "#ec4899" }}
                                    >
                                        Görüntüle
                                    </button>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};
