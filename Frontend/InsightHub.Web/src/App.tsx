import { useEffect, useState } from "react";
import "./App.css";

const API_BASE_URL = "https://localhost:7227/api";

const DATASET_ID =
    "8bebc5bd-2d1a-4ca5-b85a-9157092f1519";

interface DashboardSummary {
    datasetName: string;
    totalRows: number;
    totalColumns: number;
    numericColumns: number;
    stringColumns: number;
    dateColumns: number;
    booleanColumns: number;
    totalMissingValues: number;
}

interface DatasetColumn {
    columnName: string;
    dataType: number;
    nullCount: number;
    uniqueCount: number;
    minValue: number | null;
    maxValue: number | null;
    averageValue: number | null;
    medianValue: number | null;
    standardDeviation: number | null;
}

interface DatasetResponse {
    id: string;
    name: string;
    description: string;
    totalRows: number;
    totalColumns: number;
    uploadedAt: string;
    columns: DatasetColumn[];
}

interface CorrelationResponse {
    column1: string;
    column2: string;
    correlation: number;
}

interface OutlierResponse {
    columnName: string;
    outlierCount: number;
}

function App() {
    const [dashboard, setDashboard] =
        useState<DashboardSummary | null>(null);

    const [columns, setColumns] =
        useState<DatasetColumn[]>([]);

    const [correlation, setCorrelation] =
        useState<CorrelationResponse | null>(null);

    const [outliers, setOutliers] =
        useState<OutlierResponse | null>(null);

    const [loading, setLoading] = useState(true);

    const [error, setError] =
        useState<string | null>(null);

    useEffect(() => {
        loadDashboard();
    }, []);

    async function loadDashboard() {
        try {
            setLoading(true);
            setError(null);

            // Dashboard Summary
            const dashboardResponse = await fetch(
                `${API_BASE_URL}/Dashboard/${DATASET_ID}`
            );

            if (!dashboardResponse.ok) {
                throw new Error(
                    `Dashboard API hatası: ${dashboardResponse.status}`
                );
            }

            const dashboardData =
                await dashboardResponse.json();

            setDashboard(dashboardData);

            // Dataset + Columns
            const datasetResponse = await fetch(
                `${API_BASE_URL}/Datasets/${DATASET_ID}`
            );

            if (!datasetResponse.ok) {
                throw new Error(
                    `Dataset API hatası: ${datasetResponse.status}`
                );
            }

            const datasetData: DatasetResponse =
                await datasetResponse.json();

            setColumns(datasetData.columns ?? []);

            // Correlation
            const correlationResponse = await fetch(
                `${API_BASE_URL}/Analysis/${DATASET_ID}/correlation?column1=sepal_length&column2=petal_length`
            );

            if (correlationResponse.ok) {
                const correlationData =
                    await correlationResponse.json();

                setCorrelation(correlationData);
            }

            // Outliers
            const outlierResponse = await fetch(
                `${API_BASE_URL}/Analysis/${DATASET_ID}/outliers?columnName=sepal_length`
            );

            if (outlierResponse.ok) {
                const outlierData =
                    await outlierResponse.json();

                setOutliers(outlierData);
            }
        } catch (err) {
            console.error(err);

            if (err instanceof Error) {
                setError(err.message);
            } else {
                setError("Bir hata oluştu.");
            }
        } finally {
            setLoading(false);
        }
    }

    function getDataTypeName(dataType: number) {
        switch (dataType) {
            case 1:
                return "Numeric";

            case 2:
                return "String";

            case 3:
                return "Date";

            case 4:
                return "Boolean";

            default:
                return "Unknown";
        }
    }

    function getDataTypeClass(dataType: number) {
        switch (dataType) {
            case 1:
                return "numeric";

            case 2:
                return "string";

            case 3:
                return "date";

            case 4:
                return "boolean";

            default:
                return "";
        }
    }

    if (loading) {
        return (
            <div className="loading-screen">
                <div className="loading-content">
                    <div className="loading-spinner"></div>

                    <h2>InsightHub</h2>

                    <p>
                        Dashboard yükleniyor...
                    </p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="error-screen">
                <div className="error-card">

                    <div className="error-icon">
                        !
                    </div>

                    <h2>
                        API bağlantı hatası
                    </h2>

                    <p>
                        {error}
                    </p>

                    <button onClick={loadDashboard}>
                        Tekrar Dene
                    </button>

                </div>
            </div>
        );
    }

    if (!dashboard) {
        return (
            <div className="error-screen">

                <div className="error-card">

                    <h2>
                        Dashboard verisi bulunamadı.
                    </h2>

                    <button onClick={loadDashboard}>
                        Tekrar Dene
                    </button>

                </div>

            </div>
        );
    }

    return (
        <div className="app">

            {/* SIDEBAR */}

            <aside className="sidebar">

                <div className="logo">

                    <div className="logo-icon">
                        I
                    </div>

                    <span>
                        InsightHub
                    </span>

                </div>

                <nav className="nav">

                    <a className="nav-item active">
                        <span>▦</span>
                        Dashboard
                    </a>

                    <a className="nav-item">
                        <span>◫</span>
                        Datasets
                    </a>

                    <a className="nav-item">
                        <span>⌁</span>
                        Analysis
                    </a>

                </nav>

                <div className="sidebar-bottom">

                    <a className="nav-item">
                        <span>⚙</span>
                        Settings
                    </a>

                    <div className="user">

                        <div className="user-avatar">
                            N
                        </div>

                        <div>

                            <strong>
                                Nazlı
                            </strong>

                            <small>
                                Data Analyst
                            </small>

                        </div>

                    </div>

                </div>

            </aside>

            {/* MAIN */}

            <main className="main">

                {/* HEADER */}

                <header className="topbar">

                    <div>

                        <h1>
                            Dashboard
                        </h1>

                        <p>
                            Dataset'inize ait genel analiz özeti
                        </p>

                    </div>

                    <button className="upload-button">
                        + Upload Dataset
                    </button>

                </header>

                {/* DATASET HEADER */}

                <section className="dataset-header">

                    <div>

                        <span className="label">
                            CURRENT DATASET
                        </span>

                        <h2>
                            {dashboard.datasetName}
                        </h2>

                        <p>
                            Dataset · {dashboard.totalRows} rows ·{" "}
                            {dashboard.totalColumns} columns
                        </p>

                    </div>

                    <button className="dataset-button">
                        {dashboard.datasetName} ▾
                    </button>

                </section>

                {/* STAT CARDS */}

                <section className="stats-grid">

                    <div className="stat-card">

                        <div className="stat-icon blue">
                            ▤
                        </div>

                        <div>

                            <span>
                                Total Rows
                            </span>

                            <strong>
                                {dashboard.totalRows}
                            </strong>

                        </div>

                    </div>

                    <div className="stat-card">

                        <div className="stat-icon purple">
                            ▦
                        </div>

                        <div>

                            <span>
                                Total Columns
                            </span>

                            <strong>
                                {dashboard.totalColumns}
                            </strong>

                        </div>

                    </div>

                    <div className="stat-card">

                        <div className="stat-icon green">
                            #
                        </div>

                        <div>

                            <span>
                                Numeric Columns
                            </span>

                            <strong>
                                {dashboard.numericColumns}
                            </strong>

                        </div>

                    </div>

                    <div className="stat-card">

                        <div className="stat-icon orange">
                            A
                        </div>

                        <div>

                            <span>
                                String Columns
                            </span>

                            <strong>
                                {dashboard.stringColumns}
                            </strong>

                        </div>

                    </div>

                    <div className="stat-card">

                        <div className="stat-icon red">
                            !
                        </div>

                        <div>

                            <span>
                                Missing Values
                            </span>

                            <strong>
                                {dashboard.totalMissingValues}
                            </strong>

                        </div>

                    </div>

                </section>

                {/* CONTENT */}

                <section className="content-grid">

                    {/* COLUMN OVERVIEW */}

                    <div className="panel">

                        <div className="panel-header">

                            <div>

                                <h3>
                                    Column Overview
                                </h3>

                                <p>
                                    Dataset kolonlarının genel görünümü
                                </p>

                            </div>

                            <button className="view-all">
                                View all
                            </button>

                        </div>

                        <div className="table">

                            <div className="table-row table-head">

                                <span>
                                    Column
                                </span>

                                <span>
                                    Type
                                </span>

                                <span>
                                    Unique
                                </span>

                                <span>
                                    Missing
                                </span>

                            </div>

                            {columns.length > 0 ? (

                                columns.map((column) => (

                                    <div
                                        className="table-row"
                                        key={column.columnName}
                                    >

                                        <span>
                                            {column.columnName}
                                        </span>

                                        <span>

                                            <b
                                                className={`type ${getDataTypeClass(
                                                    column.dataType
                                                )}`}
                                            >
                                                {getDataTypeName(
                                                    column.dataType
                                                )}
                                            </b>

                                        </span>

                                        {/* GERÇEK UNIQUE DEĞERİ */}

                                        <span>
                                            {column.uniqueCount}
                                        </span>

                                        {/* GERÇEK MISSING DEĞERİ */}

                                        <span>
                                            {column.nullCount}
                                        </span>

                                    </div>

                                ))

                            ) : (

                                <div className="empty-row">
                                    Kolon bilgisi bulunamadı.
                                </div>

                            )}

                        </div>

                    </div>

                    {/* DATASET HEALTH */}

                    <div className="panel">

                        <div className="panel-header">

                            <div>

                                <h3>
                                    Dataset Health
                                </h3>

                                <p>
                                    Veri kalitesi özeti
                                </p>

                            </div>

                        </div>

                        <div className="health-score">

                            <div className="score-circle">

                                <strong>
                                    {dashboard.totalMissingValues === 0
                                        ? "100"
                                        : "—"}
                                </strong>

                                <span>
                                    %
                                </span>

                            </div>

                            <div>

                                <h4>
                                    {dashboard.totalMissingValues === 0
                                        ? "Excellent"
                                        : "Needs attention"}
                                </h4>

                                <p>
                                    {dashboard.totalMissingValues === 0
                                        ? "Datasetinizde eksik değer bulunmuyor."
                                        : `${dashboard.totalMissingValues} adet eksik değer bulunuyor.`}
                                </p>

                            </div>

                        </div>

                        <div className="health-item">

                            <span>

                                <i className="dot green-dot"></i>

                                Missing values

                            </span>

                            <strong>
                                {dashboard.totalMissingValues}
                            </strong>

                        </div>

                        <div className="health-item">

                            <span>

                                <i className="dot purple-dot"></i>

                                Numeric columns

                            </span>

                            <strong>
                                {dashboard.numericColumns}
                            </strong>

                        </div>

                        <div className="health-item">

                            <span>

                                <i className="dot orange-dot"></i>

                                Categorical columns

                            </span>

                            <strong>
                                {dashboard.stringColumns}
                            </strong>

                        </div>

                    </div>

                </section>

                {/* ANALYSIS */}

                <section className="analysis-grid">

                    <div className="analysis-card">

                        <div className="analysis-icon">
                            ∿
                        </div>

                        <div>

                            <span>
                                Correlation
                            </span>

                            <strong>
                                {correlation
                                    ? correlation.correlation.toFixed(2)
                                    : "—"}
                            </strong>

                            <small>
                                sepal_length ↔ petal_length
                            </small>

                        </div>

                    </div>

                    <div className="analysis-card">

                        <div className="analysis-icon">
                            ◒
                        </div>

                        <div>

                            <span>
                                Numeric Columns
                            </span>

                            <strong>
                                {dashboard.numericColumns}
                            </strong>

                            <small>
                                Numeric data detected
                            </small>

                        </div>

                    </div>

                    <div className="analysis-card">

                        <div className="analysis-icon">
                            ⚠
                        </div>

                        <div>

                            <span>
                                Outliers
                            </span>

                            <strong>
                                {outliers
                                    ? outliers.outlierCount
                                    : "—"}
                            </strong>

                            <small>
                                sepal_length
                            </small>

                        </div>

                    </div>

                </section>

                {/* DATA TYPES */}

                <section className="data-type-section">

                    <div className="panel-header">

                        <div>

                            <h3>
                                Data Types
                            </h3>

                            <p>
                                Dataset içerisindeki kolon türleri
                            </p>

                        </div>

                    </div>

                    <div className="data-type-grid">

                        <div className="data-type-card">

                            <span className="data-type-number">
                                {dashboard.numericColumns}
                            </span>

                            <span>
                                Numeric
                            </span>

                        </div>

                        <div className="data-type-card">

                            <span className="data-type-number">
                                {dashboard.stringColumns}
                            </span>

                            <span>
                                String
                            </span>

                        </div>

                        <div className="data-type-card">

                            <span className="data-type-number">
                                {dashboard.dateColumns}
                            </span>

                            <span>
                                Date
                            </span>

                        </div>

                        <div className="data-type-card">

                            <span className="data-type-number">
                                {dashboard.booleanColumns}
                            </span>

                            <span>
                                Boolean
                            </span>

                        </div>

                    </div>

                </section>

            </main>

        </div>
    );
}

export default App;