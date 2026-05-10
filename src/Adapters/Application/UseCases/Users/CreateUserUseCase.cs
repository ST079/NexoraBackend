
using FluentValidation;
using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.Mappings;
using NexoraBackend.Common.Helpers;
using NexoraBackend.Core.Domain.Factory;
using NexoraBackend.Core.Domain.Ports;
using NexoraBackend.Core.Domain.ValueObjects;


namespace NexoraBackend.Application.UseCases.Users;

public class CreateUserUseCase
{
    private readonly IValidator<CreateUserDto> _validator;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserMapper _mapper;

    public CreateUserUseCase(IUserRepository userRepository, IValidator<CreateUserDto> validator, IUnitOfWork unitOfWork, UserMapper mapper)
    {
        _userRepository = userRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserResponseDto> Execute(CreateUserDto input)
    {
        var result = await _validator.ValidateAsync(input);

        if (!result.IsValid)
            throw new ValidationException(
                string.Join(", ", result.Errors.Select(e => e.ErrorMessage))
            );

        var existingUser = await _userRepository.GetUserByEmailAsync(input.Email);

        if (existingUser)
            throw new ValidationException("Email already exists");

        var hashedPassword = BCryptPassword.HashPassword(input.Password);

        var user = UserFactory.Create(
            input.Name,
            input.Email,
            hashedPassword,
            new Address(input.City, input.Street, input.Country),
            RoleFactory.DefaultRoles(),
            input.PhoneNumber ?? string.Empty,
            input.ProfilePictureUrl ?? string.Empty
        );

        await _userRepository.CreateUserAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.ToResponseDto(user);
    }
}