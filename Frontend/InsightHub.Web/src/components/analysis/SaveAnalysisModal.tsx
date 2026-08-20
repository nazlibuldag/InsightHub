import React, { useState } from "react";
import * as api from "../../services/api";

interface SaveAnalysisModalProps {
    isOpen: boolean;
    onClose: () => void;
    datasetId: string;
    datasetName: string;
    defaultAnalysisType?: string;
    defaultTitle?: string;
    configuration?: any;
    resultData?: any;
    token: string | null;
    onSavedSuccessfully: () => void;
}

export const SaveAnalysisModal: React.FC<SaveAnalysisModalProps> = ({
    isOpen,
    onClose,
    datasetId,
    datasetName,
    defaultAnalysisType = "Genel Analiz",
    defaultTitle = "",
    configuration = {},
    resultData = {},
    token,
    onSavedSuccessfully
}) => {
    const [title, setTitle] = useState(defaultTitle || `${datasetName} - Analiz (${new Date().toLocaleDateString("tr-TR")})`);
    const [notes, setNotes] = useState("");
    const [analysisType, setAnalysisType] = useState(defaultAnalysisType);
    const [isSaving, setIsSaving] = useState(false);
    const [errorMsg, setErrorMsg] = useState<string | null>(null);

    // Update title and analysis type when default props change
    React.useEffect(() => {
        if (defaultTitle) setTitle(defaultTitle);
        if (defaultAnalysisType) setAnalysisType(defaultAnalysisType);
    }, [defaultTitle, defaultAnalysisType, isOpen]);

    if (!isOpen) return null;

    const handleSave = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!title.trim()) {
            setErrorMsg("Lütfen analiz için bir başlık girin.");
            return;
        }

        setIsSaving(true);
        setErrorMsg(null);

        try {
            await api.createSavedAnalysis(
                {
                    datasetId,
                    title: title.trim(),
                    notes: notes.trim(),
                    analysisType,
                    filterJson: JSON.stringify(configuration?.filter || {}),
                    configurationJson: JSON.stringify(configuration || {}),
                    resultJson: JSON.stringify(resultData || { savedAt: new Date().toISOString() })
                },
                token
            );
            onSavedSuccessfully();
            onClose();
        } catch (err: any) {
            setErrorMsg(err.message || "Analiz kaydedilirken bir hata oluştu.");
        } finally {
            setIsSaving(false);
        }
    };

    return (
        <div className="modal-overlay">
            <div className="modal-content" style={{ maxWidth: "520px" }}>
                <div className="modal-header">
                    <h3 className="desc-stat-column-title">💾 Analizi Kaydet</h3>
                    <button className="modal-close-btn" onClick={onClose}>&times;</button>
                </div>

                <form onSubmit={handleSave} style={{ display: "flex", flexDirection: "column", gap: "16px", marginTop: "16px" }}>
                    {errorMsg && (
                        <div style={{ padding: "10px 14px", borderRadius: "8px", background: "rgba(239, 68, 68, 0.15)", color: "#ef4444", fontSize: "13px" }}>
                            ⚠️ {errorMsg}
                        </div>
                    )}

                    <div>
                        <label className="desc-stat-lbl">Analiz Başlığı *</label>
                        <input
                            type="text"
                            className="input-field"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            placeholder="Örn: Q3 Satış ve Trend Raporu"
                            required
                        />
                    </div>

                    <div>
                        <label className="desc-stat-lbl">Veri Seti</label>
                        <input
                            type="text"
                            className="input-field"
                            value={datasetName}
                            disabled
                            style={{ opacity: 0.7, cursor: "not-allowed" }}
                        />
                    </div>

                    <div>
                        <label className="desc-stat-lbl">Analiz Türü</label>
                        <select
                            className="chart-select"
                            value={analysisType}
                            onChange={(e) => setAnalysisType(e.target.value)}
                            style={{ width: "100%", padding: "10px 14px" }}
                        >
                            <option value="Genel İstatistik & Grafikler">Genel İstatistik & Grafikler</option>
                            <option value="Aykırı Değer (IQR) & Dağılım">Aykırı Değer (IQR) & Dağılım</option>
                            <option value="Korelasyon Matrisi">Korelasyon Matrisi</option>
                            <option value="ML Zaman Serisi Kestirimi">ML Zaman Serisi Kestirimi</option>
                            <option value="Özel Filtrelenmiş Segment">Özel Filtrelenmiş Segment</option>
                        </select>
                    </div>

                    <div>
                        <label className="desc-stat-lbl">Analiz Notları & Yönetici Açıklaması</label>
                        <textarea
                            className="input-field"
                            value={notes}
                            onChange={(e) => setNotes(e.target.value)}
                            placeholder="Bu analiz sonuçları ile ilgili önemli notlarınızı buraya yazabilirsiniz..."
                            rows={3}
                            style={{ resize: "vertical" }}
                        />
                    </div>

                    <div style={{ display: "flex", gap: "12px", justifyContent: "flex-end", marginTop: "10px" }}>
                        <button
                            type="button"
                            className="expand-collapse-btn"
                            onClick={onClose}
                            disabled={isSaving}
                        >
                            İptal
                        </button>
                        <button
                            type="submit"
                            className="upload-button"
                            disabled={isSaving}
                        >
                            {isSaving ? "Kaydediliyor..." : "💾 Analizi Kaydet"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};
