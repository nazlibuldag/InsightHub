import React, { useState, useRef, useEffect } from "react";
import type { AuthUser } from "../../types";

interface UserProfileDropdownProps {
    user: AuthUser;
    onLogout: () => void;
    onNavigate: (tab: string) => void;
}

export const UserProfileDropdown: React.FC<UserProfileDropdownProps> = ({
    user,
    onLogout,
    onNavigate
}) => {
    const [isOpen, setIsOpen] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);

    const isAdmin = user.role === 1 || user.role === 0 || (user as any).roleName === "Admin";

    // Close dropdown on outside click
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };
        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    const getInitials = () => {
        const first = user.firstName ? user.firstName[0].toUpperCase() : "";
        const last = user.lastName ? user.lastName[0].toUpperCase() : "";
        return `${first}${last}` || "U";
    };

    return (
        <div className="user-profile-container" ref={dropdownRef} style={{ position: "relative" }}>
            <button
                className="user-profile-btn"
                onClick={() => setIsOpen(!isOpen)}
                style={{
                    display: "flex",
                    alignItems: "center",
                    gap: "10px",
                    background: "rgba(255, 255, 255, 0.08)",
                    border: "1px solid rgba(244, 114, 182, 0.3)",
                    borderRadius: "30px",
                    padding: "6px 14px 6px 8px",
                    cursor: "pointer",
                    color: "var(--text-main)",
                    transition: "all 0.2s ease"
                }}
            >
                <div
                    style={{
                        width: "32px",
                        height: "32px",
                        borderRadius: "50%",
                        background: "linear-gradient(135deg, #ec4899, #8b5cf6)",
                        color: "#fff",
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        fontWeight: 800,
                        fontSize: "13px",
                        boxShadow: "0 2px 6px rgba(236, 72, 153, 0.4)"
                    }}
                >
                    {getInitials()}
                </div>

                <div style={{ display: "flex", flexDirection: "column", alignItems: "flex-start", textAlign: "left" }}>
                    <span style={{ fontSize: "13px", fontWeight: 700, lineHeight: 1.2 }}>
                        {user.firstName} {user.lastName}
                    </span>
                    <span style={{ fontSize: "11px", color: isAdmin ? "#ec4899" : "var(--text-muted)", fontWeight: 600 }}>
                        {isAdmin ? "🛡️ Yönetici (Admin)" : "Analist"}
                    </span>
                </div>

                <span style={{ fontSize: "10px", opacity: 0.7, marginLeft: "4px" }}>
                    {isOpen ? "▲" : "▼"}
                </span>
            </button>

            {/* Dropdown Menu */}
            {isOpen && (
                <div
                    className="user-dropdown-menu"
                    style={{
                        position: "absolute",
                        top: "100%",
                        right: 0,
                        marginTop: "8px",
                        width: "220px",
                        background: "var(--bg-card, #ffffff)",
                        border: "1px solid rgba(244, 114, 182, 0.4)",
                        borderRadius: "14px",
                        boxShadow: "0 10px 25px rgba(0, 0, 0, 0.2)",
                        padding: "8px 0",
                        zIndex: 100,
                        display: "flex",
                        flexDirection: "column"
                    }}
                >
                    <div style={{ padding: "10px 16px", borderBottom: "1px solid rgba(255, 255, 255, 0.1)" }}>
                        <div style={{ fontSize: "13px", fontWeight: 700, color: "var(--text-main)" }}>
                            {user.firstName} {user.lastName}
                        </div>
                        <div style={{ fontSize: "11px", color: "var(--text-muted)", wordBreak: "break-all" }}>
                            {user.email}
                        </div>
                    </div>

                    <button
                        className="dropdown-item-btn"
                        onClick={() => {
                            onNavigate("dashboard");
                            setIsOpen(false);
                        }}
                        style={{
                            padding: "10px 16px",
                            textAlign: "left",
                            background: "transparent",
                            border: "none",
                            color: "var(--text-main)",
                            cursor: "pointer",
                            fontSize: "13px",
                            display: "flex",
                            alignItems: "center",
                            gap: "8px"
                        }}
                    >
                        📊 <strong>Datasetlerim & Dashboard</strong>
                    </button>

                    <button
                        className="dropdown-item-btn"
                        onClick={() => {
                            onNavigate("saved-analyses");
                            setIsOpen(false);
                        }}
                        style={{
                            padding: "10px 16px",
                            textAlign: "left",
                            background: "transparent",
                            border: "none",
                            color: "var(--text-main)",
                            cursor: "pointer",
                            fontSize: "13px",
                            display: "flex",
                            alignItems: "center",
                            gap: "8px"
                        }}
                    >
                        💾 <strong>Kaydedilmiş Analizlerim</strong>
                    </button>

                    {isAdmin && (
                        <button
                            className="dropdown-item-btn"
                            onClick={() => {
                                onNavigate("admin");
                                setIsOpen(false);
                            }}
                            style={{
                                padding: "10px 16px",
                                textAlign: "left",
                                background: "rgba(236, 72, 153, 0.1)",
                                border: "none",
                                color: "#ec4899",
                                cursor: "pointer",
                                fontSize: "13px",
                                fontWeight: 700,
                                display: "flex",
                                alignItems: "center",
                                gap: "8px"
                            }}
                        >
                            🛡️ <strong>Admin Paneli</strong>
                        </button>
                    )}

                    <div style={{ borderTop: "1px solid rgba(255, 255, 255, 0.1)", margin: "6px 0" }} />

                    <button
                        className="dropdown-item-btn"
                        onClick={() => {
                            setIsOpen(false);
                            onLogout();
                        }}
                        style={{
                            padding: "10px 16px",
                            textAlign: "left",
                            background: "transparent",
                            border: "none",
                            color: "#ef4444",
                            cursor: "pointer",
                            fontSize: "13px",
                            fontWeight: 700,
                            display: "flex",
                            alignItems: "center",
                            gap: "8px"
                        }}
                    >
                        🚪 <strong>Çıkış Yap</strong>
                    </button>
                </div>
            )}
        </div>
    );
};
