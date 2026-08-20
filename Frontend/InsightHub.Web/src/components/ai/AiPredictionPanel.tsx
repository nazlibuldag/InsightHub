import React, { useState, useEffect } from "react";
import type { DatasetColumn, AiPredictionResultDto } from "../../types";
import * as api from "../../services/api";

interface AiPredictionPanelProps {
    datasetId: string;
    columns: DatasetColumn[];
    token: string | null;
    onSavePrediction: (predictionResult: AiPredictionResultDto, config: any) => void;
}

export const AiPredictionPanel: React.FC<AiPredictionPanelProps> = ({
    datasetId,
    columns,
    token,
    onSavePrediction
}) => {
    const numericCols = columns.filter((c) => c.dataType === 1).map((c) => c.columnName);

    const [targetColumn, setTargetColumn] = useState<string>(numericCols[0] || "");
    const [selectedFeatures, setSelectedFeatures] = useState<string[]>([]);
    const [modelType, setModelType] = useState<string>("Auto");
    const [isTraining, setIsTraining] = useState<boolean>(false);
    const [predictionResult, setPredictionResult] = useState<AiPredictionResultDto | null>(null);
    const [inputValues, setInputValues] = useState<Record<string, number>>({});
    const [errorMsg, setErrorMsg] = useState<string | null>(null);

    // Default target & features
    useEffect(() => {
        if (numericCols.length > 0) {
            const target = numericCols[numericCols.length - 1] || numericCols[0];
            setTargetColumn(target);
            const feats = numericCols.filter((c) => c !== target);
            setSelectedFeatures(feats);

            const initialInputs: Record<string, number> = {};
            feats.forEach((f) => {
                const col = columns.find((c) => c.columnName === f);
                initialInputs[f] = col?.averageValue !== undefined && col?.averageValue !== null
                    ? Number(col.averageValue.toFixed(2))
                    : 50;
            });
            setInputValues(initialInputs);
        }
    }, [datasetId, columns]);

    const handleFeatureToggle = (colName: string) => {
        if (selectedFeatures.includes(colName)) {
            setSelectedFeatures(selectedFeatures.filter((f) => f !== colName));
        } else {
            setSelectedFeatures([...selectedFeatures, colName]);
        }
    };

    const handleTrainAndPredict = async () => {
        if (!targetColumn) {
            setErrorMsg("Lütfen tahmin edilecek bir hedef sütun seçin.");
            return;
        }

        setIsTraining(true);
        setErrorMsg(null);

        try {
            const result = await api.executeAiPrediction(
                datasetId,
                {
                    targetColumn,
                    featureColumns: selectedFeatures,
                    modelType,
                    inputValues
                },
                token
            );
            setPredictionResult(result);
        } catch (err: any) {
            setErrorMsg(err.message || "Model eğitilirken bir hata oluştu.");
        } finally {
            setIsTraining(false);
        }
    };

    return (
        <div style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
            {/* Header */}
            <div className="panel">
                <div className="panel-header" style={{ flexWrap: "wrap", gap: "12px" }}>
                    <div>
                        <h3 className="desc-stat-column-title">🧠 AI ile Tahmin Et (Makine Öğrenmesi & Model Eğitimi)</h3>
                        <p className="desc-stat-desc">
                            Veri setinizdeki sütunları kullanarak çok değişkenli yapay zeka modeli eğitin, yeni değerler için anında tahmin üretin
                        </p>
                    </div>

                    {predictionResult && (
                        <button
                            className="upload-button"
                            onClick={() => onSavePrediction(predictionResult, { targetColumn, selectedFeatures, modelType, inputValues })}
                            style={{ padding: "8px 16px", fontSize: "13px" }}
                        >
                            💾 Bu Tahmini Kaydet
                        </button>
                    )}
                </div>
            </div>

            {errorMsg && (
                <div style={{ padding: "12px 16px", borderRadius: "10px", background: "rgba(239, 68, 68, 0.15)", color: "#ef4444", fontSize: "13px", fontWeight: 600 }}>
                    ⚠️ {errorMsg}
                </div>
            )}

            {/* CONFIGURATION & MODEL TRAINING PANEL */}
            <div className="panel">
                <div className="panel-header">
                    <h4 className="desc-stat-column-title" style={{ fontSize: "16px", margin: 0 }}>
                        ⚙️ Model & Değişken Yapılandırması
                    </h4>
                </div>

                <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "24px", marginTop: "16px" }}>
                    {/* Target Column & Model Type */}
                    <div style={{ display: "flex", flexDirection: "column", gap: "16px" }}>
                        <div>
                            <label className="desc-stat-lbl">🎯 Hedef Sütun (Tahmin Edilecek Değer):</label>
                            <select
                                className="chart-select"
                                value={targetColumn}
                                onChange={(e) => {
                                    setTargetColumn(e.target.value);
                                    setSelectedFeatures(selectedFeatures.filter((f) => f !== e.target.value));
                                }}
                                style={{ width: "100%", padding: "10px 14px", fontWeight: 700 }}
                            >
                                {numericCols.map((c) => (
                                    <option key={c} value={c}>
                                        {c} (Sayısal)
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div>
                            <label className="desc-stat-lbl">🤖 Yapay Zeka / ML Algoritması:</label>
                            <select
                                className="chart-select"
                                value={modelType}
                                onChange={(e) => setModelType(e.target.value)}
                                style={{ width: "100%", padding: "10px 14px" }}
                            >
                                <option value="Auto">⚡ InsightHub AutoML (En İyi Modeli Otomatik Seç)</option>
                                <option value="FastTree">🌲 ML.NET FastTree Regresyon Motoru</option>
                                <option value="SdcaRegression">📈 ML.NET SDCA Doğrusal Regresyon</option>
                            </select>
                        </div>
                    </div>

                    {/* Features checklist */}
                    <div>
                        <label className="desc-stat-lbl">📊 Girdi Özellikleri (Features - Modele Beslenecek Sütunlar):</label>
                        <div style={{ display: "flex", flexDirection: "column", gap: "8px", maxHeight: "150px", overflowY: "auto", padding: "10px", borderRadius: "10px", background: "var(--bg-card, #ffffff)", border: "1.5px solid rgba(236, 72, 153, 0.3)", marginTop: "4px" }}>
                            {numericCols
                                .filter((c) => c !== targetColumn)
                                .map((col) => (
                                    <label key={col} style={{ display: "flex", alignItems: "center", gap: "10px", fontSize: "13px", fontWeight: 600, color: "var(--text-main, #371b2d)", cursor: "pointer" }}>
                                        <input
                                            type="checkbox"
                                            checked={selectedFeatures.includes(col)}
                                            onChange={() => handleFeatureToggle(col)}
                                            style={{ accentColor: "#ec4899", width: "16px", height: "16px", cursor: "pointer" }}
                                        />
                                        <span>{col}</span>
                                    </label>
                                ))}
                        </div>
                    </div>
                </div>

                {/* Train Button */}
                <div style={{ marginTop: "20px", display: "flex", justifyContent: "flex-end" }}>
                    <button
                        className="upload-button"
                        onClick={handleTrainAndPredict}
                        disabled={isTraining}
                        style={{ padding: "12px 24px", fontSize: "14px" }}
                    >
                        {isTraining ? "Model Eğitiliyor..." : "🚀 Modeli Eğit & Tahmin Başlat"}
                    </button>
                </div>
            </div>

            {/* RESULTS & INTERACTIVE PREDICTION INPUTS */}
            {predictionResult && (
                <div style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
                    {/* KPI CARDS */}
                    <div className="stats-grid">
                        <div className="stat-card" style={{ background: "rgba(236, 72, 153, 0.12)", borderColor: "rgba(236, 72, 153, 0.4)", display: "flex", flexDirection: "column", alignItems: "flex-start", justifyContent: "center" }}>
                            <span className="stat-title" style={{ color: "#be185d", fontWeight: 700 }}>🔮 Tahmin Edilen {predictionResult.targetColumn}</span>
                            <div className="stat-value" style={{ color: "#ec4899", fontSize: "32px", fontWeight: 800, margin: "6px 0" }}>
                                {predictionResult.predictedValue.toLocaleString("tr-TR")}
                            </div>
                            <div className="stat-subtitle" style={{ fontWeight: 600, color: "var(--text-muted)" }}>{predictionResult.modelName}</div>
                        </div>

                        <div className="stat-card" style={{ display: "flex", flexDirection: "column", alignItems: "flex-start", justifyContent: "center" }}>
                            <span className="stat-title" style={{ color: "#be185d", fontWeight: 700 }}>🎯 Model Güven Skoru ($R^2$)</span>
                            <div className="stat-value" style={{ fontSize: "32px", fontWeight: 800, margin: "6px 0", color: "var(--text-main)" }}>%{Math.round(predictionResult.r2Score * 100)}</div>
                            <div className="stat-subtitle" style={{ fontWeight: 600, color: "var(--text-muted)" }}>Açıklanan Varyans Oranı</div>
                        </div>

                        <div className="stat-card" style={{ display: "flex", flexDirection: "column", alignItems: "flex-start", justifyContent: "center" }}>
                            <span className="stat-title" style={{ color: "#be185d", fontWeight: 700 }}>📉 Ortalama Hata (MAE / RMSE)</span>
                            <div className="stat-value" style={{ fontSize: "28px", fontWeight: 800, margin: "6px 0", color: "var(--text-main)" }}>{predictionResult.meanAbsoluteError} / {predictionResult.rootMeanSquaredError}</div>
                            <div className="stat-subtitle" style={{ fontWeight: 600, color: "var(--text-muted)" }}>Model Hata Sapması</div>
                        </div>
                    </div>

                    {/* INTERACTIVE INPUT FORM TO TEST DIFFERENT VALUES */}
                    <div className="panel">
                        <div className="panel-header">
                            <div>
                                <h4 className="desc-stat-column-title" style={{ fontSize: "16px", margin: 0 }}>
                                    🎛️ Test Değerlerini Değiştirip Canlı Tahmin Alın
                                </h4>
                                <p className="desc-stat-desc" style={{ marginTop: "4px" }}>
                                    Girdi sütunları için farklı değerler girerek anında yeni tahmin sonucunu hesaplayın
                                </p>
                            </div>
                        </div>

                        <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(220px, 1fr))", gap: "16px", marginTop: "16px" }}>
                            {predictionResult.featureColumns.map((feat) => (
                                <div key={feat}>
                                    <label className="desc-stat-lbl" style={{ fontWeight: 700, color: "var(--text-main)" }}>{feat}:</label>
                                    <input
                                        type="number"
                                        step="any"
                                        className="input-field"
                                        value={inputValues[feat] !== undefined ? inputValues[feat] : 0}
                                        onChange={(e) => setInputValues({ ...inputValues, [feat]: parseFloat(e.target.value) || 0 })}
                                        style={{ width: "100%", padding: "10px 14px", fontWeight: 700, fontSize: "14px" }}
                                    />
                                </div>
                            ))}
                        </div>

                        <div style={{ marginTop: "16px", display: "flex", justifyContent: "flex-end" }}>
                            <button
                                className="upload-button"
                                onClick={handleTrainAndPredict}
                                disabled={isTraining}
                                style={{ padding: "10px 20px", fontSize: "13px" }}
                            >
                                🔄 Yeni Değerlerle Tahmini Güncelle
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};
