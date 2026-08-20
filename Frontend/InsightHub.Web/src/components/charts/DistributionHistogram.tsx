import React, { useState, useEffect } from "react";
import type { DistributionResponse } from "../../types";
import * as api from "../../services/api";
import {
    BarChart,
    Bar,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
    Cell,
    LabelList
} from "recharts";

interface DistributionHistogramProps {
    datasetId: string;
    numericColumns: string[];
    token: string | null;
}

export const DistributionHistogram: React.FC<DistributionHistogramProps> = ({ datasetId, numericColumns, token }) => {
    const [selectedCol, setSelectedCol] = useState<string>("");
    const [distribution, setDistribution] = useState<DistributionResponse | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(false);

    useEffect(() => {
        if (numericColumns.length > 0 && !selectedCol) {
            setSelectedCol(numericColumns[0]);
        }
    }, [numericColumns]);

    useEffect(() => {
        if (!datasetId || !selectedCol) return;
        setIsLoading(true);
        api.fetchDistribution(datasetId, selectedCol, 10, token)
            .then(setDistribution)
            .catch((err) => {
                console.error("Distribution fetch error", err);
                setDistribution(null);
            })
            .finally(() => setIsLoading(false));
    }, [datasetId, selectedCol, token]);

    if (numericColumns.length === 0) {
        return (
            <div className="panel">
                <p style={{ color: "var(--text-muted)" }}>Histogram için sayısal sütun bulunamadı.</p>
            </div>
        );
    }

    const totalCount = distribution ? distribution.bins.reduce((sum, b) => sum + b.count, 0) : 0;

    const chartData = distribution
        ? distribution.bins.map((b, idx) => ({
              binName: `Bin ${idx + 1}`,
              range: `${b.from.toFixed(2)} - ${b.to.toFixed(2)}`,
              actualCount: b.count,
              barHeight: b.count === 0 ? 0.15 : b.count,
              percentage: totalCount > 0 ? ((b.count / totalCount) * 100).toFixed(1) : "0"
          }))
        : [];

    return (
        <div className="panel">
            <div className="panel-header">
                <div>
                    <h3>🔔 Frekans Dağılımı ve Histogram (10 Eşit Kutu / Bin)</h3>
                    <p>Sayısal değişkenlerin 10 eşit aralıktaki veri yoğunluğu ve frekans dağılımı</p>
                </div>
                <div className="chart-select-wrapper">
                    <label style={{ fontSize: "13px", color: "var(--text-muted)", fontWeight: 600 }}>Sütun Seçin:</label>
                    <select
                        className="chart-select"
                        value={selectedCol}
                        onChange={(e) => setSelectedCol(e.target.value)}
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
                <p style={{ color: "var(--text-muted)", padding: "20px 0" }}>Dağılım analizleri yükleniyor...</p>
            ) : !distribution ? (
                <p style={{ color: "var(--text-muted)", padding: "20px 0" }}>Seçilen sütun için dağılım verisi oluşturulamadı.</p>
            ) : (
                <>
                    <div style={{ width: "100%", height: 380, marginTop: "1rem" }}>
                        <ResponsiveContainer>
                            <BarChart data={chartData} margin={{ top: 35, right: 20, left: 10, bottom: 25 }}>
                                <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" />
                                <XAxis
                                    dataKey="range"
                                    stroke="#94a3b8"
                                    tick={{ fontSize: 11 }}
                                    angle={-15}
                                    textAnchor="end"
                                    interval={0}
                                />
                                <YAxis stroke="#94a3b8" />
                                <Tooltip
                                    content={({ active, payload }) => {
                                        if (active && payload && payload.length) {
                                            const data = payload[0].payload;
                                            return (
                                                <div
                                                    style={{
                                                        backgroundColor: "rgba(15, 23, 42, 0.95)",
                                                        padding: "12px 16px",
                                                        borderRadius: "10px",
                                                        border: "1px solid rgba(236, 72, 153, 0.4)",
                                                        boxShadow: "0 8px 24px rgba(0,0,0,0.3)",
                                                        color: "#fff"
                                                    }}
                                                >
                                                    <strong style={{ color: "#ec4899", display: "block", marginBottom: "4px" }}>
                                                        {data.binName} ({data.range})
                                                    </strong>
                                                    <div>Frekans: <strong>{data.actualCount} satır</strong></div>
                                                    <div>Oran: <strong>%{data.percentage}</strong></div>
                                                </div>
                                            );
                                        }
                                        return null;
                                    }}
                                />
                                <Bar dataKey="barHeight" name="Frekans (Satır Sayısı)" radius={[8, 8, 0, 0]}>
                                    <LabelList
                                        dataKey="actualCount"
                                        position="top"
                                        fill="#ec4899"
                                        fontSize={13}
                                        fontWeight={800}
                                        formatter={(val: any) => `${val}`}
                                    />
                                    {chartData.map((entry, index) => (
                                        <Cell
                                            key={`cell-${index}`}
                                            fill={entry.actualCount > 0 ? `hsl(${270 + index * 14}, 85%, 60%)` : "rgba(236, 72, 153, 0.15)"}
                                            stroke={entry.actualCount === 0 ? "#ec4899" : undefined}
                                            strokeDasharray={entry.actualCount === 0 ? "3 3" : undefined}
                                        />
                                    ))}
                                </Bar>
                            </BarChart>
                        </ResponsiveContainer>
                    </div>

                    <div className="hist-summary-grid">
                        <div>
                            <div className="hist-summary-lbl">Minimum Değer:</div>
                            <div className="hist-summary-val">{distribution.minValue.toFixed(2)}</div>
                        </div>
                        <div>
                            <div className="hist-summary-lbl">Maksimum Değer:</div>
                            <div className="hist-summary-val">{distribution.maxValue.toFixed(2)}</div>
                        </div>
                        <div>
                            <div className="hist-summary-lbl">Toplam Veri Sayısı:</div>
                            <div className="hist-summary-val-indigo">{totalCount} satır</div>
                        </div>
                        <div>
                            <div className="hist-summary-lbl">Dolu Kutu Sayısı:</div>
                            <div className="hist-summary-val-emerald">
                                {chartData.filter((c) => c.actualCount > 0).length} / 10 kutu
                            </div>
                        </div>
                    </div>
                </>
            )}
        </div>
    );
};
