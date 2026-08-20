import React, { useState } from "react";
import type { DescriptiveStatsResponse } from "../../types";

interface DescriptiveStatsPanelProps {
    statsList: DescriptiveStatsResponse[];
    isLoading: boolean;
}

export const DescriptiveStatsPanel: React.FC<DescriptiveStatsPanelProps> = ({ statsList, isLoading }) => {
    // Track open/closed state for each column accordion box
    const [openColumns, setOpenColumns] = useState<Record<string, boolean>>(() => {
        // Expand the first column by default
        if (statsList.length > 0) {
            return { [statsList[0].columnName]: true };
        }
        return {};
    });

    const toggleColumn = (colName: string) => {
        setOpenColumns((prev) => ({
            ...prev,
            [colName]: !prev[colName]
        }));
    };

    const expandAll = () => {
        const allOpen: Record<string, boolean> = {};
        statsList.forEach((s) => {
            allOpen[s.columnName] = true;
        });
        setOpenColumns(allOpen);
    };

    const collapseAll = () => {
        setOpenColumns({});
    };

    if (isLoading) {
        return (
            <div className="panel">
                <div className="panel-header">
                    <div>
                        <h3>📊 Tanımlayıcı İstatistikler (12 Metrik)</h3>
                        <p>Tüm sayısal sütunların 12 kapsamlı istatistik parametresi</p>
                    </div>
                </div>
                <p style={{ color: "var(--text-muted)", padding: "20px 0" }}>İstatistikler hesaplanıyor...</p>
            </div>
        );
    }

    if (!statsList || statsList.length === 0) {
        return (
            <div className="panel">
                <div className="panel-header">
                    <div>
                        <h3>📊 Tanımlayıcı İstatistikler (12 Metrik)</h3>
                        <p>Tüm sayısal sütunların 12 kapsamlı istatistik parametresi</p>
                    </div>
                </div>
                <p style={{ color: "var(--text-muted)", padding: "20px 0" }}>Sayısal sütun istatistiği bulunamadı.</p>
            </div>
        );
    }

    return (
        <div className="panel">
            <div className="panel-header">
                <div>
                    <h3>📊 Tanımlayıcı İstatistikler (12 Kapsamlı Metrik)</h3>
                    <p>Aktif veri setindeki tüm sayısal sütunların detaylı açılır kart paneli</p>
                </div>

                <div style={{ display: "flex", gap: "8px" }}>
                    <button
                        onClick={expandAll}
                        className="expand-collapse-btn"
                    >
                        📂 Hepsini Aç
                    </button>
                    <button
                        onClick={collapseAll}
                        className="expand-collapse-btn"
                    >
                        📁 Hepsini Kapat
                    </button>
                </div>
            </div>

            <div style={{ display: "flex", flexDirection: "column", gap: "16px", marginTop: "20px" }}>
                {statsList.map((stat, colIdx) => {
                    const isOpen = !!openColumns[stat.columnName];
                    const metrics = [
                        { icon: "🔢", label: "Veri Adedi (N)", value: stat.count, desc: "Toplam geçerli satır sayısı" },
                        { icon: "📐", label: "Ortalama (Mean)", value: stat.mean !== null ? stat.mean.toFixed(2) : "-", desc: "Aritmetik ortalama" },
                        { icon: "📍", label: "Medyan (Ortanca)", value: stat.median !== null ? stat.median.toFixed(2) : "-", desc: "%50 ortanca değer" },
                        { icon: "🎯", label: "Mod (Tepe Değer)", value: stat.mode !== null ? stat.mode.toFixed(2) : "-", desc: "En sık tekrar eden" },
                        { icon: "⬇️", label: "Minimum (Min)", value: stat.min !== null ? stat.min.toFixed(2) : "-", desc: "En küçük değer" },
                        { icon: "⬆️", label: "Maksimum (Max)", value: stat.max !== null ? stat.max.toFixed(2) : "-", desc: "En büyük değer" },
                        { icon: "↔️", label: "Değişim Aralığı (Range)", value: stat.range !== null ? stat.range.toFixed(2) : "-", desc: "Max - Min farkı" },
                        { icon: "📊", label: "%25 Çeyrek (Q1)", value: stat.q1 !== null ? stat.q1.toFixed(2) : "-", desc: "Alt çeyreklik sınırı" },
                        { icon: "📈", label: "%75 Çeyrek (Q3)", value: stat.q3 !== null ? stat.q3.toFixed(2) : "-", desc: "Üst çeyreklik sınırı" },
                        { icon: "⚡", label: "Çeyrek Genişliği (IQR)", value: stat.iqr !== null ? stat.iqr.toFixed(2) : "-", desc: "Q3 - Q1 genişliği" },
                        { icon: "🌌", label: "Varyans (σ²)", value: stat.variance !== null ? stat.variance.toFixed(2) : "-", desc: "Sapma karesi ortalaması" },
                        { icon: "📏", label: "Standart Sapma (σ)", value: stat.standardDeviation !== null ? stat.standardDeviation.toFixed(2) : "-", desc: "Ortalamadan yayılım" }
                    ];

                    return (
                        <div key={colIdx} className="desc-stat-column-box" style={{ padding: "0", overflow: "hidden" }}>
                            {/* Accordion Header Bar */}
                            <div
                                onClick={() => toggleColumn(stat.columnName)}
                                style={{
                                    display: "flex",
                                    justifyContent: "space-between",
                                    alignItems: "center",
                                    padding: "18px 24px",
                                    cursor: "pointer",
                                    userSelect: "none",
                                    transition: "background 0.2s ease"
                                }}
                            >
                                <div style={{ display: "flex", alignItems: "center", gap: "12px" }}>
                                    <span style={{ fontSize: "20px" }}>📈</span>
                                    <h4 className="desc-stat-column-title" style={{ margin: 0 }}>
                                        {stat.columnName}
                                    </h4>
                                    <span
                                        style={{
                                            fontSize: "11px",
                                            fontWeight: 700,
                                            padding: "4px 10px",
                                            borderRadius: "14px",
                                            background: "linear-gradient(135deg, rgba(236, 72, 153, 0.2) 0%, rgba(139, 92, 246, 0.2) 100%)",
                                            color: "#f472b6",
                                            border: "1px solid rgba(236, 72, 153, 0.3)"
                                        }}
                                    >
                                        12 Metrik
                                    </span>
                                </div>

                                <div style={{ display: "flex", alignItems: "center", gap: "16px" }}>
                                    {/* Quick Summary Pill preview when collapsed */}
                                    {!isOpen && (
                                        <div style={{ fontSize: "12px", color: "var(--text-muted)", display: "flex", gap: "12px" }}>
                                            <span>Ort: <strong>{stat.mean?.toFixed(2) ?? "-"}</strong></span>
                                            <span>Medyan: <strong>{stat.median?.toFixed(2) ?? "-"}</strong></span>
                                            <span>Min/Max: <strong>{stat.min?.toFixed(1)}/{stat.max?.toFixed(1)}</strong></span>
                                        </div>
                                    )}

                                    {/* Accordion Arrow Button */}
                                    <button
                                        style={{
                                            width: "36px",
                                            height: "36px",
                                            borderRadius: "50%",
                                            border: "1px solid rgba(236, 72, 153, 0.4)",
                                            background: "linear-gradient(135deg, #ec4899 0%, #8b5cf6 100%)",
                                            color: "#fff",
                                            display: "flex",
                                            alignItems: "center",
                                            justifyContent: "center",
                                            fontSize: "14px",
                                            cursor: "pointer",
                                            transition: "transform 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
                                            transform: isOpen ? "rotate(180deg)" : "rotate(0deg)"
                                        }}
                                    >
                                        ▼
                                    </button>
                                </div>
                            </div>

                            {/* Accordion Expandable Content */}
                            {isOpen && (
                                <div
                                    style={{
                                        padding: "0 24px 24px 24px",
                                        borderTop: "1px solid rgba(255, 255, 255, 0.08)",
                                        animation: "fadeIn 0.25s ease-in-out"
                                    }}
                                >
                                    <div
                                        style={{
                                            display: "grid",
                                            gridTemplateColumns: "repeat(auto-fit, minmax(220px, 1fr))",
                                            gap: "14px",
                                            marginTop: "16px"
                                        }}
                                    >
                                        {metrics.map((m, mIdx) => (
                                            <div key={mIdx} className="desc-stat-item">
                                                <div className="desc-stat-lbl">
                                                    {m.icon} {m.label}
                                                </div>
                                                <div className="desc-stat-val">
                                                    {m.value}
                                                </div>
                                                <div className="desc-stat-desc">
                                                    {m.desc}
                                                </div>
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            )}
                        </div>
                    );
                })}
            </div>
        </div>
    );
};
