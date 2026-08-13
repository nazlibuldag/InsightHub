using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.DeleteDatasetRow;

public class DeleteDatasetRowCommandHandler
    : IRequestHandler<DeleteDatasetRowCommand, bool>
{
    private readonly IDatasetRowRepository _repository;

    public DeleteDatasetRowCommandHandler(
        IDatasetRowRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteDatasetRowCommand request,
        CancellationToken cancellationToken)
    {
        var row = await _repository.GetByDatasetIdAndRowNumberAsync(
            request.DatasetId,
            request.RowNumber,
            cancellationToken);

        if (row == null)
            return false;

        await _repository.DeleteAsync(
            row,
            cancellationToken);

        return true;
    }
}