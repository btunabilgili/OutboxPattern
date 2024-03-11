using Microsoft.EntityFrameworkCore;
using OutboxPattern.DbContexts;
using OutboxPattern.Entities;
using OutboxPattern.Services;
using System.Text.Json;

namespace OutboxPattern.Jobs
{
    public class PublishOutboxMessages(
        AppDbContext appDbContext,
        IPublisher publisher,
        ILogger<PublishOutboxMessages> logger)
    {
        public async Task PublishMessages()
        {
            var outboxMessages = await appDbContext.OutboxMessages.Where(x => !x.IsProcessed).ToListAsync();

            foreach (var outboxMessage in outboxMessages)
            {
                var user = JsonSerializer.Deserialize<User>(outboxMessage.Payload);
                publisher.Publish(user);
                outboxMessage.IsProcessed = true;

                logger.LogInformation("{user} published", user);
            }

            await appDbContext.SaveChangesAsync();
        }
    }
}
