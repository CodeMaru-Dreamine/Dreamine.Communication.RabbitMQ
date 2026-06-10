namespace Dreamine.Communication.RabbitMQ.Infrastructure;

/// <summary>
/// RabbitMQ message property abstraction used to keep the bus testable without a broker.
/// </summary>
public interface IRabbitMqBasicProperties
{
    /// <summary>
    /// Gets or sets whether the broker should persist the message.
    /// </summary>
    bool Persistent { get; set; }

    /// <summary>
    /// Gets or sets the message content type.
    /// </summary>
    string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the logical message type.
    /// </summary>
    string? Type { get; set; }
}
