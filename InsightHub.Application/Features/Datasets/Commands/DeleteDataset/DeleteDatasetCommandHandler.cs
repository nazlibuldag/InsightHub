using InsightHub.Application.Features.Datasets.Commands.DeleteDataset;
using InsightHub.Application.Interfaces;
using MediatR;

public class DeleteDatasetCommandHandler
    : IRequestHandler<DeleteDatasetCommand>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IFileStorageService _fileStorageService;

    public DeleteDatasetCommandHandler(
        IDatasetRepository datasetRepository,
        IFileStorageService fileStorageService)
    {
        _datasetRepository = datasetRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task Handle(
        DeleteDatasetCommand request,
        CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (dataset == null)
            throw new Exception("Dataset bulunamadı.");

        await _fileStorageService.DeleteFileAsync(dataset.FileName);

        await _datasetRepository.DeleteAsync(
            dataset,
            cancellationToken);
    }
}