import React from "react";
import type { DashboardSummary, DatasetListItem } from "../../types";

interface SystemOverviewCardProps {
    datasetsList: DatasetListItem[];
    dashboard: DashboardSummary | null;
    activeDatasetId: string;
    onSelectDataset: (id: string) => void;
    onOpenUploadModal: () => void;
}

export const SystemOverviewCard: React.FC<SystemOverviewCardProps> = ({
    datasetsList,
    dashboard,
    activeDatasetId,
    onSelectDataset,
    onOpenUploadModal
}) => {
    const totalSystemDatasets = datasetsList.length;
    const healthScore = dashboard ? (dashboard.totalMissingValues === 0 ? 100 : Math.max(0, 100 - dashboard.totalMissingValues)) : 100;

    return (
        <div style={{ display: "flex", flexDirection: "column", gap: "20px", marginBottom: "24px" }}>
            {/* System Summary KPI Cards */}
            <div className="stats-grid">
                <div className="stat-card">
                    <div className="stat-icon blue">📁</div>
                    <div>
                        <span>SİSTEMDEKİ VERİ SETLERİ</span>
                        <strong>{totalSystemDatasets} Adet</strong>
                    </div>
                </div>

                <div className="stat-card">
                    <div className="stat-icon purple">📊</div>
                    <div><span>TOPLAM SATIR (AKTİF)</span><strong>{dashboard?.totalRows || 0}</strong></div>
                </div>

                <div className="stat-card">
                    <div className="stat-icon green">📐</div>
                    <div><span>TOPLAM SÜTUN (AKTİF)</span><strong>{dashboard?.totalColumns || 0}</strong></div>
                </div>

                <div className="stat-card">
                    <div className="stat-icon orange">🔢</div>
                    <div><span>SAYISAL / METİN</span><strong>{dashboard?.numericColumns || 0} / {dashboard?.stringColumns || 0}</strong></div>
                </div>

                <div className="stat-card">
                    <div className="stat-icon red">❤️</div>
                    <div><span>VERİ SAĞLIĞI SKORU</span><strong>%{healthScore}</strong></div>
                </div>
            </div>

            {/* Current Dataset Selector Banner */}
            <section className="dataset-header">
                <div>
                    <span className="label">AKTİF VERİ SETİ PANOLARI</span>
                    <h2>{dashboard?.datasetName || "Veri Seti Yükleniyor..."}</h2>
                    <p>
                        Sistem kayıtlı · {dashboard?.totalRows || 0} satır · {dashboard?.totalColumns || 0} sütun
                    </p>
                </div>

                <div style={{ display: "flex", gap: "12px", alignItems: "center" }}>
                    <select
                        className="dataset-dropdown-select"
                        value={activeDatasetId}
                        onChange={(e) => onSelectDataset(e.target.value)}
                    >
                        {datasetsList.map((d) => (
                            <option key={d.id} value={d.id}>
                                {d.name} ({d.totalRows} satır)
                            </option>
                        ))}
                    </select>

                    <button className="upload-button" onClick={onOpenUploadModal}>
                        + Yeni Yükle
                    </button>
                </div>
            </section>
        </div>
    );
};
