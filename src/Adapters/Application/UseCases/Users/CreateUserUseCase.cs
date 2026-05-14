
using FluentValidation;
using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.Mappings;
using NexoraBackend.Common.Helpers;
using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.Ports;


namespace NexoraBackend.Application.UseCases.Users;

public class CreateUserUseCase
{
    private readonly IValidator<CreateUserDto> _validator;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserMapper _mapper;
    private readonly IRoleRepository _roleRepository;

    public CreateUserUseCase(IUserRepository userRepository, IValidator<CreateUserDto> validator, IUnitOfWork unitOfWork, UserMapper mapper, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _roleRepository = roleRepository;

    }

    public async Task<UserResponseDto> Execute(CreateUserDto input)
    {
        var result = await _validator.ValidateAsync(input);

        if (!result.IsValid)
            throw new FluentValidation.ValidationException(
                string.Join(", ", result.Errors.Select(e => e.ErrorMessage))
            );

        var existingUser = await _userRepository.GetUserByEmailAsync(input.Email);

        if (existingUser)
            throw new Common.Exceptions.ValidationException("Email already exists");

        var hashedPassword = BCryptPassword.HashPassword(input.Password);

        var defaultRole = await _roleRepository.GetByNameAsync("User");

        var user = _mapper.ToDomain(input, new List<Role> { defaultRole! });
        user.Password = hashedPassword;
        await _userRepository.CreateUserAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.ToResponseDto(user);
    }
}