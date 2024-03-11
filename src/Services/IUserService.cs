using OutboxPattern.Entities;
using OutboxPattern.Models;

namespace OutboxPattern.Services
{
    public interface IUserService
    {
        User CreateUser(UserCreateModel model);
    }
}
