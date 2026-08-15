using System;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Features.Auth.Dtos;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Geçersiz veya süresi dolmuş Refresh Token.");
        }

        var newToken = _jwtTokenGenerator.GenerateToken(user);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        return new AuthResponseDto
        {
            Token = newToken,
            RefreshToken = newRefreshToken,
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
            }
        };
    }
}
