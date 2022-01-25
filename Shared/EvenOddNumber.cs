using System;
using Newtonsoft.Json;

public class EvenOddNumber
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
    public string OrderId { get; set; }
    public int Number { get; set; }
    public string Type { get; set; }
}

