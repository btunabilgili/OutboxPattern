using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using OutboxPattern.DbContexts;
using OutboxPattern.Jobs;
using OutboxPattern.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<IPublisher, Publisher>();
builder.Services.AddSingleton<IConsumer, Consumer>();
builder.Services.AddHostedService<ConsumerHostedService>();
builder.Services.AddKeyedTransient<IUserService, UserServiceWithoutOutboxPattern>("WithoutOutboxPattern");
builder.Services.AddKeyedTransient<IUserService, UserServiceWithOutboxPattern>("WithOutboxPattern");

builder.Services.AddHangfire(cfg => cfg.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<PublishOutboxMessages>(Guid.NewGuid().ToString(), x => x.PublishMessages(), Cron.Never());

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
