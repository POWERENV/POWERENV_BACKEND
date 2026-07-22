using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace POWERENV_BACKEND_API.Redis
{
    public class RedisAuthCookieTicketStore : ITicketStore
    {
        private const string KeyPrefix = "AuthTicket:";
        private readonly IServiceProvider _serviceProvider;

        public RedisAuthCookieTicketStore(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private IDistributedCache GetCache() => _serviceProvider.GetRequiredService<IDistributedCache>();

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var guid = Guid.NewGuid().ToString();
            var key = KeyPrefix + guid;

            var options = new DistributedCacheEntryOptions();
            if (ticket.Properties.ExpiresUtc.HasValue)
            {
                options.AbsoluteExpiration = ticket.Properties.ExpiresUtc.Value;
            }
            else
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
            }

            // Serialize the deep passport data into compressed byte frames
            byte[] val = TicketSerializer.Default.Serialize(ticket);
            await GetCache().SetAsync(key, val, options);

            return guid; // The browser cookie will ONLY store this tiny GUID string identifier
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var redisKey = KeyPrefix + key;
            var options = new DistributedCacheEntryOptions();
            if (ticket.Properties.ExpiresUtc.HasValue)
            {
                options.AbsoluteExpiration = ticket.Properties.ExpiresUtc.Value;
            }

            byte[] val = TicketSerializer.Default.Serialize(ticket);
            await GetCache().SetAsync(redisKey, val, options);
        }

        public async Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            var redisKey = KeyPrefix + key;
            byte[]? bytes = await GetCache().GetAsync(redisKey);

            if (bytes == null) return null;

            return TicketSerializer.Default.Deserialize(bytes);
        }

        public async Task RemoveAsync(string key)
        {
            var redisKey = KeyPrefix + key;
            await GetCache().RemoveAsync(redisKey);
        }
    }
}