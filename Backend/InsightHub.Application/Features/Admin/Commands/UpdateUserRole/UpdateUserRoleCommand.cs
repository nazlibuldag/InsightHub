using System;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Enums;
using MediatR;

namespace InsightHub.Application.Features.Admin.Commands.UpdateUserRole;

public record UpdateUserRoleCommand(Guid UserId, UserRole NewRole) : IRequest<bool>;

public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserRoleCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null) return false;

        user.Role = request.NewRole;
        await _userRepository.UpdateAsync(user, cancellationToken);
        return true;
    }
}
