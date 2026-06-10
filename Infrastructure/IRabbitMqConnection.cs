namespace Dreamine.Communication.RabbitMQ.Infrastructure;

/// <summary>
/// Minimal RabbitMQ connection abstraction used by <c>RabbitMqMessageBus</c>.
/// </summary>
public interface IRabbitMqConnection : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the connection is currently open.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Creates a channel for broker operations.
    /// </summary>
    IRabbitMqChannel CreateChannel();

    /// <summary>
    /// Closes the connection.
    /// </summary>
    void Close();
}
