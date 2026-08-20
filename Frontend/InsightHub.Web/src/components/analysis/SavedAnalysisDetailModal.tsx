import React, { useState, useEffect } from "react";
import type { SavedAnalysisDto } from "../../types";
import * as api from "../../services/api";

interface SavedAnalysisDetailModalProps {
    analysisId: string | null;
    onClose: () => void;
    token: string | null;
    onOpenDataset: (datasetId: string) => void;
}

export const SavedAnalysisDetailModal: React.FC<SavedAnalysisDetailModalProps> = ({
    analysisId,
    onClose,
    token,
    onOpenDataset
}) => {
    const [analysis, setAnalysis] = useState<SavedAnalysisDto | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [isDownloadingPdf, setIsDownloadingPdf] = useState<boolean>(false);
    const [errorMsg, setErrorMsg] = useState<string | null>(null);

    useEffect(() => {
        if (!analysisId) {
            setAnalysis(null);
            return;
        }

        const loadDetail = async () => {
            setIsLoading(true);
            setErrorMsg(null);
            try {
                const data = await api.fetchSavedAnalysisById(analysisId, token);
                setAnalysis(data);
            } catch (err: any) {
                setErrorMsg(err.message || "Analiz detayları yüklenemedi.");
            } finally {
                setIsLoading(false);
            }
        };

        loadDetail();
    }, [analysisId, token]);

    if (!analysisId) return null;

    const handleDownloadPdf = async () => {
        if (!analysis) return;
        setIsDownloadingPdf(true);
        try {
            await api.downloadSavedAnalysisPdf(analysis.id, analysis.title, token);
        } catch (err: any) {
            alert(err.message || "PDF indirilirken bir hata oluştu.");
        } finally {
            setIsDownloadingPdf(false);
        }
    };

    return (
        <div className="modal-overlay">
            <div className="modal-content" style={{ maxWidth: "680px" }}>
                <div className="modal-header">
                    <h3 className="desc-stat-column-title">📑 Kaydedilmiş Analiz Detayı</h3>
                    <button className="modal-close-btn" onClick={onClose}>&times;</button>
                </div>

                {isLoading ? (
                    <div style={{ padding: "40px 0", textAlign: "center", color: "var(--text-muted)" }}>
                        Analiz bilgileri getiriliyor...
                    </div>
                ) : errorMsg ? (
                    <div style={{ padding: "20px", color: "#ef4444" }}>⚠️ {errorMsg}</div>
                ) : analysis ? (
                    <div style={{ display: "flex", flexDirection: "column", gap: "20px", marginTop: "16px" }}>
                        {/* Header Info */}
                        <div style={{ background: "rgba(244, 114, 182, 0.08)", padding: "16px", borderRadius: "12px", border: "1px solid rgba(244, 114, 182, 0.2)" }}>
                            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", flexWrap: "wrap", gap: "10px" }}>
                                <div>
                                    <h4 className="desc-stat-column-title" style={{ fontSize: "18px", margin: 0 }}>
                                        {analysis.title}
                                    </h4>
                                    <p className="desc-stat-desc" style={{ marginTop: "4px" }}>
                                        📁 Veri Seti: <strong>{analysis.datasetName}</strong>
                                    </p>
                                </div>

                                <span style={{ padding: "4px 12px", borderRadius: "20px", background: "rgba(236, 72, 153, 0.2)", color: "#ec4899", fontWeight: 700, fontSize: "12px" }}>
                                    {analysis.analysisType || "Genel Analiz"}
                                </span>
                            </div>

                            <div style={{ display: "flex", gap: "16px", marginTop: "12px", fontSize: "12px", color: "var(--text-muted)" }}>
                                <span>📅 Kayıt Tarihi: {new Date(analysis.createdDate).toLocaleDateString("tr-TR", { day: "numeric", month: "long", year: "numeric", hour: "2-digit", minute: "2-digit" })}</span>
                            </div>
                        </div>

                        {/* Notes */}
                        {analysis.notes && (
                            <div>
                                <label className="desc-stat-lbl">📝 Analiz Notları:</label>
                                <div style={{ background: "var(--bg-card)", padding: "12px 16px", borderRadius: "10px", border: "1px solid var(--border-card)", fontSize: "13px", color: "var(--text-main)", lineHeight: 1.5 }}>
                                    {analysis.notes}
                                </div>
                            </div>
                        )}

                        {/* Configuration / Filter JSON Preview */}
                        {analysis.filterJson && analysis.filterJson !== "{}" && (
                            <div>
                                <label className="desc-stat-lbl">🔍 Yapılandırma & Filtre Parametreleri:</label>
                                <pre style={{ background: "rgba(0, 0, 0, 0.2)", padding: "12px", borderRadius: "8px", fontSize: "11px", color: "var(--text-muted)", overflowX: "auto" }}>
                                    {analysis.filterJson}
                                </pre>
                            </div>
                        )}

                        {/* Action Buttons */}
                        <div style={{ display: "flex", gap: "12px", justifyContent: "space-between", flexWrap: "wrap", marginTop: "10px", borderTop: "1px solid rgba(255, 255, 255, 0.1)", paddingTop: "16px" }}>
                            <button
                                type="button"
                                className="upload-button"
                                onClick={() => {
                                    onOpenDataset(analysis.datasetId);
                                    onClose();
                                }}
                                style={{ background: "linear-gradient(135deg, #8b5cf6, #6366f1)" }}
                            >
                                🚀 Bu Veri Setini Aç & Analiz Et
                            </button>

                            <div style={{ display: "flex", gap: "10px" }}>
                                <button
                                    type="button"
                                    className="upload-button"
                                    onClick={handleDownloadPdf}
                                    disabled={isDownloadingPdf}
                                >
                                    {isDownloadingPdf ? "PDF İndiriliyor..." : "📥 PDF Raporu İndir"}
                                </button>
                                <button
                                    type="button"
                                    className="expand-collapse-btn"
                                    onClick={onClose}
                                >
                                    Kapat
                                </button>
                            </div>
                        </div>
                    </div>
                ) : null}
            </div>
        </div>
    );
};
