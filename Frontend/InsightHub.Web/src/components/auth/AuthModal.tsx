import React, { useState } from "react";
import type { AuthUser } from "../../types";

interface AuthModalProps {
    isOpen: boolean;
    onClose: () => void;
    onLoginSuccess: (user: AuthUser, token: string) => void;
}

export const AuthModal: React.FC<AuthModalProps> = ({ isOpen, onClose, onLoginSuccess }) => {
    const [mode, setMode] = useState<"login" | "register">("login");
    const [email, setEmail] = useState<string>("nazli@insighthub.com");
    const [password, setPassword] = useState<string>("Password123!");
    const [confirmPassword, setConfirmPassword] = useState<string>("Password123!");
    const [firstName, setFirstName] = useState<string>("Nazlı");
    const [lastName, setLastName] = useState<string>("Buldağ");
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(false);

    if (!isOpen) return null;

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        if (mode === "register" && password !== confirmPassword) {
            setError("Girdiğiniz şifreler birbiriyle eşleşmiyor.");
            return;
        }

        setLoading(true);

        try {
            const endpoint = mode === "login" ? "/api/Auth/login" : "/api/Auth/register";
            const payload = mode === "login"
                ? { email, password }
                : { email, password, firstName, lastName, role: 2 };

            const res = await fetch(endpoint, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });

            if (!res.ok) {
                const errText = await res.text();
                throw new Error(errText || "Kimlik doğrulama başarısız oldu.");
            }

            const data = await res.json();
            const token = data.token;
            const user = data.user || { id: data.userId, email, firstName, lastName, role: 2 };

            localStorage.setItem("insighthub_token", token);
            localStorage.setItem("insighthub_user", JSON.stringify(user));
            onLoginSuccess(user, token);
            onClose();
        } catch (err: any) {
            setError(err.message || "Bir hata oluştu.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="modal-backdrop">
            <div className="modal-card">
                <div className="modal-header" style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "20px" }}>
                    <h3 className="desc-stat-column-title">{mode === "login" ? "🔐 Giriş Yap" : "📝 Kaydol"}</h3>
                    <button className="modal-close" onClick={onClose}>✕</button>
                </div>

                {error && (
                    <div style={{ padding: "10px 14px", borderRadius: "8px", background: "rgba(239, 68, 68, 0.15)", color: "#ef4444", fontSize: "13px", marginBottom: "16px" }}>
                        ⚠️ {error}
                    </div>
                )}

                <form onSubmit={handleSubmit} style={{ display: "flex", flexDirection: "column", gap: "16px" }}>
                    {mode === "register" && (
                        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "12px" }}>
                            <div className="form-group" style={{ display: "flex", flexDirection: "column", gap: "6px" }}>
                                <label className="desc-stat-lbl">Ad *</label>
                                <input
                                    type="text"
                                    className="input-field"
                                    value={firstName}
                                    onChange={(e) => setFirstName(e.target.value)}
                                    required
                                />
                            </div>
                            <div className="form-group" style={{ display: "flex", flexDirection: "column", gap: "6px" }}>
                                <label className="desc-stat-lbl">Soyad *</label>
                                <input
                                    type="text"
                                    className="input-field"
                                    value={lastName}
                                    onChange={(e) => setLastName(e.target.value)}
                                    required
                                />
                            </div>
                        </div>
                    )}

                    <div className="form-group" style={{ display: "flex", flexDirection: "column", gap: "6px" }}>
                        <label className="desc-stat-lbl">E-posta Adresi *</label>
                        <input
                            type="email"
                            className="input-field"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>

                    <div className="form-group" style={{ display: "flex", flexDirection: "column", gap: "6px" }}>
                        <label className="desc-stat-lbl">Şifre *</label>
                        <input
                            type="password"
                            className="input-field"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>

                    {mode === "register" && (
                        <div className="form-group" style={{ display: "flex", flexDirection: "column", gap: "6px" }}>
                            <label className="desc-stat-lbl">Şifre Tekrar *</label>
                            <input
                                type="password"
                                className="input-field"
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
                                required
                            />
                        </div>
                    )}

                    <button type="submit" className="upload-button" style={{ justifyContent: "center", marginTop: "8px" }} disabled={loading}>
                        {loading ? "İşleniyor..." : mode === "login" ? "Giriş Yap" : "Hesap Oluştur"}
                    </button>
                </form>

                <div style={{ marginTop: "16px", textAlign: "center", fontSize: "13px" }}>
                    {mode === "login" ? (
                        <p style={{ margin: 0, color: "var(--text-muted)" }}>
                            Hesabınız yok mu?{" "}
                            <span style={{ color: "#818cf8", cursor: "pointer", fontWeight: 600 }} onClick={() => setMode("register")}>
                                Hemen Kaydolun
                            </span>
                        </p>
                    ) : (
                        <p style={{ margin: 0, color: "var(--text-muted)" }}>
                            Zaten hesabınız var mı?{" "}
                            <span style={{ color: "#818cf8", cursor: "pointer", fontWeight: 600 }} onClick={() => setMode("login")}>
                                Giriş Yapın
                            </span>
                        </p>
                    )}
                </div>
            </div>
        </div>
    );
};
