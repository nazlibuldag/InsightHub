import React, { useState, useEffect } from "react";
import type { OutlierDetailResponse } from "../../types";
import * as api from "../../services/api";

interface OutliersChartProps {
    datasetId: string;
    numericColumns: string[];
    token: string | null;
}

export const OutliersChart: React.FC<OutliersChartProps> = ({ datasetId, numericColumns, token }) => {
    const [selectedColumn, setSelectedColumn] = useState<string>("");
    const [outlierData, setOutlierData] = useState<OutlierDetailResponse | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(false);

    useEffect(() => {
        if (numericColumns.length > 0 && !selectedColumn) {
            setSelectedColumn(numericColumns[0]);
        }
    }, [numericColumns]);

    useEffect(() => {
        if (!datasetId || !selectedColumn) return;
        setIsLoading(true);
        api.fetchOutlierDetails(datasetId, selectedColumn, token)
            .then(setOutlierData)
            .catch((err) => {
                console.error("Outlier fetch error", err);
                setOutlierData(null);
            })
            .finally(() => setIsLoading(false));
    }, [datasetId, selectedColumn, token]);

    if (numericColumns.length === 0) {
        return <div className="panel"><p style={{ color: "var(--text-muted)" }}>Aykırı değer analizi için sayısal sütun bulunamadı.</p></div>;
    }

    return (
        <div className="panel">
            <div className="panel-header">
                <div>
                    <h3>🎯 IQR Aykırı Değer (Outlier) Tespiti</h3>
                    <p>Çeyrekler arası genişlik (IQR = Q3 - Q1) kullanılarak hesaplanan alt ve üst sınır aykırı değerleri.</p>
                </div>
                <div className="chart-select-wrapper">
                    <label style={{ fontSize: "13px", color: "var(--text-muted)", fontWeight: 600 }}>Sütun Seç:</label>
                    <select
                        className="chart-select"
                        value={selectedColumn}
                        onChange={(e) => setSelectedColumn(e.target.value)}
                    >
                        {numericColumns.map((col) => (
                            <option key={col} value={col}>
                                {col}
                            </option>
                        ))}
                    </select>
                </div>
            </div>

            {isLoading ? (
                <p style={{ color: "var(--text-muted)" }}>Aykırı değerler hesaplanıyor...</p>
            ) : !outlierData ? (
                <p style={{ color: "var(--text-muted)" }}>Bu sütun için aykırı değer hesaplanamadı.</p>
            ) : (
                <>
                    <div className="outlier-summary-grid" style={{ margin: "20px 0" }}>
                        <div className="outlier-box red-badge">
                            <span>Alt Sınır (Q1 - 1.5*IQR)</span>
                            <strong>{outlierData.lowerBound.toFixed(2)}</strong>
                        </div>
                        <div className="outlier-box">
                            <span>Q1 (%25 Çeyrek)</span>
                            <strong>{outlierData.q1.toFixed(2)}</strong>
                        </div>
                        <div className="outlier-box">
                            <span>Q3 (%75 Çeyrek)</span>
                            <strong>{outlierData.q3.toFixed(2)}</strong>
                        </div>
                        <div className="outlier-box red-badge">
                            <span>Üst Sınır (Q3 + 1.5*IQR)</span>
                            <strong>{outlierData.upperBound.toFixed(2)}</strong>
                        </div>
                        <div className="outlier-box red-badge">
                            <span>Toplam Aykırı Satır</span>
                            <strong>{outlierData.outlierCount}</strong>
                        </div>
                    </div>

                    <h4 style={{ color: "var(--text-main)", marginBottom: "12px" }}>Aykırı Satır Listesi:</h4>
                    {outlierData.outliers.length === 0 ? (
                        <div style={{ padding: "16px", background: "rgba(52, 211, 153, 0.1)", border: "1px solid rgba(52, 211, 153, 0.3)", borderRadius: "12px", color: "#34d399" }}>
                            Bu sütunda herhangi bir aykırı değer tespit edilmedi. 🎉
                        </div>
                    ) : (
                        <div style={{ maxHeight: "300px", overflowY: "auto" }}>
                            <table className="heatmap-table">
                                <thead>
                                    <tr>
                                        <th>Satır No</th>
                                        <th>Değer</th>
                                        <th>Aykırılık Türü</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {outlierData.outliers.map((o, idx) => {
                                        const isLower = o.value < outlierData.lowerBound;
                                        return (
                                            <tr key={idx}>
                                                <td style={{ padding: "10px", color: "var(--text-main)" }}>Satır #{o.rowNumber}</td>
                                                <td style={{ padding: "10px", color: "var(--text-main)", fontWeight: 700 }}>{o.value}</td>
                                                <td style={{ padding: "10px" }}>
                                                    <span style={{
                                                        padding: "4px 8px",
                                                        borderRadius: "6px",
                                                        fontSize: "11px",
                                                        fontWeight: 700,
                                                        background: isLower ? "rgba(239, 68, 68, 0.2)" : "rgba(245, 158, 11, 0.2)",
                                                        color: isLower ? "#f87171" : "#fbbf24",
                                                        border: `1px solid ${isLower ? "rgba(239, 68, 68, 0.4)" : "rgba(245, 158, 11, 0.4)"}`
                                                    }}>
                                                        {isLower ? "Alt Aykırı" : "Üst Aykırı"}
                                                    </span>
                                                </td>
                                            </tr>
                                        );
                                    })}
                                </tbody>
                            </table>
                        </div>
                    )}
                </>
            )}
        </div>
    );
};
