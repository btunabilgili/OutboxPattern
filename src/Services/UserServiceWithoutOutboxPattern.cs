using OutboxPattern.DbContexts;
using OutboxPattern.Entities;
using OutboxPattern.Models;

namespace OutboxPattern.Services
{
    public class UserServiceWithoutOutboxPattern(AppDbContext appDbContext, IPublisher publisher) : IUserService
    {
        public User CreateUser(UserCreateModel model)
        {
            var user = new User
            {
                Name = model.Name,
                Email = model.Email
            };

            appDbContext.Users.Add(user);

            appDbContext.SaveChanges();

            publisher.Publish(user);

            return user;
        }
    }
}
