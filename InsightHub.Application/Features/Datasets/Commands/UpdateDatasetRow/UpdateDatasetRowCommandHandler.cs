using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.UpdateDatasetRow;

public class UpdateDatasetRowCommandHandler
    : IRequestHandler<UpdateDatasetRowCommand, bool>
{
    private readonly IDatasetRowRepository _repository;

    public UpdateDatasetRowCommandHandler(
        IDatasetRowRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        UpdateDatasetRowCommand request,
        CancellationToken cancellationToken)
    {
        var row = await _repository.GetByDatasetIdAndRowNumberAsync(
            request.DatasetId,
            request.RowNumber,
            cancellationToken);

        if (row == null)
            return false;

        row.Data = request.Data;

        await _repository.UpdateAsync(
            row,
            cancellationToken);

        return true;
    }
}