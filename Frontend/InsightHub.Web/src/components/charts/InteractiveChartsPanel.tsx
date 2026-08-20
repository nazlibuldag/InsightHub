import React, { useState, useEffect } from "react";
import type { BarChartItem, PieChartItem, LineChartItem, ScatterChartItem, DatasetColumn } from "../../types";
import * as api from "../../services/api";
import {
    BarChart,
    Bar,
    PieChart,
    Pie,
    LineChart,
    Line,
    ScatterChart,
    Scatter,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
    Cell
} from "recharts";

const PIE_COLORS = ["#ec4899", "#8b5cf6", "#06b6d4", "#10b981", "#f59e0b", "#f43f5e", "#a855f7", "#3b82f6", "#14b8a6", "#fb923c"];

interface InteractiveChartsPanelProps {
    datasetId: string;
    columns: DatasetColumn[];
    token: string | null;
}

export const InteractiveChartsPanel: React.FC<InteractiveChartsPanelProps> = ({
    datasetId,
    columns,
    token
}) => {
    const [barData, setBarData] = useState<BarChartItem[]>([]);
    const [pieData, setPieData] = useState<PieChartItem[]>([]);
    const [lineData, setLineData] = useState<LineChartItem[]>([]);
    const [scatterData, setScatterData] = useState<ScatterChartItem[]>([]);

    const numericCols = columns.filter((c) => c.dataType === 1).map((c) => c.columnName);
    const stringCols = columns.filter((c) => c.dataType === 2).map((c) => c.columnName);

    const [selectedPieCol, setSelectedPieCol] = useState<string>("");
    const [selectedLineCol, setSelectedLineCol] = useState<string>("");
    const [selectedScatterX, setSelectedScatterX] = useState<string>("");
    const [selectedScatterY, setSelectedScatterY] = useState<string>("");

    const [loadingBar, setLoadingBar] = useState<boolean>(false);
    const [loadingPie, setLoadingPie] = useState<boolean>(false);
    const [loadingLine, setLoadingLine] = useState<boolean>(false);
    const [loadingScatter, setLoadingScatter] = useState<boolean>(false);

    // Default dropdown selections when columns change
    useEffect(() => {
        if (stringCols.length > 0 && !selectedPieCol) {
            setSelectedPieCol(stringCols[0]);
        }
        if (numericCols.length > 0) {
            if (!selectedLineCol) setSelectedLineCol(numericCols[0]);
            if (!selectedScatterX) setSelectedScatterX(numericCols[0]);
            if (!selectedScatterY) setSelectedScatterY(numericCols.length > 1 ? numericCols[1] : numericCols[0]);
        }
    }, [columns]);

    // Bar Chart
    useEffect(() => {
        if (!datasetId) return;
        setLoadingBar(true);
        api.fetchBarChartData(datasetId, token)
            .then(setBarData)
            .catch(console.error)
            .finally(() => setLoadingBar(false));
    }, [datasetId, token]);

    // Pie Chart
    useEffect(() => {
        if (!datasetId) return;
        setLoadingPie(true);
        api.fetchPieChartData(datasetId, selectedPieCol, token)
            .then(setPieData)
            .catch(console.error)
            .finally(() => setLoadingPie(false));
    }, [datasetId, selectedPieCol, token]);

    // Line Chart
    useEffect(() => {
        if (!datasetId) return;
        setLoadingLine(true);
        api.fetchLineChartData(datasetId, selectedLineCol, token)
            .then(setLineData)
            .catch(console.error)
            .finally(() => setLoadingLine(false));
    }, [datasetId, selectedLineCol, token]);

    // Scatter Chart
    useEffect(() => {
        if (!datasetId) return;
        setLoadingScatter(true);
        api.fetchScatterChartData(datasetId, selectedScatterX, selectedScatterY, token)
            .then(setScatterData)
            .catch(console.error)
            .finally(() => setLoadingScatter(false));
    }, [datasetId, selectedScatterX, selectedScatterY, token]);

    return (
        <div style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
            {/* Row 1: Bar Chart & Pie Chart */}
            <div className="content-grid">
                <div className="panel">
                    <div className="panel-header">
                        <div>
                            <h3>📊 Sayısal Sütun Ortalamaları (Bar Chart)</h3>
                            <p>Sayısal kolonların ortalama (mean) değer karşılaştırması</p>
                        </div>
                    </div>
                    {loadingBar ? (
                        <p style={{ color: "var(--text-muted)" }}>Grafik yükleniyor...</p>
                    ) : barData.length === 0 ? (
                        <p style={{ color: "var(--text-muted)" }}>Sayısal veri bulunamadı.</p>
                    ) : (
                        <div style={{ width: "100%", height: 300 }}>
                            <ResponsiveContainer>
                                <BarChart data={barData}>
                                    <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" />
                                    <XAxis dataKey="columnName" stroke="#94a3b8" tick={{ fontSize: 12 }} />
                                    <YAxis stroke="#94a3b8" />
                                    <Tooltip
                                        contentStyle={{
                                            backgroundColor: "rgba(15, 23, 42, 0.95)",
                                            borderRadius: "10px",
                                            border: "1px solid rgba(255,255,255,0.1)"
                                        }}
                                    />
                                    <Bar dataKey="average" name="Ortalama Değer" radius={[8, 8, 0, 0]}>
                                        {barData.map((_, index) => (
                                            <Cell key={`cell-${index}`} fill={PIE_COLORS[index % PIE_COLORS.length]} />
                                        ))}
                                    </Bar>
                                </BarChart>
                            </ResponsiveContainer>
                        </div>
                    )}
                </div>

                <div className="panel">
                    <div className="panel-header">
                        <div>
                            <h3>🍩 Kategorik Dağılım (Pie Chart)</h3>
                            <p>Metin türündeki sütunların frekans pastası</p>
                        </div>
                        {stringCols.length > 0 && (
                            <select
                                className="chart-select"
                                value={selectedPieCol}
                                onChange={(e) => setSelectedPieCol(e.target.value)}
                            >
                                {stringCols.map((c) => (
                                    <option key={c} value={c}>{c}</option>
                                ))}
                            </select>
                        )}
                    </div>
                    {loadingPie ? (
                        <p style={{ color: "var(--text-muted)" }}>Grafik yükleniyor...</p>
                    ) : pieData.length === 0 ? (
                        <p style={{ color: "var(--text-muted)" }}>Kategorik veri bulunamadı.</p>
                    ) : (
                        <div style={{ width: "100%", height: 300 }}>
                            <ResponsiveContainer>
                                <PieChart>
                                    <Pie
                                        data={pieData}
                                        dataKey="count"
                                        nameKey="label"
                                        cx="50%"
                                        cy="50%"
                                        outerRadius={90}
                                        innerRadius={45}
                                        label={(entry: any) => `${entry.label || entry.name}: ${entry.count || entry.value}`}
                                    >
                                        {pieData.map((_, index) => (
                                            <Cell key={`pie-cell-${index}`} fill={PIE_COLORS[index % PIE_COLORS.length]} />
                                        ))}
                                    </Pie>
                                    <Tooltip />
                                </PieChart>
                            </ResponsiveContainer>
                        </div>
                    )}
                </div>
            </div>

            {/* Row 2: Line Chart & Scatter Chart */}
            <div className="content-grid">
                <div className="panel">
                    <div className="panel-header">
                        <div>
                            <h3>📈 Satır Bazlı Trend Çizgisi (Line Chart)</h3>
                            <p>Satır sıra numarasına göre sayısal değer seyri</p>
                        </div>
                        {numericCols.length > 0 && (
                            <select
                                className="chart-select"
                                value={selectedLineCol}
                                onChange={(e) => setSelectedLineCol(e.target.value)}
                            >
                                {numericCols.map((c) => (
                                    <option key={c} value={c}>{c}</option>
                                ))}
                            </select>
                        )}
                    </div>
                    {loadingLine ? (
                        <p style={{ color: "var(--text-muted)" }}>Grafik yükleniyor...</p>
                    ) : lineData.length === 0 ? (
                        <p style={{ color: "var(--text-muted)" }}>Trend verisi bulunamadı.</p>
                    ) : (
                        <div style={{ width: "100%", height: 300 }}>
                            <ResponsiveContainer>
                                <LineChart data={lineData}>
                                    <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" />
                                    <XAxis dataKey="rowNumber" stroke="#94a3b8" label={{ value: 'Satır No', position: 'insideBottomRight', offset: -5 }} />
                                    <YAxis stroke="#94a3b8" />
                                    <Tooltip />
                                    <Line type="monotone" dataKey="value" stroke="#8b5cf6" strokeWidth={3} dot={{ r: 4 }} />
                                </LineChart>
                            </ResponsiveContainer>
                        </div>
                    )}
                </div>

                <div className="panel">
                    <div className="panel-header">
                        <div>
                            <h3>🎯 İki Değişkenli Saçılım (Scatter Plot)</h3>
                            <p>İki sayısal değişken arasındaki korelasyon dağılımı</p>
                        </div>
                        {numericCols.length > 0 && (
                            <div style={{ display: "flex", gap: "8px" }}>
                                <select
                                    className="chart-select"
                                    value={selectedScatterX}
                                    onChange={(e) => setSelectedScatterX(e.target.value)}
                                >
                                    {numericCols.map((c) => (
                                        <option key={c} value={c}>X: {c}</option>
                                    ))}
                                </select>
                                <select
                                    className="chart-select"
                                    value={selectedScatterY}
                                    onChange={(e) => setSelectedScatterY(e.target.value)}
                                >
                                    {numericCols.map((c) => (
                                        <option key={c} value={c}>Y: {c}</option>
                                    ))}
                                </select>
                            </div>
                        )}
                    </div>
                    {loadingScatter ? (
                        <p style={{ color: "var(--text-muted)" }}>Grafik yükleniyor...</p>
                    ) : scatterData.length === 0 ? (
                        <p style={{ color: "var(--text-muted)" }}>Saçılım verisi bulunamadı.</p>
                    ) : (
                        <div style={{ width: "100%", height: 300 }}>
                            <ResponsiveContainer>
                                <ScatterChart>
                                    <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" />
                                    <XAxis type="number" dataKey="x" name="X Ekseni" stroke="#94a3b8" />
                                    <YAxis type="number" dataKey="y" name="Y Ekseni" stroke="#94a3b8" />
                                    <Tooltip cursor={{ strokeDasharray: '3 3' }} />
                                    <Scatter name="Veri Noktası" data={scatterData} fill="#ec4899" />
                                </ScatterChart>
                            </ResponsiveContainer>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};
