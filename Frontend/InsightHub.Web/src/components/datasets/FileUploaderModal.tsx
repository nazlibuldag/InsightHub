import React, { useState } from "react";
import { uploadDataset } from "../../services/api";
import type { DatasetResponse } from "../../types";

interface FileUploaderModalProps {
    isOpen: boolean;
    onClose: () => void;
    token: string | null;
    onUploadSuccess: (newDataset: DatasetResponse) => void;
    signalRProgress?: { datasetId: string; percent: number; message: string } | null;
}

export const FileUploaderModal: React.FC<FileUploaderModalProps> = ({
    isOpen,
    onClose,
    token,
    onUploadSuccess,
    signalRProgress
}) => {
    const [name, setName] = useState<string>("");
    const [description, setDescription] = useState<string>("");
    const [file, setFile] = useState<File | null>(null);
    const [isUploading, setIsUploading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    if (!isOpen) return null;

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files[0]) {
            const f = e.target.files[0];
            setFile(f);
            if (!name) {
                setName(f.name.replace(/\.[^/.]+$/, ""));
            }
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!file) {
            setError("Lütfen bir CSV veya Excel dosyası seçin.");
            return;
        }

        setError(null);
        setIsUploading(true);

        try {
            const formData = new FormData();
            formData.append("file", file);
            formData.append("name", name || file.name);
            formData.append("description", description || "İçeri aktarılan veri seti");

            const newDataset = await uploadDataset(formData, token);
            onUploadSuccess(newDataset);
            onClose();
        } catch (err: any) {
            setError(err.message || "Yükleme sırasında hata oluştu.");
        } finally {
            setIsUploading(false);
        }
    };

    return (
        <div className="modal-backdrop">
            <div className="modal-card">
                <div className="modal-header" style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "16px" }}>
                    <h3>📁 Yeni Veri Seti Yükle</h3>
                    <button className="modal-close" onClick={onClose}>✕</button>
                </div>
                <p style={{ margin: "0 0 20px", color: "var(--text-muted)", fontSize: "13px" }}>
                    .csv, .xlsx veya .xls dosyalarınızı (50MB'a kadar) yükleyin.
                </p>

                {error && <div className="modal-error" style={{ marginBottom: "16px" }}>{error}</div>}

                <form onSubmit={handleSubmit} style={{ display: "flex", flexDirection: "column", gap: "16px" }}>
                    <div className="form-group" style={{ display: "flex", flexDirection: "column", gap: "6px" }}>
                        <label>Veri Seti Adı</label>
                        <input
                            type="text"
                            placeholder="Örn: 2026 Satış Verileri"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            required
                        />
                    </div>

                    <div className="form-group" style={{ display: "flex", flexDirection: "column", gap: "6px" }}>
                        <label>Açıklama</label>
                        <textarea
                            placeholder="Veri setinizin içeriğini tanımlayın..."
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            rows={3}
                        />
                    </div>

                    <div className="form-group" style={{ display: "flex", flexDirection: "column", gap: "6px" }}>
                        <label>Dosya Seç</label>
                        <input
                            type="file"
                            accept=".csv,.xlsx,.xls"
                            onChange={handleFileChange}
                            required
                        />
                        {file && (
                            <small style={{ color: "#a78bfa", marginTop: "4px" }}>
                                Seçilen: {file.name} ({(file.size / 1024 / 1024).toFixed(2)} MB)
                            </small>
                        )}
                    </div>

                    {isUploading && signalRProgress && (
                        <div style={{ marginTop: "8px" }}>
                            <div style={{ background: "rgba(255,255,255,0.1)", borderRadius: "6px", height: "8px", overflow: "hidden" }}>
                                <div
                                    style={{
                                        width: `${signalRProgress.percent}%`,
                                        background: "linear-gradient(90deg, #ec4899, #8b5cf6)",
                                        height: "100%",
                                        transition: "width 0.3s ease"
                                    }}
                                />
                            </div>
                            <span style={{ fontSize: "11px", color: "var(--text-muted)", marginTop: "4px", display: "block" }}>
                                %{signalRProgress.percent} - {signalRProgress.message}
                            </span>
                        </div>
                    )}

                    <div style={{ display: "flex", justifyContent: "flex-end", gap: "12px", marginTop: "12px" }}>
                        <button type="button" className="btn-secondary" onClick={onClose} disabled={isUploading}>
                            İptal
                        </button>
                        <button type="submit" className="upload-button" disabled={isUploading}>
                            {isUploading ? "Yükleniyor..." : "Yükle ve Analiz Et"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};
