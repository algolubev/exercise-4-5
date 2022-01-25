using NServiceBus;

public class ResponceMessage : IMessage
{
    public string NumberType { get; set; }
}