using FluentValidation;
using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.Mappings;
using NexoraBackend.Common.Exceptions;
using NexoraBackend.Core.Domain.Ports;


namespace NexoraBackend.Application.UseCases.Users;

public class UpdateUserUseCase
{
    private readonly IValidator<UpdateUserDto> _validator;
    private readonly IUserRepository _userRepository;

    private readonly IUnitOfWork _unitOfWork;
    private readonly UserMapper _mapper;

    public UpdateUserUseCase(IUserRepository userRepository, IValidator<UpdateUserDto> validator, IUnitOfWork unitOfWork, UserMapper mapper)
    {
        _userRepository = userRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserResponseDto> Execute(UpdateUserDto input)
    {
        var result = await _validator.ValidateAsync(input);

        if (!result.IsValid)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.ErrorMessage)));
        }
        var user = await _userRepository.GetUserByIdAsync(input.Id);

        if (user == null)
            throw new NotFoundException("User not found");

        var updatedUser = await _userRepository.UpdateUserAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.ToResponseDto(updatedUser);
    }
}