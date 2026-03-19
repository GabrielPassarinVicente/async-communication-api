using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Communication.Infrastructure.Data;

public class CommunicationDbContextFactory : IDesignTimeDbContextFactory<CommunicationDbContext>
{
    public CommunicationDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var apiPath = Path.Combine(basePath, "Communication.Api");
        if (Directory.Exists(apiPath))
        {
            basePath = apiPath;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration["ConnectionStrings:DefaultConnection"]
            ?? "Server=localhost;Port=3306;Database=communication;Uid=root;Pwd=862945;SslMode=None;AllowPublicKeyRetrieval=True;";

        var optionsBuilder = new DbContextOptionsBuilder<CommunicationDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new CommunicationDbContext(optionsBuilder.Options);
    }
}
