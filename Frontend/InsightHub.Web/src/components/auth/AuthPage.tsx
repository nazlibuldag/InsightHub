import React, { useState } from "react";
import type { AuthUser } from "../../types";

interface AuthPageProps {
    onLoginSuccess: (user: AuthUser, token: string) => void;
    theme: "dark" | "pink";
    setTheme: (theme: "dark" | "pink") => void;
}

export const AuthPage: React.FC<AuthPageProps> = ({
    onLoginSuccess,
    theme,
    setTheme
}) => {
    const [mode, setMode] = useState<"login" | "register">("login");
    const [email, setEmail] = useState<string>("admin@insighthub.com");
    const [password, setPassword] = useState<string>("Password123!");
    const [confirmPassword, setConfirmPassword] = useState<string>("Password123!");
    const [firstName, setFirstName] = useState<string>("Admin");
    const [lastName, setLastName] = useState<string>("User");
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(false);

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
                throw new Error(errText || "Giriş işlemi başarısız oldu. Lütfen bilgilerinizi kontrol edin.");
            }

            const data = await res.json();
            const token = data.token;
            const user = data.user || {
                id: data.userId,
                email,
                firstName: data.firstName || firstName,
                lastName: data.lastName || lastName,
                role: data.role || (email.toLowerCase().includes("admin") ? 1 : 2)
            };

            localStorage.setItem("insighthub_token", token);
            localStorage.setItem("insighthub_user", JSON.stringify(user));
            onLoginSuccess(user, token);
        } catch (err: any) {
            setError(err.message || "Bir bağlantı hatası oluştu.");
        } finally {
            setLoading(false);
        }
    };

    const handleQuickFill = (roleType: "admin" | "analyst") => {
        setMode("login");
        setError(null);
        if (roleType === "admin") {
            setEmail("admin@insighthub.com");
            setPassword("Password123!");
        } else {
            setEmail("nazli@insighthub.com");
            setPassword("Password123!");
        }
    };

    return (
        <div
            className={`auth-page-wrapper ${theme === "pink" ? "theme-pink" : ""}`}
            style={{
                minHeight: "100vh",
                width: "100vw",
                display: "flex",
                background: "var(--bg-app)",
                color: "var(--text-main)",
                overflow: "hidden"
            }}
        >
            {/* LEFT HERO / BRANDING PANEL */}
            <div
                className="auth-hero-panel"
                style={{
                    flex: "1.1",
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "space-between",
                    padding: "48px 60px",
                    background: "linear-gradient(135deg, rgba(236, 72, 153, 0.15) 0%, rgba(139, 92, 246, 0.15) 100%)",
                    borderRight: "1px solid var(--border-card)",
                    position: "relative"
                }}
            >
                <div>
                    <div style={{ display: "flex", alignItems: "center", gap: "12px", marginBottom: "40px" }}>
                        <div
                            style={{
                                width: "44px",
                                height: "44px",
                                borderRadius: "12px",
                                background: "linear-gradient(135deg, #ec4899, #8b5cf6)",
                                display: "flex",
                                alignItems: "center",
                                justifyContent: "center",
                                fontSize: "24px",
                                boxShadow: "0 4px 14px rgba(236, 72, 153, 0.4)"
                            }}
                        >
                            📊
                        </div>
                        <div>
                            <h2 style={{ margin: 0, fontSize: "24px", fontWeight: 800, letterSpacing: "-0.5px" }}>InsightHub</h2>
                            <span style={{ fontSize: "11px", fontWeight: 700, color: "#ec4899" }}>Kurumsal Veri Analitiği & İş Zekası Platformu</span>
                        </div>
                    </div>

                    <h1 style={{ fontSize: "36px", fontWeight: 800, lineHeight: 1.25, maxWidth: "520px", marginBottom: "20px" }}>
                        Verilerinizi Keşfedin, <span style={{ color: "#ec4899" }}>Geleceği Kestirin.</span>
                    </h1>
                    <p style={{ fontSize: "15px", color: "var(--text-muted)", lineHeight: 1.6, maxWidth: "480px", marginBottom: "32px" }}>
                        CSV ve Excel veri setlerinizi saniyeler içinde yükleyin; yapay zeka destekli istatistikler, korelasyon ısı haritaları, ML.NET zaman serisi tahminleri ve kurumsal PDF raporları ile iş kararlarınızı güçlendirin.
                    </p>

                    {/* Features List */}
                    <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "16px", maxWidth: "520px" }}>
                        <div style={{ padding: "14px", borderRadius: "12px", background: "var(--bg-card)", border: "1px solid var(--border-card)" }}>
                            <div style={{ fontSize: "18px", marginBottom: "4px" }}>🤖</div>
                            <strong style={{ fontSize: "13px" }}>ML.NET Tahminleme</strong>
                            <p style={{ margin: "2px 0 0 0", fontSize: "11px", color: "var(--text-muted)" }}>Gelecek adımları yapay zeka modelleriyle öngörün.</p>
                        </div>

                        <div style={{ padding: "14px", borderRadius: "12px", background: "var(--bg-card)", border: "1px solid var(--border-card)" }}>
                            <div style={{ fontSize: "18px", marginBottom: "4px" }}>📑</div>
                            <strong style={{ fontSize: "13px" }}>PDF Raporlama</strong>
                            <p style={{ margin: "2px 0 0 0", fontSize: "11px", color: "var(--text-muted)" }}>QuestPDF motoruyla kurumsal rapor çıktıları.</p>
                        </div>

                        <div style={{ padding: "14px", borderRadius: "12px", background: "var(--bg-card)", border: "1px solid var(--border-card)" }}>
                            <div style={{ fontSize: "18px", marginBottom: "4px" }}>🎯</div>
                            <strong style={{ fontSize: "13px" }}>IQR & Outlier Analizi</strong>
                            <p style={{ margin: "2px 0 0 0", fontSize: "11px", color: "var(--text-muted)" }}>Aykırı değerleri ve veri kalitesini otomatik tespit edin.</p>
                        </div>

                        <div style={{ padding: "14px", borderRadius: "12px", background: "var(--bg-card)", border: "1px solid var(--border-card)" }}>
                            <div style={{ fontSize: "18px", marginBottom: "4px" }}>🛡️</div>
                            <strong style={{ fontSize: "13px" }}>Rol Tabanlı Güvenlik</strong>
                            <p style={{ margin: "2px 0 0 0", fontSize: "11px", color: "var(--text-muted)" }}>Admin ve analist yetki izolasyonu ve veri sahipliği.</p>
                        </div>
                    </div>
                </div>

                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", borderTop: "1px solid var(--border-card)", paddingTop: "20px" }}>
                    <span style={{ fontSize: "12px", color: "var(--text-muted)" }}>© 2026 InsightHub Analytics Platform</span>
                    <button
                        onClick={() => setTheme(theme === "dark" ? "pink" : "dark")}
                        style={{
                            padding: "6px 14px",
                            borderRadius: "20px",
                            border: "1px solid var(--border-card)",
                            background: "var(--bg-card)",
                            color: "var(--text-main)",
                            cursor: "pointer",
                            fontSize: "12px",
                            fontWeight: 600
                        }}
                    >
                        {theme === "pink" ? "🌙 Koyu Tema" : "🌸 Pembe Tema"}
                    </button>
                </div>
            </div>

            {/* RIGHT AUTH CARD FORM */}
            <div
                className="auth-form-panel"
                style={{
                    flex: "0.9",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    padding: "40px",
                    background: "var(--bg-app)"
                }}
            >
                <div
                    style={{
                        width: "100%",
                        maxWidth: "440px",
                        background: "var(--bg-card)",
                        padding: "36px 32px",
                        borderRadius: "20px",
                        border: "1px solid var(--border-card)",
                        boxShadow: "0 15px 35px rgba(0, 0, 0, 0.1)"
                    }}
                >
                    {/* Tab Buttons: Giriş Yap / Kayıt Ol */}
                    <div
                        style={{
                            display: "flex",
                            background: "rgba(0, 0, 0, 0.05)",
                            padding: "4px",
                            borderRadius: "12px",
                            marginBottom: "24px"
                        }}
                    >
                        <button
                            type="button"
                            onClick={() => {
                                setMode("login");
                                setError(null);
                            }}
                            style={{
                                flex: 1,
                                padding: "10px",
                                borderRadius: "8px",
                                border: "none",
                                background: mode === "login" ? "linear-gradient(135deg, #ec4899, #8b5cf6)" : "transparent",
                                color: mode === "login" ? "#fff" : "var(--text-muted)",
                                fontWeight: 700,
                                fontSize: "14px",
                                cursor: "pointer",
                                transition: "all 0.2s ease"
                            }}
                        >
                            🔐 Giriş Yap
                        </button>

                        <button
                            type="button"
                            onClick={() => {
                                setMode("register");
                                setError(null);
                            }}
                            style={{
                                flex: 1,
                                padding: "10px",
                                borderRadius: "8px",
                                border: "none",
                                background: mode === "register" ? "linear-gradient(135deg, #ec4899, #8b5cf6)" : "transparent",
                                color: mode === "register" ? "#fff" : "var(--text-muted)",
                                fontWeight: 700,
                                fontSize: "14px",
                                cursor: "pointer",
                                transition: "all 0.2s ease"
                            }}
                        >
                            📝 Kayıt Ol
                        </button>
                    </div>

                    {error && (
                        <div
                            style={{
                                padding: "12px 16px",
                                borderRadius: "10px",
                                background: "rgba(239, 68, 68, 0.15)",
                                color: "#ef4444",
                                border: "1px solid rgba(239, 68, 68, 0.3)",
                                fontSize: "13px",
                                marginBottom: "20px",
                                fontWeight: 600
                            }}
                        >
                            ⚠️ {error}
                        </div>
                    )}

                    {/* FORM */}
                    <form onSubmit={handleSubmit} style={{ display: "flex", flexDirection: "column", gap: "16px" }}>
                        {mode === "register" && (
                            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "12px" }}>
                                <div>
                                    <label className="desc-stat-lbl">Ad *</label>
                                    <input
                                        type="text"
                                        className="input-field"
                                        value={firstName}
                                        onChange={(e) => setFirstName(e.target.value)}
                                        placeholder="Adınız"
                                        required
                                    />
                                </div>
                                <div>
                                    <label className="desc-stat-lbl">Soyad *</label>
                                    <input
                                        type="text"
                                        className="input-field"
                                        value={lastName}
                                        onChange={(e) => setLastName(e.target.value)}
                                        placeholder="Soyadınız"
                                        required
                                    />
                                </div>
                            </div>
                        )}

                        <div>
                            <label className="desc-stat-lbl">E-posta Adresi *</label>
                            <input
                                type="email"
                                className="input-field"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                placeholder="ornek@insighthub.com"
                                required
                            />
                        </div>

                        <div>
                            <label className="desc-stat-lbl">Şifre *</label>
                            <input
                                type="password"
                                className="input-field"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                placeholder="••••••••"
                                required
                            />
                        </div>

                        {mode === "register" && (
                            <div>
                                <label className="desc-stat-lbl">Şifre Tekrar *</label>
                                <input
                                    type="password"
                                    className="input-field"
                                    value={confirmPassword}
                                    onChange={(e) => setConfirmPassword(e.target.value)}
                                    placeholder="••••••••"
                                    required
                                />
                            </div>
                        )}

                        <button
                            type="submit"
                            className="upload-button"
                            disabled={loading}
                            style={{
                                padding: "12px",
                                fontSize: "14px",
                                justifyContent: "center",
                                marginTop: "6px"
                            }}
                        >
                            {loading ? "İşleniyor..." : mode === "login" ? "🚀 Giriş Yap & Başla" : "✨ Hesabımı Oluştur"}
                        </button>
                    </form>

                    {/* Quick Credentials / Hızlı Test Girişleri */}
                    <div style={{ marginTop: "24px", paddingTop: "18px", borderTop: "1px solid var(--border-card)" }}>
                        <span style={{ fontSize: "11px", color: "var(--text-muted)", fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.5px" }}>
                            ⚡ Hızlı Test Girişi:
                        </span>
                        <div style={{ display: "flex", gap: "8px", marginTop: "8px" }}>
                            <button
                                type="button"
                                className="expand-collapse-btn"
                                onClick={() => handleQuickFill("admin")}
                                style={{ flex: 1, padding: "6px 8px", fontSize: "12px", color: "#ec4899", borderColor: "rgba(236, 72, 153, 0.4)", fontWeight: 700 }}
                            >
                                👑 Admin Girişi
                            </button>
                            <button
                                type="button"
                                className="expand-collapse-btn"
                                onClick={() => handleQuickFill("analyst")}
                                style={{ flex: 1, padding: "6px 8px", fontSize: "12px", fontWeight: 700 }}
                            >
                                📊 Analist Girişi
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};
