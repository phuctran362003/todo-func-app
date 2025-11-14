using Infrastructure.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using User = Domain.Entities.User;

namespace Infrastructure.Repositories
{

    public class UserRepository : CosmosGenericRepository<User>
    {

        public UserRepository(Container container, ICurrentTime timeService, IClaimsService claimsService, ILogger<CosmosGenericRepository<User>> logger)
       : base(container, timeService, claimsService, logger)
        {
        }

        protected override string GetPartitionKey(Domain.Entities.User entity)
        {
            return entity.id ?? "USER";
        }
    }
}
