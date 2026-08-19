using System;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Admin.Commands.ToggleUserStatus;

public record ToggleUserStatusCommand(Guid UserId) : IRequest<bool>;

public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public ToggleUserStatusCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null) return false;

        user.IsActive = !user.IsActive;
        await _userRepository.UpdateAsync(user, cancellationToken);
        return true;
    }
}
