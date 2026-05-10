
using Microsoft.EntityFrameworkCore;
using NexoraBackend.Application.Mappings;
using NexoraBackend.Common.Exceptions;
using NexoraBackend.Common.Helpers;
using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.Ports;
using NexoraBackend.Infrastructure.Persistence;

namespace NexoraBackend.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    private readonly UserMapper _mapper;

    //constructor injection of the database context
    public UserRepository(AppDbContext dbContext, UserMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }


    //create a new user and save it to the database
    public async Task<User> CreateUserAsync(User user)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (existingUser != null) throw new CustomException("A user with this email already exists.", 409);
        var userEntity = _mapper.ToEntity(user);
        await _dbContext.Users.AddAsync(userEntity);
        return user;
    }
    public async Task<User> LoginAsync(string email, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || !BCryptPassword.VerifyPassword(password, user.Password))
        {
            throw new CustomException("Invalid email or password.", 401);
        }
        return _mapper.ToDomain(user);
    }

    //delete a user by id
    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);

        if (user == null)
            throw new CustomException("User not found.", 404);

        _dbContext.Users.Remove(user);
        return true;
    }

    //fetch a user by id
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        try
        {
            var user = await _dbContext.Users.FindAsync(id);
            return _mapper.ToDomain(user);
        }
        catch (Exception ex)
        {
            throw new CustomException("An error occurred while fetching the user.", 500);
        }
    }

    public async Task<bool> GetUserByEmailAsync(string email)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
        if (user != null)
        {
            return true;
        }
        return false;
    }

    //fetch all users from the database
    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        try
        {
            var user = await _dbContext.Users.ToListAsync();
            return user.Select(_mapper.ToDomain);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while fetching users.", ex);
        }
    }

    //update an existing user
    public async Task<User> UpdateUserAsync(User user)
    {
        try
        {
            var findUser = await GetUserByIdAsync(user.Id);
            if (user.Name != null) findUser?.Name = user.Name;
            if (user.Email != null) findUser?.Email = user.Email;
            if (user.PhoneNumber != null) findUser?.PhoneNumber = user.PhoneNumber;
            if (user.Address != null) findUser?.Address = user.Address;
            return findUser!;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the user.", ex);
        }
    }


}
