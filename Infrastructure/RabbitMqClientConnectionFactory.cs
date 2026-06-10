using Dreamine.Communication.RabbitMQ.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Dreamine.Communication.RabbitMQ.Infrastructure;

/// <summary>
/// RabbitMQ.Client-backed connection factory.
/// </summary>
public sealed class RabbitMqClientConnectionFactory : IRabbitMqConnectionFactory
{
    /// <summary>
    /// Creates a RabbitMQ.Client-backed connection.
    /// </summary>
    /// <param name="options">RabbitMQ connection and topology options.</param>
    /// <returns>A RabbitMQ connection abstraction.</returns>
    public IRabbitMqConnection CreateConnection(RabbitMqMessageBusOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var factory = new ConnectionFactory
        {
            HostName = options.HostName,
            Port = options.Port,
            VirtualHost = options.VirtualHost,
            UserName = options.UserName,
            Password = options.Password,
            DispatchConsumersAsync = true
        };

        return new RabbitMqClientConnection(factory.CreateConnection());
    }

    private sealed class RabbitMqClientConnection : IRabbitMqConnection
    {
        private readonly IConnection _connection;

        public RabbitMqClientConnection(IConnection connection)
        {
            _connection = connection;
        }

        public bool IsOpen => _connection.IsOpen;

        public IRabbitMqChannel CreateChannel()
        {
            return new RabbitMqClientChannel(_connection.CreateModel());
        }

        public void Close()
        {
            _connection.Close();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }

    private sealed class RabbitMqClientChannel : IRabbitMqChannel
    {
        private readonly IModel _channel;

        public RabbitMqClientChannel(IModel channel)
        {
            _channel = channel;
        }

        public bool IsOpen => _channel.IsOpen;

        public void ExchangeDeclare(
            string exchange,
            string type,
            bool durable,
            bool autoDelete)
        {
            _channel.ExchangeDeclare(exchange, type, durable, autoDelete, arguments: null);
        }

        public void QueueDeclare(
            string queue,
            bool durable,
            bool exclusive,
            bool autoDelete)
        {
            _channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments: null);
        }

        public void QueueBind(
            string queue,
            string exchange,
            string routingKey)
        {
            _channel.QueueBind(queue, exchange, routingKey);
        }

        public IRabbitMqBasicProperties CreateBasicProperties()
        {
            return new RabbitMqClientBasicProperties(_channel.CreateBasicProperties());
        }

        public void BasicPublish(
            string exchange,
            string routingKey,
            bool mandatory,
            IRabbitMqBasicProperties properties,
            ReadOnlyMemory<byte> body)
        {
            var clientProperties = properties is RabbitMqClientBasicProperties wrapped
                ? wrapped.Inner
                : _channel.CreateBasicProperties();

            clientProperties.Persistent = properties.Persistent;
            clientProperties.ContentType = properties.ContentType;
            clientProperties.Type = properties.Type;

            _channel.BasicPublish(exchange, routingKey, mandatory, clientProperties, body);
        }

        public string BasicConsume(
            string queue,
            bool autoAck,
            Func<RabbitMqDelivery, CancellationToken, Task> onReceived)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (_, args) =>
            {
                var delivery = new RabbitMqDelivery(args.DeliveryTag, args.RoutingKey, args.Body);
                await onReceived(delivery, CancellationToken.None).ConfigureAwait(false);
            };

            return _channel.BasicConsume(queue, autoAck, consumer);
        }

        public void BasicAck(ulong deliveryTag, bool multiple)
        {
            _channel.BasicAck(deliveryTag, multiple);
        }

        public void BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            _channel.BasicNack(deliveryTag, multiple, requeue);
        }

        public void BasicReject(ulong deliveryTag, bool requeue)
        {
            _channel.BasicReject(deliveryTag, requeue);
        }

        public void BasicCancel(string consumerTag)
        {
            _channel.BasicCancel(consumerTag);
        }

        public void Close()
        {
            _channel.Close();
        }

        public void Dispose()
        {
            _channel.Dispose();
        }
    }

    private sealed class RabbitMqClientBasicProperties : IRabbitMqBasicProperties
    {
        public RabbitMqClientBasicProperties(IBasicProperties inner)
        {
            Inner = inner;
        }

        public IBasicProperties Inner { get; }

        public bool Persistent
        {
            get => Inner.Persistent;
            set => Inner.Persistent = value;
        }

        public string? ContentType
        {
            get => Inner.ContentType;
            set => Inner.ContentType = value;
        }

        public string? Type
        {
            get => Inner.Type;
            set => Inner.Type = value;
        }
    }
}
