using BankingSystemTransactionMicroservice.Consumer;
using BankingSystemTransactionMicroservice.Models;
using BankingSystemTransactionMicroservice.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<BankingSystemMicroContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Register RabbitMQ service
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddHostedService<RabbitMqConsumerService>();
builder.Services.AddSingleton<RabbitMqServiceEvent>();

builder.Services.AddScoped<RecurrentTransactionProcess>();

builder.Services.AddScoped<CalculatNextTransaction>();

// Add Hangfire services
builder.Services.AddHangfire(configuration =>
{
    configuration.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Use Hangfire Dashboard for monitoring (optional)
app.UseHangfireDashboard();

// Schedule the recurring job
RecurringJob.AddOrUpdate<RecurrentTransactionProcess>(
    handler => handler.ProcessRecurrentTransactions(), 
    Cron.Daily);

app.Run();