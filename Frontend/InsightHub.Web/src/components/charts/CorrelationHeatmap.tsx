import React from "react";
import type { CorrelationMatrixResponse } from "../../types";

interface CorrelationHeatmapProps {
    data: CorrelationMatrixResponse | null;
    isLoading: boolean;
}

export const CorrelationHeatmap: React.FC<CorrelationHeatmapProps> = ({ data, isLoading }) => {
    if (isLoading) return <div className="panel"><p>Korelasyon matrisi hesaplanıyor...</p></div>;
    if (!data || !data.columns || data.columns.length === 0) {
        return <div className="panel"><p>Korelasyon matrisi için yeterli sayısal veri bulunamadı.</p></div>;
    }

    const getColor = (val: number) => {
        if (val === 1) return "rgba(139, 92, 246, 0.9)";
        if (val > 0) return `rgba(236, 72, 153, ${Math.max(val, 0.2)})`;
        if (val < 0) return `rgba(239, 68, 68, ${Math.max(Math.abs(val), 0.2)})`;
        return "rgba(255, 255, 255, 0.05)";
    };

    return (
        <div className="panel">
            <div className="panel-header">
                <div>
                    <h3>🌡️ 2D İnteraktif Korelasyon Matrisi (Heatmap)</h3>
                    <p>Değişkenler arasındaki pozitif (pembe/mor) ve negatif (kırmızı) yönlü ilişkilerin ısı haritası.</p>
                </div>
            </div>
            <div className="heatmap-wrapper">
                <table className="heatmap-table">
                    <thead>
                        <tr>
                            <th></th>
                            {data.columns.map((col, idx) => (
                                <th key={idx}>{col}</th>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {data.matrix.map((row, rIdx) => (
                            <tr key={rIdx}>
                                <td className="heatmap-row-header">{data.columns[rIdx]}</td>
                                {row.map((val, cIdx) => (
                                    <td
                                        key={cIdx}
                                        className="heatmap-cell"
                                        style={{ backgroundColor: getColor(val) }}
                                        title={`${data.columns[rIdx]} vs ${data.columns[cIdx]}: ${val.toFixed(4)}`}
                                    >
                                        {val.toFixed(2)}
                                    </td>
                                ))}
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};
