namespace Dreamine.Communication.RabbitMQ.Infrastructure;

/// <summary>
/// Minimal RabbitMQ channel abstraction used by <c>RabbitMqMessageBus</c>.
/// </summary>
public interface IRabbitMqChannel : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the channel is currently open.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Declares an exchange.
    /// </summary>
    void ExchangeDeclare(
        string exchange,
        string type,
        bool durable,
        bool autoDelete);

    /// <summary>
    /// Declares a queue.
    /// </summary>
    void QueueDeclare(
        string queue,
        bool durable,
        bool exclusive,
        bool autoDelete);

    /// <summary>
    /// Binds a queue to an exchange with a routing key.
    /// </summary>
    void QueueBind(
        string queue,
        string exchange,
        string routingKey);

    /// <summary>
    /// Creates message properties for a publish operation.
    /// </summary>
    IRabbitMqBasicProperties CreateBasicProperties();

    /// <summary>
    /// Publishes a message to an exchange.
    /// </summary>
    void BasicPublish(
        string exchange,
        string routingKey,
        bool mandatory,
        IRabbitMqBasicProperties properties,
        ReadOnlyMemory<byte> body);

    /// <summary>
    /// Starts consuming deliveries from a queue.
    /// </summary>
    string BasicConsume(
        string queue,
        bool autoAck,
        Func<RabbitMqDelivery, CancellationToken, Task> onReceived);

    /// <summary>
    /// Acknowledges a delivery.
    /// </summary>
    void BasicAck(ulong deliveryTag, bool multiple);

    /// <summary>
    /// Negatively acknowledges a delivery.
    /// </summary>
    void BasicNack(ulong deliveryTag, bool multiple, bool requeue);

    /// <summary>
    /// Rejects a delivery.
    /// </summary>
    void BasicReject(ulong deliveryTag, bool requeue);

    /// <summary>
    /// Cancels a consumer.
    /// </summary>
    void BasicCancel(string consumerTag);

    /// <summary>
    /// Closes the channel.
    /// </summary>
    void Close();
}
