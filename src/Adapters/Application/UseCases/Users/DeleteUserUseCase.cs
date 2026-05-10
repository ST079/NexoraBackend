

using NexoraBackend.Core.Domain.Ports;

namespace NexoraBackend.Application.UseCases.Users;

public class DeleteUserUseCase
{

    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Execute(Guid id)
    {
        if (id == Guid.Empty) throw new Exception("Invalid Operation, Id is required");

        var result = await _userRepository.DeleteUserAsync(id);
        if (result)
        {
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        return false;
    }
}