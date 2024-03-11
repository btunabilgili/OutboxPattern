using OutboxPattern.DbContexts;
using OutboxPattern.Entities;
using OutboxPattern.Models;
using System.Text.Json;

namespace OutboxPattern.Services
{
    public class UserServiceWithOutboxPattern(AppDbContext appDbContext) : IUserService
    {
        public User CreateUser(UserCreateModel model)
        {
            var user = new User
            {
                Name = model.Name,
                Email = model.Email
            };

            appDbContext.Users.Add(user);

            appDbContext.OutboxMessages.Add(new OutboxMessage
            {
                Payload = JsonSerializer.Serialize(user),
                IsProcessed = false
            });

            appDbContext.SaveChanges();

            return user;
        }
    }
}
