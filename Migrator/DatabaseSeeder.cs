using System.Text.Json;
using ZapMe.Constants;
using ZapMe.DTOs;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Database;

public class DatabaseSeeder
{
    private readonly DatabaseContext _dbContext;
    private readonly IServiceProvider _serviceProvider;

    public DatabaseSeeder(DatabaseContext dbContext, IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_dbContext.Users.Any())
            await SeedAccountsAsync(cancellationToken);
    }

    private async Task SeedAccountsAsync(CancellationToken cancellationToken)
    {
        string[] names = new string[] {
            "Admin",
            "System",
            "Support",
            "Moderator",
            "Root",
            "Owner",
            "Developer",
            "Tester",
            "Bot",
            "Backup",
            App.AppName,
            App.AppCreator,
            "SuperUser",
            "Staff",
            "Guest",
            "Anonymous",
            "User",
            "Service",
            "Api",
            "Status"
        };

        var accounts = new List<SystemUserCreationDto>();

        var userRepo = _serviceProvider.GetRequiredService<IUserRepository>();

        foreach (string name in names)
        {
            var account = new SystemUserCreationDto(
                Username: name,
                Email: $"{name.ToLower()}@{App.Domain}",
                Password: PasswordUtils.GeneratePassword()
                );

            Console.WriteLine($"Creating account {account.Username} ({account.Email})", cancellationToken);

            await userRepo.CreateSystemUserAsync(account, cancellationToken);

            accounts.Add(account);
        }

        string fileName = "accounts.json";

        await File.WriteAllTextAsync(fileName, JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true }), cancellationToken);

        Console.WriteLine($"Please save the accounts contained in \"{fileName}\" SECURELY.", cancellationToken);
    }
}