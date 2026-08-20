export interface DashboardSummary {
    datasetName: string;
    totalRows: number;
    totalColumns: number;
    numericColumns: number;
    stringColumns: number;
    dateColumns: number;
    booleanColumns: number;
    totalMissingValues: number;
}

export interface DatasetColumn {
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

export interface DatasetResponse {
    id: string;
    name: string;
    description: string;
    totalRows: number;
    totalColumns: number;
    uploadedAt: string;
    columns: DatasetColumn[];
}

export interface CorrelationResponse {
    column1: string;
    column2: string;
    correlation: number;
}

export interface OutlierResponse {
    columnName: string;
    outlierCount: number;
}

export interface BarChartItem {
    columnName: string;
    average: number;
}

export interface PieChartItem {
    label: string;
    count: number;
}

export interface LineChartItem {
    rowNumber: number;
    value: number;
}

export interface ScatterChartItem {
    x: number;
    y: number;
    rowNumber: number;
}

export interface CorrelationMatrixResponse {
    columns: string[];
    matrix: number[][];
}

export interface DescriptiveStatsResponse {
    columnName: string;
    count: number;
    mean: number | null;
    median: number | null;
    mode: number | null;
    min: number | null;
    max: number | null;
    range: number | null;
    q1: number | null;
    q3: number | null;
    iqr: number | null;
    variance: number | null;
    standardDeviation: number | null;
}

export interface OutlierDetailResponse {
    columnName: string;
    q1: number;
    q3: number;
    iqr: number;
    lowerBound: number;
    upperBound: number;
    outlierCount: number;
    outliers: { rowNumber: number; value: number }[];
}

export interface DistributionBin {
    from: number;
    to: number;
    count: number;
}

export interface DistributionResponse {
    columnName: string;
    minValue: number;
    maxValue: number;
    binCount: number;
    bins: DistributionBin[];
}

export interface ForecastPointDto {
    stepIndex: number;
    predictedValue: number;
}

export interface ColumnForecastDto {
    targetColumn: string;
    columnName?: string;
    historicalAverage?: number;
    historicalValues?: number[];
    forecastedValues?: number[];
    forecastValues?: ForecastPointDto[];
    slope: number;
    intercept: number;
    rSquared: number;
    trendDirection: string;
}

export interface DatasetForecastResponse {
    datasetId: string;
    columnForecasts: ColumnForecastDto[];
}

export interface SavedAnalysisDto {
    id: string;
    userId: string;
    datasetId: string;
    datasetName: string;
    title: string;
    notes: string;
    analysisType: string;
    filterJson: string;
    configurationJson: string;
    resultJson: string;
    createdDate: string;
}

export interface DatasetListItem {
    id: string;
    name: string;
    description: string;
    totalRows: number;
    totalColumns: number;
    uploadedAt: string;
}

export interface AuthUser {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    role: number;
}

export interface AdminUserDto {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    role: number;
    isActive: boolean;
    datasetCount: number;
    savedAnalysisCount: number;
    createdDate: string;
}

export interface AdminStatsDto {
    totalUsers: number;
    totalDatasets: number;
    totalRows: number;
    totalSavedAnalyses: number;
    activeUsersCount: number;
    adminUsersCount: number;
}

export interface UserRecentDatasetDto {
    id: string;
    name: string;
    totalRows: number;
    totalColumns: number;
    uploadedAt: string;
}

export interface UserRecentAnalysisDto {
    id: string;
    datasetId: string;
    title: string;
    datasetName: string;
    analysisType: string;
    createdDate: string;
}

export interface UserDashboardSummaryDto {
    totalDatasets: number;
    totalSavedAnalyses: number;
    totalRows: number;
    recentDatasetName?: string | null;
    recentDatasetUploadedAt?: string | null;
    recentDatasets: UserRecentDatasetDto[];
    recentAnalyses: UserRecentAnalysisDto[];
}

export interface FeatureWeightDto {
    featureName: string;
    weight: number;
    importancePercent: number;
}

export interface ActualVsPredictedDto {
    sampleIndex: number;
    actual: number;
    predicted: number;
}

export interface AiPredictionResultDto {
    targetColumn: string;
    featureColumns: string[];
    modelName: string;
    predictedValue: number;
    r2Score: number;
    meanAbsoluteError: number;
    rootMeanSquaredError: number;
    featureWeights: FeatureWeightDto[];
    evaluationSamples: ActualVsPredictedDto[];
}

export interface PredictRequest {
    targetColumn: string;
    featureColumns?: string[];
    modelType?: string;
    inputValues?: Record<string, number>;
}

