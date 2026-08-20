import type {
    DatasetListItem,
    DatasetResponse,
    DashboardSummary,
    CorrelationResponse,
    OutlierResponse,
    BarChartItem,
    PieChartItem,
    LineChartItem,
    ScatterChartItem,
    CorrelationMatrixResponse,
    DescriptiveStatsResponse,
    OutlierDetailResponse,
    DistributionResponse,
    DatasetForecastResponse,
    SavedAnalysisDto
} from "../types";

const API_BASE_URL = "/api";

function getAuthHeaders(token: string | null): HeadersInit {
    const headers: Record<string, string> = {
        "Content-Type": "application/json"
    };
    if (token) {
        headers["Authorization"] = `Bearer ${token}`;
    }
    return headers;
}

export async function fetchDatasets(token: string | null): Promise<DatasetListItem[]> {
    const res = await fetch(`${API_BASE_URL}/Datasets`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Dataset listesi alınamadı.");
    return res.json();
}

export async function fetchDatasetDetails(id: string, token: string | null): Promise<DatasetResponse> {
    const res = await fetch(`${API_BASE_URL}/Datasets/${id}`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Dataset detayları alınamadı.");
    return res.json();
}

export async function fetchDashboardSummary(id: string, token: string | null): Promise<DashboardSummary> {
    const res = await fetch(`${API_BASE_URL}/Dashboard/${id}`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Özet alınamadı.");
    return res.json();
}

export async function fetchCorrelation(id: string, token: string | null): Promise<CorrelationResponse[]> {
    const res = await fetch(`${API_BASE_URL}/Analysis/${id}/correlation`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Korelasyon verisi alınamadı.");
    return res.json();
}

export async function fetchOutliers(id: string, token: string | null): Promise<OutlierResponse[]> {
    const res = await fetch(`${API_BASE_URL}/Analysis/${id}/outliers`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Outlier verisi alınamadı.");
    return res.json();
}

export async function fetchBarChartData(id: string, token: string | null): Promise<BarChartItem[]> {
    const res = await fetch(`${API_BASE_URL}/Dashboard/${id}/charts/bar`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Bar chart verisi alınamadı.");
    return res.json();
}

export async function fetchPieChartData(id: string, columnName?: string, token: string | null = null): Promise<PieChartItem[]> {
    const query = columnName ? `?columnName=${encodeURIComponent(columnName)}` : "";
    const res = await fetch(`${API_BASE_URL}/Dashboard/${id}/charts/pie${query}`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Pie chart verisi alınamadı.");
    return res.json();
}

export async function fetchLineChartData(id: string, columnName?: string, token: string | null = null): Promise<LineChartItem[]> {
    const query = columnName ? `?columnName=${encodeURIComponent(columnName)}` : "";
    const res = await fetch(`${API_BASE_URL}/Dashboard/${id}/charts/line${query}`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Line chart verisi alınamadı.");
    return res.json();
}

export async function fetchScatterChartData(id: string, xColumnName?: string, yColumnName?: string, token: string | null = null): Promise<ScatterChartItem[]> {
    const queryParams: string[] = [];
    if (xColumnName) queryParams.push(`xColumnName=${encodeURIComponent(xColumnName)}`);
    if (yColumnName) queryParams.push(`yColumnName=${encodeURIComponent(yColumnName)}`);
    const query = queryParams.length > 0 ? `?${queryParams.join("&")}` : "";

    const res = await fetch(`${API_BASE_URL}/Dashboard/${id}/charts/scatter${query}`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Scatter chart verisi alınamadı.");
    return res.json();
}

export async function fetchCorrelationMatrix(id: string, token: string | null): Promise<CorrelationMatrixResponse> {
    const res = await fetch(`${API_BASE_URL}/Analysis/${id}/correlation-matrix`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Korelasyon matrisi alınamadı.");
    return res.json();
}

export async function fetchDescriptiveStats(id: string, columnName?: string, token: string | null = null): Promise<DescriptiveStatsResponse> {
    const query = columnName ? `?columnName=${encodeURIComponent(columnName)}` : "";
    const res = await fetch(`${API_BASE_URL}/Analysis/${id}/statistics${query}`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Tanımlayıcı istatistikler alınamadı.");
    return res.json();
}

export async function fetchOutlierDetails(id: string, columnName?: string, token: string | null = null): Promise<OutlierDetailResponse> {
    const query = columnName ? `?columnName=${encodeURIComponent(columnName)}` : "";
    const res = await fetch(`${API_BASE_URL}/Analysis/${id}/outliers${query}`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Detaylı aykırı değer analizi alınamadı.");
    return res.json();
}

export async function fetchDistribution(id: string, columnName?: string, binCount: number = 10, token: string | null = null): Promise<DistributionResponse> {
    const queryParams: string[] = [];
    if (columnName) queryParams.push(`columnName=${encodeURIComponent(columnName)}`);
    if (binCount) queryParams.push(`binCount=${binCount}`);
    const query = queryParams.length > 0 ? `?${queryParams.join("&")}` : "";

    const res = await fetch(`${API_BASE_URL}/Analysis/${id}/distribution${query}`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Histogram dağılımları alınamadı.");
    return res.json();
}

export async function fetchDatasetForecast(id: string, stepsAhead: number = 10, token: string | null = null): Promise<DatasetForecastResponse> {
    const res = await fetch(`${API_BASE_URL}/Analysis/${id}/forecast?stepsAhead=${stepsAhead}`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("ML Tahmin verisi alınamadı.");
    return res.json();
}

export async function fetchSavedAnalyses(token: string | null): Promise<SavedAnalysisDto[]> {
    const res = await fetch(`${API_BASE_URL}/SavedAnalysis`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Kaydedilmiş analizler alınamadı.");
    return res.json();
}

export async function fetchSavedAnalysisById(id: string, token: string | null): Promise<SavedAnalysisDto> {
    const res = await fetch(`${API_BASE_URL}/SavedAnalysis/${id}`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Kaydedilmiş analiz detayı alınamadı.");
    return res.json();
}

export async function createSavedAnalysis(
    payload: {
        datasetId: string;
        title: string;
        notes: string;
        analysisType?: string;
        filterJson?: string;
        configurationJson?: string;
        resultJson?: string;
    },
    token: string | null
): Promise<SavedAnalysisDto> {
    const res = await fetch(`${API_BASE_URL}/SavedAnalysis`, {
        method: "POST",
        headers: getAuthHeaders(token),
        body: JSON.stringify({
            datasetId: payload.datasetId,
            title: payload.title,
            notes: payload.notes || "",
            analysisType: payload.analysisType || "General",
            filterJson: payload.filterJson || "{}",
            configurationJson: payload.configurationJson || "{}",
            resultJson: payload.resultJson || "{}"
        })
    });
    if (!res.ok) throw new Error("Analiz kaydedilemedi.");
    return res.json();
}

export async function deleteSavedAnalysis(id: string, token: string | null): Promise<void> {
    const res = await fetch(`${API_BASE_URL}/SavedAnalysis/${id}`, {
        method: "DELETE",
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Kaydedilmiş analiz silinemedi.");
}

export async function downloadSavedAnalysisPdf(id: string, title: string, token: string | null): Promise<void> {
    const res = await fetch(`${API_BASE_URL}/SavedAnalysis/${id}/pdf`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) {
        const errorText = await res.text().catch(() => "");
        throw new Error(errorText || "PDF raporu indirilemedi.");
    }
    const blob = await res.blob();
    const cleanTitle = (title || "InsightHub_Analiz_Raporu")
        .replace(/[^\w\s\u00C0-\u017F-]/gi, "")
        .trim()
        .replace(/\s+/g, "_");
    const filename = cleanTitle ? `${cleanTitle}.pdf` : `InsightHub_Rapor_${id.slice(0, 8)}.pdf`;

    const url = window.URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.style.display = "none";
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    setTimeout(() => {
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    }, 200);
}

export async function fetchAdminUsers(token: string | null): Promise<import("../types").AdminUserDto[]> {
    const res = await fetch(`${API_BASE_URL}/Admin/users`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Admin kullanıcı listesi alınamadı.");
    return res.json();
}

export async function fetchAdminStats(token: string | null): Promise<import("../types").AdminStatsDto> {
    const res = await fetch(`${API_BASE_URL}/Admin/stats`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Admin istatistikleri alınamadı.");
    return res.json();
}

export async function updateUserRole(userId: string, role: number, token: string | null): Promise<void> {
    const res = await fetch(`${API_BASE_URL}/Admin/users/${userId}/role`, {
        method: "PUT",
        headers: getAuthHeaders(token),
        body: JSON.stringify({ role })
    });
    if (!res.ok) throw new Error("Kullanıcı rolü güncellenemedi.");
}

export async function toggleUserStatus(userId: string, token: string | null): Promise<void> {
    const res = await fetch(`${API_BASE_URL}/Admin/users/${userId}/toggle-status`, {
        method: "PUT",
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Kullanıcı durumu değiştirilemedi.");
}

export async function uploadDataset(formData: FormData, token: string | null): Promise<DatasetResponse> {
    const headers: Record<string, string> = {};
    if (token) {
        headers["Authorization"] = `Bearer ${token}`;
    }
    const res = await fetch(`${API_BASE_URL}/Datasets/upload`, {
        method: "POST",
        headers,
        body: formData
    });
    if (!res.ok) {
        const errorText = await res.text();
        throw new Error(errorText || "Dosya yüklenirken bir hata oluştu.");
    }
    return res.json();
}

export async function fetchUserDashboardSummary(token: string | null): Promise<import("../types").UserDashboardSummaryDto> {
    const res = await fetch(`${API_BASE_URL}/Dashboard/user-summary`, {
        headers: getAuthHeaders(token)
    });
    if (!res.ok) throw new Error("Kullanıcı dashboard verileri alınamadı.");
    return res.json();
}

export async function executeAiPrediction(
    datasetId: string,
    payload: import("../types").PredictRequest,
    token: string | null
): Promise<import("../types").AiPredictionResultDto> {
    const res = await fetch(`${API_BASE_URL}/Analysis/${datasetId}/predict`, {
        method: "POST",
        headers: getAuthHeaders(token),
        body: JSON.stringify(payload)
    });
    if (!res.ok) {
        const errorText = await res.text();
        throw new Error(errorText || "AI tahmin modeli çalıştırılamadı.");
    }
    return res.json();
}

