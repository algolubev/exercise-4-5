using NServiceBus;

namespace Repositories
{
    public class CosmosDbNumbersRespository : INumbersRespository
    {
        //public ICosmosStorageSession session;
        public ICosmosStorageSession Session { private get; set; }
        //public CosmosDbNumbersRespository(ICosmosStorageSession cosmosStorageSession)
        //{
        //    session = cosmosStorageSession;
        //}

        public void AddNumber(EvenOddNumber number)
        {
            Session.Batch.CreateItem(number);
        }
    }
}