using FluentValidation;
using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.Ports;


namespace NexoraBackend.Application.UseCases.Users;

public class UpdateUserUseCase
{
    private readonly IValidator<UpdateUserDto> _validator;
    private readonly IUserRepository _userRepository;

    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserUseCase(IUserRepository userRepository, IValidator<UpdateUserDto> validator, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<User> Execute(Guid id, UpdateUserDto input)
    {
        var result = await _validator.ValidateAsync(input);

        if (!result.IsValid)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.ErrorMessage)));
        }
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user == null)
            throw new Exception("User not found");

        var updatedUser = await _userRepository.UpdateUserAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return updatedUser;
    }
}