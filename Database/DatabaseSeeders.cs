using System.Text.Json;
using ZapMe.Constants;
using ZapMe.Database.Models;
using ZapMe.Enums;
using ZapMe.Utils;

namespace ZapMe.Database;

public static class DatabaseSeeders
{
    private static readonly DateTime _CreationTime = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static async Task SeedAsync(DatabaseContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.Users.Any())
            await SeedAccountsAsync(context, cancellationToken);

        if (File.Exists("passwords.json"))
        {
            Console.WriteLine("Please save the passwords contained in \"passwords.json\" SECURELY, and delete them COMPLETELY from the server.", cancellationToken);
            Console.WriteLine("Server will refuse to start as long as that file exists in order to avoid passwords getting leaked", cancellationToken);
            Console.WriteLine("Server shutting down...", cancellationToken);
            Environment.Exit(0);
        }
    }

    private static async Task SeedAccountsAsync(DatabaseContext context, CancellationToken cancellationToken)
    {
        string[] accounts = new string[] {
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

        Dictionary<string, object> accountPasswords = new Dictionary<string, object>();

        for (int i = 0; i < accounts.Length; i++)
        {
            Guid id = Guid.Parse($"00000000-0000-0000-0000-{i + 1:X12}");
            string name = accounts[i];
            string email = $"{name.ToLower()}@{App.Domain}";
            string password = PasswordUtils.GeneratePassword();
            string passwordHash = PasswordUtils.HashPassword(password);

            Console.WriteLine($"Creating account {name} ({email})", cancellationToken);

            accountPasswords.Add(name, new { email, password });

            await context.Users.AddAsync(new UserEntity
            {
                Id = id,
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                AcceptedPrivacyPolicyVersion = Int32.MaxValue,
                AcceptedTermsOfServiceVersion = Int32.MaxValue,
                OnlineStatus = UserStatus.Online,
                OnlineStatusText = "I'm online!",
                CreatedAt = _CreationTime,
                UpdatedAt = _CreationTime,
                LastOnline = _CreationTime,
            }, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
        await File.WriteAllTextAsync("passwords.json", JsonSerializer.Serialize(accountPasswords, new JsonSerializerOptions { WriteIndented = true }), cancellationToken);
    }
}