using System;
using NServiceBus;

[Serializable]
public class Command : IMessage, IProvideOrderIdForNumber
{
    public int Id { get; set; }
    public string OrderId { get; set; }
}

