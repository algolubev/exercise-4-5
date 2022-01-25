using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Logging;
using Repositories;
using Services;

public class Message1Handler :
    IHandleMessages<Command>
{
    static ILog log = LogManager.GetLogger<Message1Handler>();

    private readonly NumberProcessor numberProcessor;
    private readonly CosmosDbNumbersRespository numbersRespository;

    public Message1Handler(NumberProcessor numberProcessor, CosmosDbNumbersRespository numbersRespository)
    {
        this.numberProcessor = numberProcessor;
        this.numbersRespository = numbersRespository;
    }

    public Task Handle(Command message, IMessageHandlerContext context)
    {
        log.Info("Hello from CommandMessageHandler");

        //var numberGuid = new Guid(message.OrderId);
        var numberGuid = message.OrderId;
        var number = message.Id;
        //var numberType = (message.Id % 2 == 0) ? "Even" : "Odd";
        var numberType = numberProcessor.EnsureNumberIsEven(number) ? "Even" : "Odd";

        //persistence
        var session = context.SynchronizedStorageSession.CosmosPersistenceSession();
        //session.Batch.CreateItem(new EvenOddNumber { Id = Guid.NewGuid(), OrderId = numberGuid, Number = number, Type = numberType, });
        //INumbersRespository numbersRespository = new CosmosDbNumbersRespository();
        //numbersRespository.Session = session;
        numbersRespository.AddNumber(new EvenOddNumber { Id = Guid.NewGuid(), OrderId = numberGuid, Number = number, Type = numberType});
        return context.Reply(new ResponceMessage { NumberType = numberType });
    }


    //public class EvenOddNumber
    //{
    //    [JsonProperty("id")]
    //    public Guid Id { get; set; }
    //    public string OrderId { get; set; }
    //    public int Number { get; set; }
    //    public string Type { get; set; }
    //}
}