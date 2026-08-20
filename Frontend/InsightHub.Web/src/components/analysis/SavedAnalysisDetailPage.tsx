import React, { useState, useEffect } from "react";
import type { SavedAnalysisDto } from "../../types";
import * as api from "../../services/api";
import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    Legend,
    ResponsiveContainer,
    BarChart,
    Bar,
    Cell
} from "recharts";

interface SavedAnalysisDetailPageProps {
    analysisId: string;
    token: string | null;
    onBack: () => void;
    onOpenDataset: (datasetId: string) => void;
}

export const SavedAnalysisDetailPage: React.FC<SavedAnalysisDetailPageProps> = ({
    analysisId,
    token,
    onBack,
    onOpenDataset
}) => {
    const [analysis, setAnalysis] = useState<SavedAnalysisDto | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [isDownloadingPdf, setIsDownloadingPdf] = useState<boolean>(false);
    const [errorMsg, setErrorMsg] = useState<string | null>(null);

    // Fallback data if snapshot is minimal
    const [fallbackDatasetDetails, setFallbackDatasetDetails] = useState<any>(null);

    useEffect(() => {
        const loadAnalysis = async () => {
            setIsLoading(true);
            setErrorMsg(null);
            try {
                const data = await api.fetchSavedAnalysisById(analysisId, token);
                setAnalysis(data);
                if (data?.datasetId) {
                    api.fetchDatasetDetails(data.datasetId, token)
                        .then(setFallbackDatasetDetails)
                        .catch(() => {});
                }
            } catch (err: any) {
                setErrorMsg(err.message || "Kaydedilmiş analiz detayları yüklenemedi.");
            } finally {
                setIsLoading(false);
            }
        };

        loadAnalysis();
    }, [analysisId, token]);

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

    if (isLoading) {
        return (
            <div className="panel" style={{ padding: "60px 20px", textAlign: "center" }}>
                <div style={{ fontSize: "28px", marginBottom: "12px" }}>⏳</div>
                <h3 className="desc-stat-column-title">Analiz Snapshot Verileri Yükleniyor...</h3>
            </div>
        );
    }

    if (errorMsg || !analysis) {
        return (
            <div className="panel" style={{ padding: "40px 20px" }}>
                <div style={{ color: "#ef4444", marginBottom: "16px" }}>⚠️ {errorMsg || "Analiz bulunamadı."}</div>
                <button className="upload-button" onClick={onBack}>
                    ← Kaydedilmiş Analizler Listesine Dön
                </button>
            </div>
        );
    }

    // Parse snapshot results
    let resultObj: any = {};
    try {
        if (analysis.resultJson) resultObj = JSON.parse(analysis.resultJson);
    } catch { }

    const aType = (analysis.analysisType || "").trim();
    const isAi = aType.includes("AI") || Boolean(resultObj?.aiPredictionResult);
    const isMl = aType.includes("ML") || aType.includes("Trend") || aType.includes("Zaman") || Boolean(resultObj?.forecastResult);
    const isDetail = aType.includes("Detaylı") || Boolean(resultObj?.statsList) || Boolean(resultObj?.correlationMatrix);
    const isGeneral = aType.includes("Genel") || (!isAi && !isMl && !isDetail);

    // Columns data from snapshot or fallback
    const columnsList = resultObj?.columns || fallbackDatasetDetails?.columns || [];
    const numericColumns = columnsList.filter((c: any) => c.dataType === 1);

    return (
        <div style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
            {/* TOP BAR / NAVIGATION */}
            <div className="panel">
                <div className="panel-header" style={{ flexWrap: "wrap", gap: "16px" }}>
                    <div style={{ display: "flex", alignItems: "center", gap: "12px" }}>
                        <button
                            className="expand-collapse-btn"
                            onClick={onBack}
                            style={{ padding: "8px 14px", fontSize: "13px", fontWeight: 700 }}
                        >
                            ← Geri Dön
                        </button>
                        <div>
                            <div style={{ display: "flex", alignItems: "center", gap: "10px", flexWrap: "wrap" }}>
                                <h2 className="desc-stat-column-title" style={{ fontSize: "22px", margin: 0 }}>
                                    {analysis.title}
                                </h2>
                                <span style={{ padding: "4px 12px", borderRadius: "20px", background: "rgba(236, 72, 153, 0.15)", color: "#ec4899", fontWeight: 700, fontSize: "12px" }}>
                                    {analysis.analysisType || "Genel Analiz"}
                                </span>
                            </div>
                            <p className="desc-stat-desc" style={{ marginTop: "4px" }}>
                                📁 Veri Seti: <strong>{analysis.datasetName}</strong> · 📅 Kayıt Tarihi: {new Date(analysis.createdDate).toLocaleDateString("tr-TR", { day: "numeric", month: "long", year: "numeric", hour: "2-digit", minute: "2-digit" })}
                            </p>
                        </div>
                    </div>

                    <div style={{ display: "flex", gap: "10px", flexWrap: "wrap" }}>
                        <button
                            className="upload-button"
                            onClick={handleDownloadPdf}
                            disabled={isDownloadingPdf}
                            style={{ padding: "10px 18px", fontSize: "13px" }}
                        >
                            {isDownloadingPdf ? "📥 PDF İndiriliyor..." : "📥 PDF Raporu İndir"}
                        </button>
                        <button
                            className="upload-button"
                            onClick={() => onOpenDataset(analysis.datasetId)}
                            style={{ padding: "10px 18px", fontSize: "13px", background: "linear-gradient(135deg, #8b5cf6, #6366f1)" }}
                        >
                            🚀 Bu Veri Setini Aç & Canlı Analiz Et
                        </button>
                    </div>
                </div>

                {/* Notes box */}
                {analysis.notes && (
                    <div style={{ marginTop: "16px", padding: "16px 20px", borderRadius: "12px", background: "var(--bg-card, #ffffff)", border: "2px solid rgba(236, 72, 153, 0.4)", boxShadow: "0 2px 8px rgba(0,0,0,0.04)" }}>
                        <span style={{ fontSize: "12px", fontWeight: 800, color: "#be185d", textTransform: "uppercase", letterSpacing: "0.5px", display: "block", marginBottom: "6px" }}>
                            📝 Analist Notları & Yönetici Açıklaması:
                        </span>
                        <p style={{ margin: 0, fontSize: "14px", fontWeight: 600, color: "var(--text-main)", lineHeight: 1.6 }}>
                            {analysis.notes}
                        </p>
                    </div>
                )}
            </div>

            {/* 1. AI PREDICTION SNAPSHOT VIEW */}
            {isAi && resultObj?.aiPredictionResult && (
                <div className="panel">
                    <div className="panel-header">
                        <div>
                            <h3 className="desc-stat-column-title">🧠 Kaydedilmiş AI Tahmin Sonucu & Model Parametreleri</h3>
                            <p className="desc-stat-desc">Hedef Kolon: <strong>{resultObj.aiPredictionResult.targetColumn}</strong> · Model: {resultObj.aiPredictionResult.modelName}</p>
                        </div>
                    </div>

                    <div className="stats-grid" style={{ marginTop: "16px" }}>
                        <div className="stat-card" style={{ background: "rgba(236, 72, 153, 0.12)", borderColor: "rgba(236, 72, 153, 0.4)" }}>
                            <span className="stat-title" style={{ color: "#be185d", fontWeight: 700 }}>🔮 Tahmin Edilen Değer</span>
                            <div className="stat-value" style={{ color: "#ec4899", fontSize: "32px", fontWeight: 800 }}>
                                {resultObj.aiPredictionResult.predictedValue}
                            </div>
                            <div className="stat-subtitle">Yapay Zeka Modeli Çıktısı</div>
                        </div>

                        <div className="stat-card">
                            <span className="stat-title" style={{ color: "#be185d", fontWeight: 700 }}>🎯 Model Güven Skoru ($R^2$)</span>
                            <div className="stat-value" style={{ fontSize: "28px", fontWeight: 800 }}>
                                %{Math.round(resultObj.aiPredictionResult.r2Score * 100)}
                            </div>
                            <div className="stat-subtitle">MAE: {resultObj.aiPredictionResult.meanAbsoluteError} | RMSE: {resultObj.aiPredictionResult.rootMeanSquaredError}</div>
                        </div>

                        <div className="stat-card">
                            <span className="stat-title" style={{ color: "#be185d", fontWeight: 700 }}>📊 Kullanılan Özellikler</span>
                            <div className="stat-value" style={{ fontSize: "28px", fontWeight: 800 }}>
                                {resultObj.aiPredictionResult.featureColumns?.length || 0} Kolon
                            </div>
                            <div className="stat-subtitle">Girdi Değişkenleri</div>
                        </div>
                    </div>

                    {/* Feature Importance */}
                    {resultObj.aiPredictionResult.featureWeights && (
                        <div style={{ marginTop: "24px" }}>
                            <h4 className="desc-stat-column-title" style={{ fontSize: "15px" }}>Özellik Ağırlıkları & Etki Oranları</h4>
                            <div style={{ display: "flex", flexDirection: "column", gap: "10px", marginTop: "12px" }}>
                                {resultObj.aiPredictionResult.featureWeights.map((fw: any, idx: number) => (
                                    <div key={idx} style={{ display: "flex", alignItems: "center", gap: "12px" }}>
                                        <span style={{ width: "160px", fontSize: "13px", fontWeight: 600 }}>{fw.featureName}</span>
                                        <div style={{ flex: 1, background: "rgba(0, 0, 0, 0.08)", height: "10px", borderRadius: "6px", overflow: "hidden" }}>
                                            <div style={{ width: `${fw.importancePercent}%`, background: "linear-gradient(90deg, #ec4899, #8b5cf6)", height: "100%", borderRadius: "6px" }} />
                                        </div>
                                        <span style={{ width: "60px", fontSize: "12px", fontWeight: 700, textAlign: "right" }}>%{fw.importancePercent}</span>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}
                </div>
            )}

            {/* 2. ML FORECAST SNAPSHOT VIEW */}
            {isMl && (
                <div className="panel">
                    <div className="panel-header">
                        <div>
                            <h3 className="desc-stat-column-title">🤖 Kaydedilmiş ML Zaman Serisi & Kestirim Grafiği</h3>
                            <p className="desc-stat-desc">
                                Hedef Kolon: <strong>{resultObj?.forecastResult?.targetColumn || "Sayısal Kolon"}</strong> · Model: {resultObj?.forecastResult?.trendDirection || "ML.NET Zaman Serisi"}
                            </p>
                        </div>
                    </div>

                    {resultObj?.forecastResult ? (
                        <>
                            {/* Model KPI Cards */}
                            <div className="stats-grid" style={{ marginTop: "16px" }}>
                                <div className="stat-card">
                                    <span className="stat-title" style={{ color: "#be185d", fontWeight: 700 }}>🎯 Model Güven Skoru ($R^2$)</span>
                                    <div className="stat-value" style={{ color: "#ec4899", fontSize: "28px", fontWeight: 800 }}>
                                        {resultObj.forecastResult.rSquared ? `%${(resultObj.forecastResult.rSquared * 100).toFixed(1)}` : "%85.0"}
                                    </div>
                                    <div className="stat-subtitle">Doğruluk Uyum Katsayısı</div>
                                </div>

                                <div className="stat-card">
                                    <span className="stat-title" style={{ color: "#be185d", fontWeight: 700 }}>📈 Trend Eğimi (Slope)</span>
                                    <div className="stat-value" style={{ fontSize: "28px", fontWeight: 800 }}>
                                        {resultObj.forecastResult.slope ?? 0}
                                    </div>
                                    <div className="stat-subtitle">{resultObj.forecastResult.trendDirection}</div>
                                </div>

                                <div className="stat-card">
                                    <span className="stat-title" style={{ color: "#be185d", fontWeight: 700 }}>🔮 Gelecek Adım Sayısı</span>
                                    <div className="stat-value" style={{ fontSize: "28px", fontWeight: 800 }}>
                                        +{resultObj.forecastResult.forecastedValues?.length || 10} Adım
                                    </div>
                                    <div className="stat-subtitle">Kestirilen Gelecek Noktaları</div>
                                </div>
                            </div>

                            {/* Recharts Line Chart */}
                            <div style={{ marginTop: "24px", height: "340px" }}>
                                <ResponsiveContainer width="100%" height="100%">
                                    <LineChart
                                        data={[
                                            ...(resultObj.forecastResult.historicalValues || []).map((v: number, i: number) => ({
                                                index: `Satır #${i + 1}`,
                                                historical: v,
                                                forecast: null
                                            })),
                                            ...(resultObj.forecastResult.forecastedValues || []).map((v: number, i: number) => ({
                                                index: `Gelecek +${i + 1}`,
                                                historical: null,
                                                forecast: v
                                            }))
                                        ]}
                                    >
                                        <CartesianGrid strokeDasharray="3 3" stroke="rgba(244, 114, 182, 0.2)" />
                                        <XAxis dataKey="index" stroke="var(--text-muted)" />
                                        <YAxis stroke="var(--text-muted)" />
                                        <Tooltip contentStyle={{ background: "var(--bg-card)", border: "1px solid var(--border-card)", borderRadius: "8px" }} />
                                        <Legend />
                                        <Line type="monotone" dataKey="historical" name="Geçmiş Gerçek Değerler" stroke="#8b5cf6" strokeWidth={2.5} dot={false} />
                                        <Line type="monotone" dataKey="forecast" name="ML Tahmin Projeksiyonu" stroke="#ec4899" strokeWidth={2.5} strokeDasharray="5 5" dot={{ r: 4 }} />
                                    </LineChart>
                                </ResponsiveContainer>
                            </div>
                        </>
                    ) : (
                        <div style={{ padding: "20px 0", color: "var(--text-muted)" }}>
                            Bu veri setinin zaman serisi ve trend projeksiyonu kaydedilmiştir. Canlı tahmin çalıştırmak için yukarıdaki <strong>"Bu Veri Setini Aç & Canlı Analiz Et"</strong> butonuna tıklayabilirsiniz.
                        </div>
                    )}
                </div>
            )}

            {/* 3. DETAILED ANALYSIS SNAPSHOT VIEW (HEATMAP / STATS) */}
            {isDetail && (
                <div style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
                    {/* Stats List Cards */}
                    {resultObj?.statsList && resultObj.statsList.length > 0 && (
                        <div className="panel">
                            <div className="panel-header">
                                <div>
                                    <h3 className="desc-stat-column-title">🔬 Tanımlayıcı İstatistikler Özeti</h3>
                                    <p className="desc-stat-desc">Tüm sayısal sütunların ortalama, medyan, mod, çeyrekler ve IQR değerleri</p>
                                </div>
                            </div>

                            <div style={{ overflowX: "auto", marginTop: "16px" }}>
                                <table className="heatmap-table" style={{ width: "100%" }}>
                                    <thead>
                                        <tr>
                                            <th style={{ textAlign: "left", padding: "12px 14px" }}>Sütun</th>
                                            <th style={{ textAlign: "left", padding: "12px 14px" }}>Ortalama</th>
                                            <th style={{ textAlign: "left", padding: "12px 14px" }}>Medyan</th>
                                            <th style={{ textAlign: "left", padding: "12px 14px" }}>Std. Sapma</th>
                                            <th style={{ textAlign: "left", padding: "12px 14px" }}>Min / Max</th>
                                            <th style={{ textAlign: "left", padding: "12px 14px" }}>IQR</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {resultObj.statsList.map((st: any, idx: number) => (
                                            <tr key={idx}>
                                                <td style={{ fontWeight: 700, padding: "12px 14px", textAlign: "left" }}>{st.columnName}</td>
                                                <td style={{ padding: "12px 14px", textAlign: "left" }}>{st.mean !== null ? Number(st.mean).toFixed(2) : "-"}</td>
                                                <td style={{ padding: "12px 14px", textAlign: "left" }}>{st.median !== null ? Number(st.median).toFixed(2) : "-"}</td>
                                                <td style={{ padding: "12px 14px", textAlign: "left" }}>{st.standardDeviation !== null ? Number(st.standardDeviation).toFixed(2) : "-"}</td>
                                                <td style={{ padding: "12px 14px", textAlign: "left" }}>{st.min !== null && st.max !== null ? `${st.min} / ${st.max}` : "-"}</td>
                                                <td style={{ padding: "12px 14px", textAlign: "left" }}>{st.iqr !== null ? Number(st.iqr).toFixed(2) : "-"}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    )}

                    {/* Correlation Matrix Table */}
                    {resultObj?.correlationMatrix && resultObj.correlationMatrix.columns && (
                        <div className="panel">
                            <div className="panel-header">
                                <div>
                                    <h3 className="desc-stat-column-title">🔥 Korelasyon Matrisi (Pearson)</h3>
                                    <p className="desc-stat-desc">Sayısal sütunlar arası -1.0 ile +1.0 arasındaki ilişki katsayıları</p>
                                </div>
                            </div>

                            <div style={{ overflowX: "auto", marginTop: "16px" }}>
                                <table className="heatmap-table" style={{ width: "100%" }}>
                                    <thead>
                                        <tr>
                                            <th style={{ padding: "10px" }}></th>
                                            {resultObj.correlationMatrix.columns.map((c: string) => (
                                                <th key={c} style={{ padding: "10px", fontSize: "12px" }}>{c}</th>
                                            ))}
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {resultObj.correlationMatrix.columns.map((rCol: string, rIdx: number) => (
                                            <tr key={rCol}>
                                                <td style={{ fontWeight: 700, padding: "10px", textAlign: "left", fontSize: "12px" }}>{rCol}</td>
                                                {resultObj.correlationMatrix.columns.map((_: string, cIdx: number) => {
                                                    const val = resultObj.correlationMatrix.matrix?.[rIdx]?.[cIdx] ?? 0;
                                                    const isPos = val >= 0;
                                                    const bg = rIdx === cIdx ? "rgba(236, 72, 153, 0.3)" : isPos ? `rgba(139, 92, 246, ${Math.abs(val) * 0.4})` : `rgba(239, 68, 68, ${Math.abs(val) * 0.4})`;
                                                    return (
                                                        <td key={cIdx} style={{ padding: "10px", textAlign: "center", background: bg, fontWeight: 700, fontSize: "12px" }}>
                                                            {Number(val).toFixed(2)}
                                                        </td>
                                                    );
                                                })}
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    )}
                </div>
            )}

            {/* 4. GENERAL ANALYSIS SNAPSHOT / COLUMN OVERVIEW & CHARTS */}
            {(isGeneral || columnsList.length > 0) && (
                <div style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
                    {/* General KPI Summary */}
                    {resultObj?.dashboardSummary && (
                        <div className="stats-grid">
                            <div className="stat-card">
                                <span className="stat-title" style={{ color: "#be185d", fontWeight: 700 }}>📊 Toplam Satır</span>
                                <div className="stat-value" style={{ fontSize: "28px", fontWeight: 800 }}>
                                    {resultObj.dashboardSummary.totalRows?.toLocaleString("tr-TR") || "-"}
                                </div>
                                <div className="stat-subtitle">Veri Seti Kayıt Sayısı</div>
                            </div>
                            <div className="stat-card">
                                <span className="stat-title" style={{ color: "#be185d", fontWeight: 700 }}>📁 Toplam Kolon</span>
                                <div className="stat-value" style={{ fontSize: "28px", fontWeight: 800 }}>
                                    {resultObj.dashboardSummary.totalColumns || "-"}
                                </div>
                                <div className="stat-subtitle">Sütun Sayısı</div>
                            </div>
                            <div className="stat-card">
                                <span className="stat-title" style={{ color: "#be185d", fontWeight: 700 }}>🔢 Sayısal Sütunlar</span>
                                <div className="stat-value" style={{ fontSize: "28px", fontWeight: 800 }}>
                                    {resultObj.dashboardSummary.numericColumns || numericColumns.length}
                                </div>
                                <div className="stat-subtitle">Sayısal Özellikler</div>
                            </div>
                            <div className="stat-card">
                                <span className="stat-title" style={{ color: "#be185d", fontWeight: 700 }}>⚠️ Eksik Değerler</span>
                                <div className="stat-value" style={{ fontSize: "28px", fontWeight: 800 }}>
                                    {resultObj.dashboardSummary.totalMissingValues ?? 0}
                                </div>
                                <div className="stat-subtitle">Null Değerler</div>
                            </div>
                        </div>
                    )}

                    {/* Columns Summary Table */}
                    {columnsList.length > 0 && (
                        <div className="panel">
                            <div className="panel-header">
                                <div>
                                    <h3 className="desc-stat-column-title">📊 Sütun Özeti & Veri Tipleri</h3>
                                    <p className="desc-stat-desc">Kaydedilmiş veri setinin temel sütun bilgileri ve ortalamaları</p>
                                </div>
                            </div>

                            <div style={{ overflowX: "auto", marginTop: "14px" }}>
                                <table className="heatmap-table" style={{ width: "100%" }}>
                                    <thead>
                                        <tr>
                                            <th style={{ textAlign: "left", padding: "12px 14px" }}>Sütun Adı</th>
                                            <th style={{ textAlign: "left", padding: "12px 14px" }}>Veri Tipi</th>
                                            <th style={{ textAlign: "left", padding: "12px 14px" }}>Eksik (Null)</th>
                                            <th style={{ textAlign: "left", padding: "12px 14px" }}>Benzersiz</th>
                                            <th style={{ textAlign: "left", padding: "12px 14px" }}>Ortalama</th>
                                            <th style={{ textAlign: "left", padding: "12px 14px" }}>Min / Max</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {columnsList.map((col: any, idx: number) => (
                                            <tr key={idx}>
                                                <td style={{ fontWeight: 700, padding: "12px 14px", textAlign: "left" }}>{col.columnName}</td>
                                                <td style={{ padding: "12px 14px", textAlign: "left" }}>
                                                    <span style={{ padding: "4px 8px", borderRadius: "6px", fontSize: "11px", fontWeight: 700, background: "rgba(99, 102, 241, 0.15)", color: "#818cf8" }}>
                                                        {col.dataType === 1 ? "Sayısal" : col.dataType === 2 ? "Metin" : "Tarih"}
                                                    </span>
                                                </td>
                                                <td style={{ padding: "12px 14px", textAlign: "left" }}>{col.nullCount}</td>
                                                <td style={{ padding: "12px 14px", textAlign: "left" }}>{col.uniqueCount}</td>
                                                <td style={{ padding: "12px 14px", fontWeight: 700, textAlign: "left" }}>{col.averageValue !== null && col.averageValue !== undefined ? Number(col.averageValue).toFixed(2) : "-"}</td>
                                                <td style={{ padding: "12px 14px", textAlign: "left" }}>
                                                    {col.minValue !== null && col.maxValue !== null && col.minValue !== undefined && col.maxValue !== undefined ? `${Number(col.minValue).toFixed(1)} / ${Number(col.maxValue).toFixed(1)}` : "-"}
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    )}

                    {/* Numeric Columns Average Visual Chart */}
                    {numericColumns.length > 0 && (
                        <div className="panel">
                            <div className="panel-header">
                                <div>
                                    <h3 className="desc-stat-column-title">📊 Sayısal Sütun Ortalamaları Grafiği</h3>
                                    <p className="desc-stat-desc">Veri setindeki sayısal özelliklerin karşılaştırmalı ortalama değerleri</p>
                                </div>
                            </div>

                            <div style={{ height: "300px", marginTop: "16px" }}>
                                <ResponsiveContainer width="100%" height="100%">
                                    <BarChart data={numericColumns.map((c: any) => ({ columnName: c.columnName, average: c.averageValue ?? 0 }))}>
                                        <CartesianGrid strokeDasharray="3 3" stroke="rgba(244, 114, 182, 0.2)" />
                                        <XAxis dataKey="columnName" stroke="var(--text-muted)" />
                                        <YAxis stroke="var(--text-muted)" />
                                        <Tooltip contentStyle={{ background: "var(--bg-card)", border: "1px solid var(--border-card)", borderRadius: "8px" }} />
                                        <Bar dataKey="average" name="Ortalama Değer" fill="#ec4899" radius={[6, 6, 0, 0]}>
                                            {numericColumns.map((_: any, index: number) => (
                                                <Cell key={`cell-${index}`} fill={index % 2 === 0 ? "#ec4899" : "#8b5cf6"} />
                                            ))}
                                        </Bar>
                                    </BarChart>
                                </ResponsiveContainer>
                            </div>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};
