import { useEffect, useState } from "react";
import "./App.css";
import { Sidebar, type ActiveTabType } from "./components/common/Sidebar";
import { AuthPage } from "./components/auth/AuthPage";
import { FileUploaderModal } from "./components/datasets/FileUploaderModal";
import { UserDashboard } from "./components/dashboard/UserDashboard";
import { GeneralAnalysisPanel } from "./components/analysis/GeneralAnalysisPanel";
import { CorrelationHeatmap } from "./components/charts/CorrelationHeatmap";
import { DistributionHistogram } from "./components/charts/DistributionHistogram";
import { OutliersChart } from "./components/charts/OutliersChart";
import { DescriptiveStatsPanel } from "./components/charts/DescriptiveStatsPanel";
import { MlForecastPanel } from "./components/charts/MlForecastPanel";
import { AiPredictionPanel } from "./components/ai/AiPredictionPanel";
import { SaveAnalysisModal } from "./components/analysis/SaveAnalysisModal";
import { SavedAnalysisDetailPage } from "./components/analysis/SavedAnalysisDetailPage";
import { UserProfilePanel } from "./components/profile/UserProfilePanel";
import { AdminPanel } from "./components/admin/AdminPanel";
import { useSignalR } from "./hooks/useSignalR";
import * as api from "./services/api";
import type {
    AuthUser,
    DatasetListItem,
    DatasetResponse,
    DashboardSummary,
    CorrelationMatrixResponse,
    DescriptiveStatsResponse,
    SavedAnalysisDto
} from "./types";

const DEFAULT_DATASET_ID = "b39c4f4b-cf33-4e01-b359-d48c628ed8c4";

