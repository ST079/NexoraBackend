using NexoraBackend.Core.Domain.Ports;
using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.DTOs.Responses.Auth;
using NexoraBackend.Core.Domain.Entities;


namespace NexoraBackend.Application.UseCases.Auth;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginUseCase(IAuditLogRepository auditLogRepository, IUnitOfWork unitOfWork, IUserRepository userRepository, ITokenService jwtService, IRefreshTokenRepository refreshTokenRepository)
    {

        _userRepository = userRepository;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<LoginResponseDto> Execute(LoginDto input)
    {
        var user = await _userRepository.LoginAsync(input.Email, input.Password);

        if (user == null)
        {
            throw new Exception("Invalid email or password.");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        await _refreshTokenRepository.CreateAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(2),
            IsRevoked = false
        });

        await _unitOfWork.SaveChangesAsync();

        //logging the login action
        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = user.Id,
            Action = "LOGIN",
            Details = "User logged in successfully"
        });

        return new LoginResponseDto
        {
            LoggedInUser = new UserResponseDto
            {
                Name = user.Name,
                Email = user.Email
            },
            Token = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        };
    }
}