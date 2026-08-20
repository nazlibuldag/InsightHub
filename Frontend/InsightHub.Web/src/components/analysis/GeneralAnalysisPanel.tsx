import React from "react";
import { SystemOverviewCard } from "../dashboard/SystemOverviewCard";
import { InteractiveChartsPanel } from "../charts/InteractiveChartsPanel";
import type { DatasetListItem, DatasetResponse, DashboardSummary } from "../../types";

interface GeneralAnalysisPanelProps {
    datasetsList: DatasetListItem[];
    activeDatasetId: string;
    setActiveDatasetId: (id: string) => void;
    dashboard: DashboardSummary | null;
    datasetDetails: DatasetResponse | null;
    token: string | null;
    onOpenUploadModal: () => void;
    onSaveAnalysis: () => void;
}

export const GeneralAnalysisPanel: React.FC<GeneralAnalysisPanelProps> = ({
    datasetsList,
    activeDatasetId,
    setActiveDatasetId,
    dashboard,
    datasetDetails,
    token,
    onOpenUploadModal,
    onSaveAnalysis
}) => {
    return (
        <div style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
            {/* Header & Save Action */}
            <div className="panel" style={{ padding: "18px 24px" }}>
                <div className="panel-header" style={{ flexWrap: "wrap", gap: "12px", margin: 0 }}>
                    <div>
                        <h3 className="desc-stat-column-title" style={{ fontSize: "18px", margin: 0 }}>
                            📊 Genel Analiz & Görsel Keşif
                        </h3>
                        <p className="desc-stat-desc" style={{ marginTop: "4px" }}>
                            Seçili veri setinin temel dağılım grafikleri, ortalamalar, kategorik pasta dilimleri ve sütun tipleri
                        </p>
                    </div>

                    <button
                        className="upload-button"
                        onClick={onSaveAnalysis}
                        style={{ padding: "8px 16px", fontSize: "13px" }}
                    >
                        💾 Bu Analizi Kaydet
                    </button>
                </div>
            </div>

            {/* System Overview KPIs & Active Dataset Banner */}
            <SystemOverviewCard
                datasetsList={datasetsList}
                dashboard={dashboard}
                activeDatasetId={activeDatasetId}
                onSelectDataset={setActiveDatasetId}
                onOpenUploadModal={onOpenUploadModal}
            />

            {/* Interactive Visual Charts (Bar, Pie, Line, Scatter) */}
            <InteractiveChartsPanel
                datasetId={activeDatasetId}
                columns={datasetDetails?.columns || []}
                token={token}
            />

            {/* Column Summary Table */}
            <div className="panel">
                <div className="panel-header">
                    <div>
                        <h3 className="desc-stat-column-title">📊 Sütun Özeti & Veri Tipleri</h3>
                        <p className="desc-stat-desc">Aktif veri setinin temel sütun bilgileri ve istatistikleri</p>
                    </div>
                </div>
                {datasetDetails && (
                    <div style={{ overflowX: "auto" }}>
                        <table className="heatmap-table" style={{ width: "100%" }}>
                            <thead>
                                <tr>
                                    <th style={{ textAlign: "left", padding: "12px 16px" }}>Sütun Adı</th>
                                    <th style={{ textAlign: "left", padding: "12px 16px" }}>Veri Tipi</th>
                                    <th style={{ textAlign: "left", padding: "12px 16px" }}>Eksik (Null)</th>
                                    <th style={{ textAlign: "left", padding: "12px 16px" }}>Benzersiz</th>
                                    <th style={{ textAlign: "left", padding: "12px 16px" }}>Ortalama</th>
                                    <th style={{ textAlign: "left", padding: "12px 16px" }}>Min / Max</th>
                                </tr>
                            </thead>
                            <tbody>
                                {datasetDetails.columns.map((col, idx) => (
                                    <tr key={idx}>
                                        <td style={{ fontWeight: 700, padding: "12px 16px", textAlign: "left" }}>{col.columnName}</td>
                                        <td style={{ padding: "12px 16px", textAlign: "left" }}>
                                            <span style={{ padding: "4px 8px", borderRadius: "6px", fontSize: "11px", fontWeight: 700, background: "rgba(99, 102, 241, 0.2)", color: "#818cf8" }}>
                                                {col.dataType === 1 ? "Sayısal" : col.dataType === 2 ? "Metin" : "Tarih"}
                                            </span>
                                        </td>
                                        <td style={{ padding: "12px 16px", textAlign: "left" }}>{col.nullCount}</td>
                                        <td style={{ padding: "12px 16px", textAlign: "left" }}>{col.uniqueCount}</td>
                                        <td style={{ padding: "12px 16px", fontWeight: 600, textAlign: "left" }}>{col.averageValue !== null ? col.averageValue.toFixed(2) : "-"}</td>
                                        <td style={{ padding: "12px 16px", textAlign: "left" }}>
                                            {col.minValue !== null && col.maxValue !== null ? `${col.minValue.toFixed(1)} / ${col.maxValue.toFixed(1)}` : "-"}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </div>
    );
};
