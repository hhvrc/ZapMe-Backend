using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ZapMe.Constants;
using ZapMe.Data.Models;
using ZapMe.Enums;
using ZapMe.Utils;

namespace ZapMe.Data;

public static class DataSeeders
{
    private static readonly DateTime _CreationTime = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static async Task SeedAsync(ZapMeContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.Images.Any())
            await SeedImagesAsync(context, cancellationToken);

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

    private static async Task SeedImagesAsync([NotNull] ZapMeContext context, CancellationToken cancellationToken)
    {
        await context.Images.AddAsync(new ImageEntity
        {
            Id = ImageEntity.DefaultImageId,
            Height = 0,
            Width = 0,
            SizeBytes = 0,
            Sha256 = "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855",
            HashPerceptual = 0,
            UploaderId = null,
            Uploader = null
        }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedAccountsAsync([NotNull] ZapMeContext context, CancellationToken cancellationToken)
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
                AcceptedTosVersion = Int32.MaxValue,
                ProfilePictureId = ImageEntity.DefaultImageId,
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