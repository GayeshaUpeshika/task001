using MongoDB.Driver;

public class UserService
{
    private readonly IMongoCollection<User> _users;

    public UserService(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("job");
        _users = database.GetCollection<User>("Users");
    }

    public async Task<User> RegisterAsync(string name, string email, string password)
    {
        var existingUser = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        if (existingUser != null)
        {
            throw new UserAlreadyExistsException("A user with the same email already exists.");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Email = email,
            Password = hashedPassword
        };

        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task<User> LoginAsync(string email, string password)
    {
        var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new InvalidCredentialsException("Invalid email or password.");
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            throw new InvalidCredentialsException("Invalid email or password.");
        }

        return user;
    }
}

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string message) : base(message)
    {
    }
}

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message) : base(message)
    {
    }
}