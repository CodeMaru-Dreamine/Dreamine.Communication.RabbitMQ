using Dreamine.Communication.RabbitMQ.Options;

namespace Dreamine.Communication.RabbitMQ.Infrastructure;

/// <summary>
/// Creates RabbitMQ connections from Dreamine RabbitMQ options.
/// </summary>
public interface IRabbitMqConnectionFactory
{
    /// <summary>
    /// Creates a RabbitMQ connection from the supplied options.
    /// </summary>
    /// <param name="options">RabbitMQ connection and topology options.</param>
    /// <returns>A RabbitMQ connection abstraction.</returns>
    IRabbitMqConnection CreateConnection(RabbitMqMessageBusOptions options);
}