function App() {
    const [theme, setTheme] = useState<"dark" | "pink">(() => {
        return (localStorage.getItem("insighthub_theme") as "dark" | "pink") || "pink";
    });

    useEffect(() => {
        localStorage.setItem("insighthub_theme", theme);
        if (theme === "pink") {
            document.documentElement.classList.add("theme-pink");
        } else {
            document.documentElement.classList.remove("theme-pink");
        }
    }, [theme]);

    const [user, setUser] = useState<AuthUser | null>(() => {
        const saved = localStorage.getItem("insighthub_user");
        return saved ? JSON.parse(saved) : null;
    });

    const [token, setToken] = useState<string | null>(() =>
        localStorage.getItem("insighthub_token")
    );

    const isAdmin = Boolean(
        user && (user.role === 1 || user.role === 0 || (user as any).roleName === "Admin" || (user.role as any) === "Admin")
    );

    const [showUploadModal, setShowUploadModal] = useState<boolean>(false);
    const [showSaveAnalysisModal, setShowSaveAnalysisModal] = useState<boolean>(false);
    const [saveModalData, setSaveModalData] = useState<{
        defaultTitle: string;
        defaultAnalysisType: string;
        configuration: any;
        resultData: any;
    }>({
        defaultTitle: "",
        defaultAnalysisType: "Genel Analiz",
        configuration: {},
        resultData: {}
    });

    const [selectedAnalysisDetailId, setSelectedAnalysisDetailId] = useState<string | null>(null);

    const [activeDatasetId, setActiveDatasetId] = useState<string>(DEFAULT_DATASET_ID);
    const [activeTab, setActiveTab] = useState<ActiveTabType>("dashboard");

    // Guard: If non-admin tries to open admin tab, redirect to dashboard immediately
    useEffect(() => {
        if (activeTab === "admin" && !isAdmin) {
            setActiveTab("dashboard");
        }
    }, [activeTab, isAdmin]);

    // Expose tab switcher for topbar profile dropdown
    useEffect(() => {
        (window as any).onSidebarTabChange = (tab: ActiveTabType) => {
            setActiveTab(tab);
        };
        return () => {
            delete (window as any).onSidebarTabChange;
        };
    }, []);

    const { isConnected: isSignalRConnected, notification: signalRNotification, progress: signalRProgress } = useSignalR();

    const [datasetsList, setDatasetsList] = useState<DatasetListItem[]>([]);
    const [dashboard, setDashboard] = useState<DashboardSummary | null>(null);
    const [datasetDetails, setDatasetDetails] = useState<DatasetResponse | null>(null);
    const [correlationMatrix, setCorrelationMatrix] = useState<CorrelationMatrixResponse | null>(null);
    const [statsDataList, setStatsDataList] = useState<DescriptiveStatsResponse[]>([]);
    const [savedAnalysesList, setSavedAnalysesList] = useState<SavedAnalysisDto[]>([]);

    const [isLoadingStats, setIsLoadingStats] = useState<boolean>(false);
    const [isLoadingMatrix, setIsLoadingMatrix] = useState<boolean>(false);

    const numericCols = datasetDetails?.columns.filter((c) => c.dataType === 1).map((c) => c.columnName) || [];

    // Fetch dataset list
    const loadDatasets = async () => {
        try {
            const list = await api.fetchDatasets(token);
            setDatasetsList(list);
            if (list.length > 0 && (!activeDatasetId || activeDatasetId === DEFAULT_DATASET_ID)) {
                setActiveDatasetId(list[0].id);
            }
        } catch (err) {
            console.error("Dataset list fetch error", err);
        }
    };

    const loadDatasetData = async (id: string) => {
        if (!id) return;
        try {
            const [summary, details] = await Promise.all([
                api.fetchDashboardSummary(id, token).catch(() => null),
                api.fetchDatasetDetails(id, token).catch(() => null)
            ]);
            if (summary) setDashboard(summary);
            if (details) setDatasetDetails(details);
        } catch (err) {
            console.error("Dataset load error", err);
        }
    };

    const loadSavedAnalyses = async () => {
        try {
            const list = await api.fetchSavedAnalyses(token);
            setSavedAnalysesList(list);
        } catch (err) {
            console.error("Saved analyses fetch error", err);
        }
    };

    useEffect(() => {
        if (token) {
            loadDatasets();
        }
    }, [token]);

    useEffect(() => {
        if (activeDatasetId && token) {
            loadDatasetData(activeDatasetId);
        }
    }, [activeDatasetId, token]);

    // Tab Data Loaders
    useEffect(() => {
        if (!token) return;

        if (activeTab === "saved-analysis") {
            loadSavedAnalyses();
            return;
        }

        if (!activeDatasetId) return;

        if (activeTab === "analysis") {
            setIsLoadingMatrix(true);
            api.fetchCorrelationMatrix(activeDatasetId, token)
                .then(setCorrelationMatrix)
                .catch(console.error)
                .finally(() => setIsLoadingMatrix(false));

            if (numericCols.length > 0) {
                setIsLoadingStats(true);
                Promise.all(
                    numericCols.map((col) =>
                        api.fetchDescriptiveStats(activeDatasetId, col, token).catch(() => null)
                    )
                )
                    .then((results) => {
                        const valid = results.filter((r): r is DescriptiveStatsResponse => r !== null);
                        setStatsDataList(valid);
                    })
                    .catch(console.error)
                    .finally(() => setIsLoadingStats(false));
            }
        }
    }, [activeTab, activeDatasetId, datasetDetails, token]);

    const handleLogout = () => {
        localStorage.removeItem("insighthub_token");
        localStorage.removeItem("insighthub_user");
        setToken(null);
        setUser(null);
        setActiveTab("dashboard");
    };

    const handleLoginSuccess = (authUser: AuthUser, authToken: string) => {
        setUser(authUser);
        setToken(authToken);
        setActiveTab("dashboard");
        loadDatasets();
    };

    const handleUploadSuccess = (newDataset: DatasetResponse) => {
        loadDatasets();
        setActiveDatasetId(newDataset.id);
    };

    const handleOpenSaveModal = (
        defaultTitle: string,
        defaultAnalysisType: string,
        configuration: any = {},
        resultData: any = {}
    ) => {
        setSaveModalData({
            defaultTitle,
            defaultAnalysisType,
            configuration,
            resultData
        });
        setShowSaveAnalysisModal(true);
    };

    const handleDeleteSavedAnalysis = async (id: string) => {
        if (!window.confirm("Bu kaydedilmiş analizi silmek istediğinize emin misiniz?")) return;
        try {
            await api.deleteSavedAnalysis(id, token);
            await loadSavedAnalyses();
        } catch (err: any) {
            alert(err.message || "Analiz silinirken bir hata oluştu.");
        }
    };

    const handleDownloadSavedAnalysisPdf = async (id: string, title: string) => {
        try {
            await api.downloadSavedAnalysisPdf(id, title, token);
        } catch (err: any) {
            alert(err.message || "PDF indirilirken bir hata oluştu.");
        }
    };

    // If user is not authenticated, show the FULL-PAGE Login / Register screen
    if (!user || !token) {
        return (
            <AuthPage
                onLoginSuccess={handleLoginSuccess}
                theme={theme}
                setTheme={setTheme}
            />
        );
    }

    return (
        <div className={`app ${theme === "pink" ? "theme-pink" : ""}`}>
            <Sidebar
                activeTab={activeTab}
                setActiveTab={setActiveTab}
                user={user}
                handleLogout={handleLogout}
            />

            <main className="main">
                {/* TOPBAR */}
                <header className="topbar">
                    <div>
                        <h1>
                            {activeTab === "dashboard"
                                ? "Kullanıcı Dashboard"
                                : activeTab === "datasets"
                                ? "Veri Seti Yönetimi"
                                : activeTab === "general-analysis"
                                ? "Genel Analiz & Görsel Keşif"
                                : activeTab === "analysis"
                                ? "Detaylı İstatistiksel Analiz"
                                : activeTab === "ml-forecast"
                                ? "ML Tahminleme & Trendler"
                                : activeTab === "ai-prediction"
                                ? "AI ile Tahmin Et"
                                : activeTab === "saved-analysis"
                                ? "Kaydedilmiş Analizlerim"
                                : activeTab === "saved-analysis-detail"
                                ? "Kaydedilmiş Analiz Snapshot Detayı"
                                : activeTab === "profile"
                                ? "Kullanıcı Profili"
                                : activeTab === "admin"
                                ? "Sistem Yönetim & Admin Paneli"
                                : "Analitik Paneli"}
                        </h1>
                        <p>
                            {activeTab === "dashboard"
                                ? "Veri setleriniz, son analizleriniz ve hızlı analitik işlemleri"
                                : activeTab === "general-analysis"
                                ? "Bar, pasta, çizgi ve nokta grafikleri ile temel sütun dağılımı"
                                : activeTab === "analysis"
                                ? "Korelasyon ısı haritası, IQR outlier analizi ve tanımlayıcı istatistikler"
                                : activeTab === "ml-forecast"
                                ? "ML.NET zaman serisi algoritmalarıyla gelecek trend projeksiyonları"
                                : activeTab === "ai-prediction"
                                ? "Çoklu özellik regresyon modelleriyle hedef sütun kestirimi"
                                : "InsightHub Kurumsal Veri Analitiği Platformu"}
                        </p>
                    </div>

                    <div style={{ display: "flex", gap: "10px", alignItems: "center", flexWrap: "wrap" }}>
                        <button
                            className="theme-toggle-btn"
                            onClick={() => setTheme(theme === "dark" ? "pink" : "dark")}
                            style={{
                                padding: "10px 18px",
                                borderRadius: "12px",
                                border: theme === "pink" ? "1px solid #ec4899" : "1px solid rgba(255, 255, 255, 0.2)",
                                background: theme === "pink" ? "linear-gradient(135deg, #fce7f3 0%, #fbcfe8 100%)" : "linear-gradient(135deg, rgba(99, 102, 241, 0.2) 0%, rgba(168, 85, 247, 0.2) 100%)",
                                color: theme === "pink" ? "#9d174d" : "#f8fafc",
                                fontWeight: "700",
                                cursor: "pointer"
                            }}
                        >
                            {theme === "dark" ? "🌸 Pembeli Canlı Açık Tema" : "🌙 Koyu Gece Teması"}
                        </button>

                        <span
                            style={{
                                fontSize: 11,
                                fontWeight: 600,
                                padding: "4px 10px",
                                borderRadius: "12px",
                                background: isSignalRConnected ? "rgba(40, 167, 69, 0.1)" : "rgba(228, 93, 93, 0.1)",
                                color: isSignalRConnected ? "#28a745" : "#e45d5d",
                                border: `1px solid ${isSignalRConnected ? "#28a745" : "#e45d5d"}`,
                                display: "inline-flex",
                                alignItems: "center",
                                gap: "4px"
                            }}
                        >
                            {isSignalRConnected ? "🟢 SignalR Canlı" : "🔴 SignalR Çevrimdışı"}
                        </span>

                        <button className="upload-button" onClick={() => setShowUploadModal(true)}>
                            + Upload Dataset
                        </button>
                    </div>
                </header>

                {signalRNotification && (
                    <div
                        style={{
                            position: "fixed",
                            top: 20,
                            right: 20,
                            zIndex: 9999,
                            background: "linear-gradient(135deg, #635bff 0%, #a855f7 100%)",
                            color: "#fff",
                            padding: "12px 20px",
                            borderRadius: "12px",
                            boxShadow: "0 8px 24px rgba(99, 91, 255, 0.4)",
                            fontWeight: 600,
                            fontSize: "14px"
                        }}
                    >
                        {signalRNotification}
                    </div>
                )}

                {/* 1. DASHBOARD TAB */}
                {activeTab === "dashboard" && (
                    <UserDashboard
                        user={user}
                        token={token}
                        onNavigate={(tab, dsId, saId) => {
                            if (dsId) setActiveDatasetId(dsId);
                            if (saId) setSelectedAnalysisDetailId(saId);
                            setActiveTab(tab as ActiveTabType);
                        }}
                        onOpenUploadModal={() => setShowUploadModal(true)}
                    />
                )}

                {/* 2. GENERAL ANALYSIS TAB */}
                {activeTab === "general-analysis" && (
                    <GeneralAnalysisPanel
                        datasetsList={datasetsList}
                        activeDatasetId={activeDatasetId}
                        setActiveDatasetId={setActiveDatasetId}
                        dashboard={dashboard}
                        datasetDetails={datasetDetails}
                        token={token}
                        onOpenUploadModal={() => setShowUploadModal(true)}
                        onSaveAnalysis={() =>
                            handleOpenSaveModal(
                                `${datasetDetails?.name || "Dataset"} - Genel Görsel Analiz`,
                                "Genel Analiz",
                                { datasetId: activeDatasetId, datasetName: datasetDetails?.name },
                                {
                                    dashboardSummary: dashboard,
                                    columns: datasetDetails?.columns || [],
                                    savedAt: new Date().toISOString()
                                }
                            )
                        }
                    />
                )}

                {/* 3. DETAILED ANALYSIS TAB */}
                {activeTab === "analysis" && (
                    <div style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
                        <div className="panel" style={{ padding: "16px 24px" }}>
                            <div className="panel-header" style={{ margin: 0 }}>
                                <div>
                                    <h3 className="desc-stat-column-title" style={{ fontSize: "18px", margin: 0 }}>🔬 Detaylı İstatistiksel Analiz</h3>
                                    <p className="desc-stat-desc" style={{ marginTop: "4px" }}>Isı haritası korelasyon matrisi, IQR aykırı değer tespiti ve histogram dağılımları</p>
                                </div>
                                <button
                                    className="upload-button"
                                    onClick={() =>
                                        handleOpenSaveModal(
                                            `${datasetDetails?.name || "Dataset"} - Detaylı İstatistik Analizi`,
                                            "Detaylı Analiz",
                                            { datasetId: activeDatasetId },
                                            {
                                                correlationMatrix,
                                                statsList: statsDataList,
                                                savedAt: new Date().toISOString()
                                            }
                                        )
                                    }
                                    style={{ padding: "8px 16px", fontSize: "13px" }}
                                >
                                    💾 Bu Analizi Kaydet
                                </button>
                            </div>
                        </div>

                        <CorrelationHeatmap data={correlationMatrix} isLoading={isLoadingMatrix} />
                        <OutliersChart datasetId={activeDatasetId} numericColumns={numericCols} token={token} />
                        <DistributionHistogram datasetId={activeDatasetId} numericColumns={numericCols} token={token} />
                        <DescriptiveStatsPanel statsList={statsDataList} isLoading={isLoadingStats} />
                    </div>
                )}

                {/* 4. ML FORECAST TAB */}
                {activeTab === "ml-forecast" && (
                    <div style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
                        <MlForecastPanel
                            datasetId={activeDatasetId}
                            token={token}
                            onSaveForecast={(activeForecast, stepsAhead) => {
                                handleOpenSaveModal(
                                    `${datasetDetails?.name || "Dataset"} - ML ${activeForecast.targetColumn || activeForecast.columnName || "Trend"} Kestirimi`,
                                    "ML Tahminleme & Trendler",
                                    { datasetId: activeDatasetId, stepsAhead },
                                    {
                                        forecastResult: activeForecast,
                                        stepsAhead,
                                        savedAt: new Date().toISOString()
                                    }
                                );
                            }}
                        />
                    </div>
                )}

                {/* 5. AI PREDICTION TAB */}
                {activeTab === "ai-prediction" && (
                    <AiPredictionPanel
                        datasetId={activeDatasetId}
                        columns={datasetDetails?.columns || []}
                        token={token}
                        onSavePrediction={(predictionResult, config) => {
                            handleOpenSaveModal(
                                `${datasetDetails?.name || "Dataset"} - AI ${predictionResult.targetColumn} Tahmini`,
                                "AI ile Tahmin Et",
                                config,
                                {
                                    aiPredictionResult: predictionResult,
                                    savedAt: new Date().toISOString()
                                }
                            );
                        }}
                    />
                )}

                {/* 6. SAVED ANALYSIS TAB */}
                {activeTab === "saved-analysis" && (
                    <div className="panel">
                        <div className="panel-header" style={{ flexWrap: "wrap", gap: "12px" }}>
                            <div>
                                <h3 className="desc-stat-column-title">💾 Kaydedilmiş Analizlerim ({savedAnalysesList.length})</h3>
                                <p className="desc-stat-desc">Filtrelenmiş ve saklanmış analiz geçmişiniz, PDF raporları ve dondurulmuş sonuç snapshot'ları</p>
                            </div>
                            <button
                                className="upload-button"
                                onClick={() => setActiveTab("general-analysis")}
                                style={{ padding: "8px 18px", fontSize: "13px" }}
                            >
                                🚀 Yeni Analiz Başlat
                            </button>
                        </div>

                        {savedAnalysesList.length === 0 ? (
                            <div style={{ padding: "60px 20px", textAlign: "center" }}>
                                <div style={{ fontSize: "42px", marginBottom: "12px" }}>📂</div>
                                <h4 className="desc-stat-column-title" style={{ margin: 0 }}>Henüz kaydedilmiş analiz bulunmuyor.</h4>
                                <p className="desc-stat-desc" style={{ marginTop: "6px" }}>
                                    Genel Analiz, Detaylı Analiz, ML Tahmin veya AI Tahmin sayfalarında <strong>"Analizi Kaydet"</strong> butonuna basarak analizlerinizi dilediğiniz zaman incelemek üzere saklayabilirsiniz.
                                </p>
                                <button
                                    className="upload-button"
                                    onClick={() => setActiveTab("general-analysis")}
                                    style={{ marginTop: "16px", padding: "10px 20px" }}
                                >
                                    Analiz Sayfasına Git
                                </button>
                            </div>
                        ) : (
                            <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(320px, 1fr))", gap: "16px", marginTop: "20px" }}>
                                {savedAnalysesList.map((sa) => (
                                    <div
                                        key={sa.id}
                                        className="stats-card-item"
                                        style={{
                                            display: "flex",
                                            flexDirection: "column",
                                            justifyContent: "space-between",
                                            padding: "18px",
                                            borderRadius: "14px",
                                            border: "1px solid rgba(244, 114, 182, 0.3)",
                                            background: "var(--bg-card)",
                                            boxShadow: "0 4px 14px rgba(0, 0, 0, 0.05)"
                                        }}
                                    >
                                        <div>
                                            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", gap: "8px" }}>
                                                <h4 className="desc-stat-column-title" style={{ fontSize: "15px", margin: 0, lineHeight: 1.3 }}>
                                                    {sa.title}
                                                </h4>
                                                <span style={{ fontSize: "10px", fontWeight: 700, padding: "3px 8px", borderRadius: "12px", background: "rgba(236, 72, 153, 0.15)", color: "#ec4899", whiteSpace: "nowrap" }}>
                                                    {sa.analysisType || "Genel Analiz"}
                                                </span>
                                            </div>

                                            <div style={{ fontSize: "12px", color: "var(--text-muted)", marginTop: "6px" }}>
                                                📁 Veri Seti: <strong>{sa.datasetName}</strong>
                                            </div>

                                            <p style={{ margin: "10px 0", fontSize: "12px", color: "var(--text-main)", lineHeight: 1.4, minHeight: "34px" }}>
                                                {sa.notes ? (sa.notes.length > 90 ? sa.notes.slice(0, 90) + "..." : sa.notes) : "Açıklama belirtilmemiş."}
                                            </p>
                                        </div>

                                        <div>
                                            <div style={{ fontSize: "11px", color: "var(--text-muted)", marginBottom: "12px" }}>
                                                📅 {new Date(sa.createdDate).toLocaleDateString("tr-TR", { day: "numeric", month: "short", year: "numeric", hour: "2-digit", minute: "2-digit" })}
                                            </div>

                                            <div style={{ display: "flex", gap: "8px", borderTop: "1px solid rgba(255, 255, 255, 0.1)", paddingTop: "12px" }}>
                                                <button
                                                    className="upload-button"
                                                    onClick={() => {
                                                        setSelectedAnalysisDetailId(sa.id);
                                                        setActiveTab("saved-analysis-detail");
                                                    }}
                                                    style={{ flex: 1, padding: "7px 10px", fontSize: "12px", justifyContent: "center" }}
                                                >
                                                    👁️ Görüntüle
                                                </button>

                                                <button
                                                    className="expand-collapse-btn"
                                                    onClick={() => handleDownloadSavedAnalysisPdf(sa.id, sa.title)}
                                                    title="PDF Raporu İndir"
                                                    style={{ padding: "7px 10px", fontSize: "12px" }}
                                                >
                                                    📥 PDF
                                                </button>

                                                <button
                                                    className="expand-collapse-btn"
                                                    onClick={() => handleDeleteSavedAnalysis(sa.id)}
                                                    title="Analizi Sil"
                                                    style={{ padding: "7px 10px", fontSize: "12px", color: "#ef4444", borderColor: "rgba(239, 68, 68, 0.3)" }}
                                                >
                                                    🗑️
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                )}

                {/* 7. SAVED ANALYSIS DETAIL PAGE */}
                {activeTab === "saved-analysis-detail" && selectedAnalysisDetailId && (
                    <SavedAnalysisDetailPage
                        analysisId={selectedAnalysisDetailId}
                        token={token}
                        onBack={() => setActiveTab("saved-analysis")}
                        onOpenDataset={(dsId) => {
                            setActiveDatasetId(dsId);
                            setActiveTab("general-analysis");
                        }}
                    />
                )}

                {/* 8. PROFILE TAB */}
                {activeTab === "profile" && (
                    <UserProfilePanel
                        user={user}
                        totalDatasets={datasetsList.length}
                        totalSavedAnalyses={savedAnalysesList.length}
                        theme={theme}
                        setTheme={setTheme}
                        onLogout={handleLogout}
                    />
                )}

                {/* 9. ADMIN PANEL TAB */}
                {activeTab === "admin" && isAdmin && (
                    <AdminPanel token={token} />
                )}

                {/* DATASETS MANAGEMENT TAB */}
                {activeTab === "datasets" && (
                    <div className="panel">
                        <div className="panel-header">
                            <div>
                                <h3 className="desc-stat-column-title">📁 Veri Seti Yönetimi ({datasetsList.length})</h3>
                                <p className="desc-stat-desc">Hesabınıza kayıtlı tüm veri setleri ve detayları</p>
                            </div>
                            <button className="upload-button" onClick={() => setShowUploadModal(true)}>
                                + Yeni Veri Seti Yükle
                            </button>
                        </div>
                        <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))", gap: "16px", marginTop: "16px" }}>
                            {datasetsList.map((d) => (
                                <div
                                    key={d.id}
                                    className="stats-card-item"
                                    style={{
                                        cursor: "pointer",
                                        border: d.id === activeDatasetId ? "2px solid #ec4899" : "1px solid rgba(244, 114, 182, 0.2)",
                                        padding: "16px",
                                        borderRadius: "12px",
                                        background: "var(--bg-card)"
                                    }}
                                    onClick={() => {
                                        setActiveDatasetId(d.id);
                                        setActiveTab("general-analysis");
                                    }}
                                >
                                    <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                                        <span style={{ fontSize: "15px", color: "var(--text-main)", fontWeight: 700 }}>{d.name}</span>
                                        {d.id === activeDatasetId && (
                                            <span style={{ fontSize: "10px", padding: "2px 8px", borderRadius: "10px", background: "#ec4899", color: "#fff", fontWeight: 700 }}>
                                                Aktif
                                            </span>
                                        )}
                                    </div>
                                    <p style={{ margin: "8px 0", fontSize: "12px", color: "var(--text-muted)", minHeight: "32px" }}>
                                        {d.description || "Özel veri seti"}
                                    </p>
                                    <div style={{ fontSize: "11px", color: "var(--text-main)", fontWeight: 600, borderTop: "1px solid rgba(255, 255, 255, 0.1)", paddingTop: "8px" }}>
                                        📊 {d.totalRows} satır · {d.totalColumns} sütun
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                )}
            </main>

            {/* MODALS */}
            <FileUploaderModal
                isOpen={showUploadModal}
                onClose={() => setShowUploadModal(false)}
                token={token}
                onUploadSuccess={handleUploadSuccess}
                signalRProgress={signalRProgress}
            />

            <SaveAnalysisModal
                isOpen={showSaveAnalysisModal}
                onClose={() => setShowSaveAnalysisModal(false)}
                datasetId={activeDatasetId}
                datasetName={datasetDetails?.name || "Aktif Veri Seti"}
                defaultTitle={saveModalData.defaultTitle}
                defaultAnalysisType={saveModalData.defaultAnalysisType}
                configuration={saveModalData.configuration}
                resultData={saveModalData.resultData}
                token={token}
                onSavedSuccessfully={() => {
                    loadSavedAnalyses();
                    alert("Analiz snapshot'ı ve PDF raporu başarıyla kaydedildi!");
                }}
            />
        </div>
    );
}

export default App;