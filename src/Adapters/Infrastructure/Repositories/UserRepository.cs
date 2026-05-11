
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
        var user = await _dbContext.Users.ToListAsync();
        return user.Select(u => _mapper.ToDomain(u));
    }

    //update an existing user
    public async Task<User> UpdateUserAsync(User user)
    {
        var entity = await _dbContext.Users
        .FirstOrDefaultAsync(x => x.Id == user.Id);

        if (entity == null)
            throw new NotFoundException("User not found");

        if (user.Name != null) entity.Name = user.Name;
        if (user.Email != null) entity.Email = user.Email;
        if (user.PhoneNumber != null) entity.PhoneNumber = user.PhoneNumber;

        if (user.Address != null)
        {
            entity.Street = user.Address.Street ?? "";
            entity.City = user.Address.City;
            entity.Country = user.Address.Country;
        }

        return _mapper.ToDomain(entity);
    }



}
