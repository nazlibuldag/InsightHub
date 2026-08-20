import React, { useState, useEffect } from "react";
import type { DatasetForecastResponse, ColumnForecastDto } from "../../types";
import * as api from "../../services/api";
import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer
} from "recharts";

interface MlForecastPanelProps {
    datasetId: string;
    token: string | null;
    onSaveForecast?: (activeForecast: ColumnForecastDto, stepsAhead: number) => void;
}

export const MlForecastPanel: React.FC<MlForecastPanelProps> = ({ datasetId, token, onSaveForecast }) => {
    const [stepsAhead, setStepsAhead] = useState<number>(10);
    const [forecastData, setForecastData] = useState<DatasetForecastResponse | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [selectedColumn, setSelectedColumn] = useState<string>("");

    const loadForecast = async () => {
        if (!datasetId) return;
        setIsLoading(true);
        try {
            const data = await api.fetchDatasetForecast(datasetId, stepsAhead, token);
            setForecastData(data);
            if (data.columnForecasts.length > 0 && !selectedColumn) {
                setSelectedColumn(data.columnForecasts[0].targetColumn || data.columnForecasts[0].columnName || "");
            }
        } catch (err) {
            console.error("Forecast fetch error", err);
            setForecastData(null);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        loadForecast();
    }, [datasetId, stepsAhead, token]);

    const activeForecast: ColumnForecastDto | undefined = forecastData?.columnForecasts.find(
        (f) => (f.targetColumn || f.columnName) === selectedColumn
    ) || forecastData?.columnForecasts[0];

    // Combine Historical and Forecasted values into a single time series chart dataset
    const chartData: { stepLabel: string; historical?: number; forecast?: number; isForecast: boolean }[] = [];

    if (activeForecast) {
        const hist = activeForecast.historicalValues || [];
        const fc = activeForecast.forecastValues || [];
        const fcArray = activeForecast.forecastedValues || fc.map((f) => f.predictedValue);

        // Historical points
        hist.forEach((val, idx) => {
            chartData.push({
                stepLabel: `Satır #${idx + 1}`,
                historical: val,
                isForecast: false
            });
        });

        // Connect last historical point to first forecast point for seamless line
        if (hist.length > 0) {
            chartData[chartData.length - 1].forecast = hist[hist.length - 1];
        }

        // Forecast future points
        fcArray.forEach((val, idx) => {
            chartData.push({
                stepLabel: `Gelecek +${idx + 1}`,
                forecast: val,
                isForecast: true
            });
        });
    }

    const lastHistorical = activeForecast?.historicalValues?.[activeForecast.historicalValues.length - 1] ?? 0;

    return (
        <div style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
            {/* Header Control Bar */}
            <div className="panel">
                <div className="panel-header" style={{ flexWrap: "wrap", gap: "12px" }}>
                    <div>
                        <h3 className="desc-stat-column-title">🤖 ML.NET Zaman Serisi Tahminleme & Trend Analitiği</h3>
                        <p className="desc-stat-desc">Lineer Regresyon ve Makine Öğrenmesi modelleri ile gelecek N adım kestirimi</p>
                    </div>

                    <div style={{ display: "flex", gap: "12px", alignItems: "center", flexWrap: "wrap" }}>
                        <label className="desc-stat-lbl" style={{ fontSize: "13px" }}>⚡ Tahmin Adımı Seçin:</label>
                        <select
                            className="chart-select"
                            value={stepsAhead}
                            onChange={(e) => setStepsAhead(Number(e.target.value))}
                        >
                            <option value={5}>+5 Adım Gelecek</option>
                            <option value={10}>+10 Adım Gelecek</option>
                            <option value={15}>+15 Adım Gelecek</option>
                            <option value={20}>+20 Adım Gelecek</option>
                            <option value={30}>+30 Adım Gelecek</option>
                        </select>

                        {activeForecast && onSaveForecast && (
                            <button
                                className="upload-button"
                                onClick={() => onSaveForecast(activeForecast, stepsAhead)}
                                style={{ padding: "8px 16px", fontSize: "13px" }}
                            >
                                💾 Bu Tahmini Kaydet
                            </button>
                        )}
                    </div>
                </div>
            </div>

            {/* Main Interactive Forecast Dashboard */}
            {isLoading ? (
                <div className="panel"><p style={{ color: "var(--text-muted)", padding: "20px 0" }}>ML Modeli çalıştırılıyor ve gelecek adımlar hesaplanıyor...</p></div>
            ) : !activeForecast ? (
                <div className="panel"><p style={{ color: "var(--text-muted)", padding: "20px 0" }}>Tahmin verisi bulunamadı.</p></div>
            ) : (
                <>
                    {/* Column Picker Bar & Model Indicators */}
                    <div className="panel">
                        <div className="panel-header">
                            <div style={{ display: "flex", alignItems: "center", gap: "12px" }}>
                                <label className="desc-stat-column-title" style={{ fontSize: "14px" }}>İncelenen Sütun:</label>
                                <select
                                    className="chart-select"
                                    value={selectedColumn}
                                    onChange={(e) => setSelectedColumn(e.target.value)}
                                    style={{ fontSize: "14px", fontWeight: 700 }}
                                >
                                    {forecastData?.columnForecasts.map((f) => {
                                        const colName = f.targetColumn || f.columnName || "";
                                        return (
                                            <option key={colName} value={colName}>
                                                {colName} ({f.trendDirection})
                                            </option>
                                        );
                                    })}
                                </select>
                            </div>

                            <span
                                style={{
                                    fontSize: "12px",
                                    fontWeight: 700,
                                    padding: "6px 14px",
                                    borderRadius: "20px",
                                    background: activeForecast.slope > 0 ? "rgba(52, 211, 153, 0.2)" : "rgba(239, 68, 68, 0.2)",
                                    color: activeForecast.slope > 0 ? "#34d399" : "#f87171",
                                    border: `1px solid ${activeForecast.slope > 0 ? "rgba(52, 211, 153, 0.4)" : "rgba(239, 68, 68, 0.4)"}`
                                }}
                            >
                                {activeForecast.trendDirection}
                            </span>
                        </div>

                        {/* ML Model Parameter Cards */}
                        <div className="outlier-summary-grid" style={{ margin: "20px 0" }}>
                            <div className="outlier-box">
                                <span>Model Güven Skoru (R²)</span>
                                <strong style={{ color: "#ec4899" }}>%{ (activeForecast.rSquared * 100).toFixed(1) }</strong>
                            </div>
                            <div className="outlier-box">
                                <span>Regresyon Eğimi (Slope - β₁)</span>
                                <strong style={{ color: activeForecast.slope >= 0 ? "#34d399" : "#f87171" }}>
                                    {activeForecast.slope > 0 ? `+${activeForecast.slope.toFixed(4)}` : activeForecast.slope.toFixed(4)}
                                </strong>
                            </div>
                            <div className="outlier-box">
                                <span>Kesişim Değeri (Intercept - β₀)</span>
                                <strong>{activeForecast.intercept.toFixed(4)}</strong>
                            </div>
                            <div className="outlier-box">
                                <span>Son Geçmiş Değeri</span>
                                <strong>{lastHistorical.toFixed(2)}</strong>
                            </div>
                            <div className="outlier-box red-badge">
                                <span>+{stepsAhead}. Adım Tahmini</span>
                                <strong>
                                    {(activeForecast.forecastedValues?.[activeForecast.forecastedValues.length - 1] ??
                                        activeForecast.forecastValues?.[activeForecast.forecastValues.length - 1]?.predictedValue ??
                                        0).toFixed(2)}
                                </strong>
                            </div>
                        </div>

                        {/* Line Chart: Historical vs Future ML Forecast */}
                        <div style={{ marginTop: "24px" }}>
                            <h4 className="desc-stat-column-title" style={{ marginBottom: "12px", display: "flex", gap: "16px", alignItems: "center", flexWrap: "wrap" }}>
                                <span>📈 Zaman Serisi & ML Gelecek Kestirim Grafiği:</span>
                                <span style={{ fontSize: "12px", color: "#818cf8", fontWeight: 600 }}>🟣 Mor: Geçmiş Satırlar</span>
                                <span style={{ fontSize: "12px", color: "#ec4899", fontWeight: 600 }}>💖 Pembe Kesikli: ML Tahmini (+{stepsAhead} Adım)</span>
                            </h4>

                            <div style={{ width: "100%", height: 360 }}>
                                <ResponsiveContainer>
                                    <LineChart data={chartData} margin={{ top: 20, right: 30, left: 10, bottom: 20 }}>
                                        <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" />
                                        <XAxis dataKey="stepLabel" stroke="#94a3b8" tick={{ fontSize: 11 }} />
                                        <YAxis stroke="#94a3b8" />
                                        <Tooltip
                                            content={({ active, payload }) => {
                                                if (active && payload && payload.length) {
                                                    const pt = payload[0].payload;
                                                    const val = pt.isForecast ? pt.forecast : pt.historical;
                                                    return (
                                                        <div
                                                            style={{
                                                                backgroundColor: "rgba(15, 23, 42, 0.95)",
                                                                padding: "12px 16px",
                                                                borderRadius: "10px",
                                                                border: `1px solid ${pt.isForecast ? "#ec4899" : "#8b5cf6"}`,
                                                                color: "#fff"
                                                            }}
                                                        >
                                                            <strong style={{ color: pt.isForecast ? "#ec4899" : "#8b5cf6" }}>
                                                                {pt.stepLabel} {pt.isForecast ? "(ML Gelecek Tahmini)" : "(Geçmiş Veri)"}
                                                            </strong>
                                                            <div style={{ fontSize: "14px", fontWeight: 700, marginTop: "4px" }}>
                                                                Değer: {val !== undefined ? val.toFixed(2) : "-"}
                                                            </div>
                                                        </div>
                                                    );
                                                }
                                                return null;
                                            }}
                                        />
                                        {/* Historical Line */}
                                        <Line
                                            type="monotone"
                                            dataKey="historical"
                                            name="Geçmiş Veri"
                                            stroke="#8b5cf6"
                                            strokeWidth={3}
                                            dot={{ r: 3 }}
                                            connectNulls
                                        />
                                        {/* Forecast Line */}
                                        <Line
                                            type="monotone"
                                            dataKey="forecast"
                                            name="ML Tahmini"
                                            stroke="#ec4899"
                                            strokeWidth={3}
                                            strokeDasharray="5 5"
                                            dot={{ r: 5, fill: "#ec4899" }}
                                            connectNulls
                                        />
                                    </LineChart>
                                </ResponsiveContainer>
                            </div>
                        </div>
                    </div>

                    {/* Predicted Steps Table */}
                    <div className="panel">
                        <div className="panel-header">
                            <div>
                                <h3 className="desc-stat-column-title">📋 Adım Adım Gelecek Tahmin Tablosu (+{stepsAhead} Adım)</h3>
                                <p className="desc-stat-desc">Regresyon denklemi y = {activeForecast.slope.toFixed(4)} * x + {activeForecast.intercept.toFixed(4)} ile hesaplanan adımlar</p>
                            </div>
                        </div>

                        <div className="forecast-table-container">
                            <table className="heatmap-table" style={{ width: "100%", borderCollapse: "collapse" }}>
                                <thead className="forecast-table-head">
                                    <tr>
                                        <th style={{ textAlign: "left", padding: "14px 16px", width: "22%" }}>Tahmin Adımı</th>
                                        <th style={{ textAlign: "left", padding: "14px 16px", width: "25%" }}>Tahmin Edilen Değer</th>
                                        <th style={{ textAlign: "left", padding: "14px 16px", width: "33%" }}>Geçmiş Son Değere Göre Değişim</th>
                                        <th style={{ textAlign: "left", padding: "14px 16px", width: "20%" }}>Eğilim Durumu</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {(activeForecast.forecastValues || activeForecast.forecastedValues?.map((val, idx) => ({ stepIndex: idx + 1, predictedValue: val })) || []).map((pt, idx) => {
                                        const predVal = pt.predictedValue;
                                        const diff = predVal - lastHistorical;
                                        const percent = lastHistorical !== 0 ? (diff / lastHistorical) * 100 : 0;
                                        const isPositive = diff >= 0;

                                        return (
                                            <tr key={idx}>
                                                <td style={{ textAlign: "left", fontWeight: 700, padding: "12px 16px" }}>Adım +{pt.stepIndex}</td>
                                                <td style={{ textAlign: "left", fontWeight: 800, padding: "12px 16px", fontSize: "15px" }}>
                                                    <span className="desc-stat-val" style={{ fontSize: "15px" }}>{predVal.toFixed(2)}</span>
                                                </td>
                                                <td style={{ textAlign: "left", padding: "12px 16px", fontWeight: 700, color: isPositive ? "#059669" : "#dc2626" }}>
                                                    {isPositive ? `+${diff.toFixed(2)} (+${percent.toFixed(1)}%)` : `${diff.toFixed(2)} (${percent.toFixed(1)}%)`}
                                                </td>
                                                <td style={{ textAlign: "left", padding: "12px 16px" }}>
                                                    <span
                                                        style={{
                                                            padding: "4px 10px",
                                                            borderRadius: "8px",
                                                            fontSize: "11px",
                                                            fontWeight: 700,
                                                            background: isPositive ? "rgba(52, 211, 153, 0.15)" : "rgba(239, 68, 68, 0.15)",
                                                            color: isPositive ? "#059669" : "#dc2626",
                                                            border: `1px solid ${isPositive ? "rgba(52, 211, 153, 0.3)" : "rgba(239, 68, 68, 0.3)"}`
                                                        }}
                                                    >
                                                        {isPositive ? "↗️ Yükseliş" : "↘️ Düşüş"}
                                                    </span>
                                                </td>
                                            </tr>
                                        );
                                    })}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </>
            )}
        </div>
    );
};
