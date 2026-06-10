using System.Text.Json;

namespace StockExchange.Shared.Network;

public class AppMessage
{
    public string RequestId { get; set; } = Guid.NewGuid().ToString("N");
    public MessageType Type { get; set; }
    public bool Success { get; set; } = true;
    public string Error { get; set; } = string.Empty;
    public JsonElement Payload { get; set; }

    public static AppMessage Create<T>(MessageType type, T payload, string? requestId = null)
    {
        return new AppMessage
        {
            RequestId = requestId ?? Guid.NewGuid().ToString("N"),
            Type = type,
            Payload = JsonSerializer.SerializeToElement(payload)
        };
    }

    public static AppMessage Failure(MessageType type, string requestId, string error)
    {
        return new AppMessage
        {
            RequestId = requestId,
            Type = type,
            Success = false,
            Error = error,
            Payload = JsonSerializer.SerializeToElement<object?>(null)
        };
    }
}
