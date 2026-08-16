using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.AuditLogs.Queries.GetRecentAuditLogs;

public record GetRecentAuditLogsQuery(int Count = 50) : IRequest<List<AuditLogDto>>;

public record AuditLogDto(
    Guid Id,
    Guid? UserId,
    string UserEmail,
    string Action,
    string EntityName,
    string EntityId,
    string IpAddress,
    string Details,
    DateTime Timestamp
);

public class GetRecentAuditLogsQueryHandler : IRequestHandler<GetRecentAuditLogsQuery, List<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetRecentAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<List<AuditLogDto>> Handle(GetRecentAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _auditLogRepository.GetRecentLogsAsync(request.Count, cancellationToken);
        return logs.Select(x => new AuditLogDto(
            x.Id,
            x.UserId,
            x.UserEmail,
            x.Action,
            x.EntityName,
            x.EntityId,
            x.IpAddress,
            x.Details,
            x.Timestamp
        )).ToList();
    }
}
