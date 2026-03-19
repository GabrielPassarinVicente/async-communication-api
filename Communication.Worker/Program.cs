using Communication.Domain.Interfaces;
using Communication.Infrastructure.Data;
using Communication.Infrastructure.Repositories;
using Communication.Worker;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CommunicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<ISchedulingRepository, SchedulingRepository>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
