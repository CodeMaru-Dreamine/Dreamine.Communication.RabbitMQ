namespace Dreamine.Communication.RabbitMQ.Infrastructure;

/// <summary>
/// Broker delivery data passed from the RabbitMQ channel adapter to the message bus.
/// </summary>
public sealed class RabbitMqDelivery
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqDelivery"/> class.
    /// </summary>
    /// <param name="deliveryTag">Broker delivery tag.</param>
    /// <param name="routingKey">Delivery routing key.</param>
    /// <param name="body">Delivery body.</param>
    public RabbitMqDelivery(
        ulong deliveryTag,
        string routingKey,
        ReadOnlyMemory<byte> body)
    {
        DeliveryTag = deliveryTag;
        RoutingKey = routingKey ?? string.Empty;
        Body = body;
    }

    /// <summary>
    /// Gets the broker delivery tag.
    /// </summary>
    public ulong DeliveryTag { get; }

    /// <summary>
    /// Gets the delivery routing key.
    /// </summary>
    public string RoutingKey { get; }

    /// <summary>
    /// Gets the delivery body.
    /// </summary>
    public ReadOnlyMemory<byte> Body { get; }
}
