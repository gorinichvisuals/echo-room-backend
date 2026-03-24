namespace EchoRoom.Shared.Constants.Options;

public sealed class RedisOptions
{
    public required string Configuration {  get; set; }
    public required string InstanceName { get; set; }
}