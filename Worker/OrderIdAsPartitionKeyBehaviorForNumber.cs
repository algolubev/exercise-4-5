using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence;
using NServiceBus.Persistence.CosmosDB;
using NServiceBus.Pipeline;
using Repositories;

namespace Worker
{
    public class OrderIdAsPartitionKeyBehaviorForNumber : Behavior<IIncomingLogicalMessageContext>
    {
        static readonly ILog Log = LogManager.GetLogger<OrderIdAsPartitionKeyBehaviorForNumber>();

        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            if (context.Message.Instance is IProvideOrderIdForNumber provideItemId)
            {
                var session = context.Extensions.Get<SynchronizedStorageSession>();

                // Get this message's Repo from the DI container
                var repo = context.Builder.Build<CosmosDbNumbersRespository>();
                
                repo.Session = session.CosmosPersistenceSession();

                var partitionKeyValue = provideItemId.OrderId.ToString();

                Log.Info($"PartitionKeyBehavior Invoke:: '{partitionKeyValue}' from '{nameof(provideItemId)}'");

                context.Extensions.Set(new PartitionKey(partitionKeyValue));

            }
            await next().ConfigureAwait(false);
        }

        public class Registration : RegisterStep
        {
            public Registration() :
                base(nameof(OrderIdAsPartitionKeyBehaviorForNumber),
                    typeof(OrderIdAsPartitionKeyBehaviorForNumber),
                    "Determines the PartitionKey from the logical message",
                    b => new OrderIdAsPartitionKeyBehaviorForNumber())
            {
                InsertBeforeIfExists(nameof(LogicalOutboxBehavior));
            }
        }
    }
}
