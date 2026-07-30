using Dreamine.Communication.RabbitMQ.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Dreamine.Communication.RabbitMQ.Infrastructure;

/// <summary>
/// \if KO
/// <para>RabbitMQ.Client 라이브러리로 실제 브로커 연결을 생성합니다.</para>
/// \endif
/// \if EN
/// <para>Creates real broker connections backed by RabbitMQ.Client.</para>
/// \endif
/// </summary>
public sealed class RabbitMqClientConnectionFactory : IRabbitMqConnectionFactory
{
    /// <summary>
    /// \if KO
    /// <para>지정한 설정으로 RabbitMQ.Client 기반 연결을 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a RabbitMQ.Client-backed connection from the specified options.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>브로커 연결 설정입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The broker connection options.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>생성된 연결 어댑터입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The created connection adapter.</para>
    /// \endif
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="options"/>가 <see langword="null"/>인 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="options"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
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

    /// <summary>
    /// \if KO
    /// <para>Rabbit Mq Client Connection 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Encapsulates rabbit mq client connection functionality and related state.</para>
    /// \endif
    /// </summary>
    private sealed class RabbitMqClientConnection : IRabbitMqConnection
    {
        /// <summary>
        /// \if KO
        /// <para>connection 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the connection value.</para>
        /// \endif
        /// </summary>
        private readonly IConnection _connection;

        /// <summary>
        /// \if KO
        /// <para>실제 RabbitMQ.Client 연결을 감싸는 어댑터를 초기화합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Initializes an adapter around a RabbitMQ.Client connection.</para>
        /// \endif
        /// </summary>
        /// <param name="connection">
        /// \if KO
        /// <para>감쌀 실제 연결입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The underlying connection to wrap.</para>
        /// \endif
        /// </param>
        public RabbitMqClientConnection(IConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// \if KO
        /// <para>실제 연결이 열려 있는지 여부를 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets whether the underlying connection is open.</para>
        /// \endif
        /// </summary>
        public bool IsOpen => _connection.IsOpen;

        /// <summary>
        /// \if KO
        /// <para>실제 RabbitMQ 모델을 감싸는 새 채널을 생성합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Creates a channel wrapping a new RabbitMQ model.</para>
        /// \endif
        /// </summary>
        /// <returns>
        /// \if KO
        /// <para>새 채널 어댑터입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A new channel adapter.</para>
        /// \endif
        /// </returns>
        public IRabbitMqChannel CreateChannel()
        {
            return new RabbitMqClientChannel(_connection.CreateModel());
        }

        /// <summary>
        /// \if KO
        /// <para>실제 연결을 닫습니다.</para>
        /// \endif
        /// \if EN
        /// <para>Closes the underlying connection.</para>
        /// \endif
        /// </summary>
        public void Close()
        {
            _connection.Close();
        }

        /// <summary>
        /// \if KO
        /// <para>실제 연결 리소스를 해제합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Disposes the underlying connection resources.</para>
        /// \endif
        /// </summary>
        public void Dispose()
        {
            _connection.Dispose();
        }
    }

    /// <summary>
    /// \if KO
    /// <para>Rabbit Mq Client Channel 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Encapsulates rabbit mq client channel functionality and related state.</para>
    /// \endif
    /// </summary>
    private sealed class RabbitMqClientChannel : IRabbitMqChannel
    {
        /// <summary>
        /// \if KO
        /// <para>channel 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the channel value.</para>
        /// \endif
        /// </summary>
        private readonly IModel _channel;

        /// <summary>
        /// \if KO
        /// <para>실제 RabbitMQ 채널 모델을 감싸는 어댑터를 초기화합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Initializes an adapter around a RabbitMQ channel model.</para>
        /// \endif
        /// </summary>
        /// <param name="channel">
        /// \if KO
        /// <para>감쌀 실제 채널입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The underlying channel to wrap.</para>
        /// \endif
        /// </param>
        public RabbitMqClientChannel(IModel channel)
        {
            _channel = channel;
        }

        /// <summary>
        /// \if KO
        /// <para>실제 채널이 열려 있는지 여부를 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets whether the underlying channel is open.</para>
        /// \endif
        /// </summary>
        public bool IsOpen => _channel.IsOpen;

        /// <summary>
        /// \if KO
        /// <para>Exchange Declare 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the exchange declare operation.</para>
        /// \endif
        /// </summary>
        /// <param name="exchange">
        /// \if KO
        /// <para>exchange에 사용할 <see cref="string"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="string"/> value used for exchange.</para>
        /// \endif
        /// </param>
        /// <param name="type">
        /// \if KO
        /// <para>type에 사용할 <see cref="string"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="string"/> value used for type.</para>
        /// \endif
        /// </param>
        /// <param name="durable">
        /// \if KO
        /// <para>durable에 사용할 <see cref="bool"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="bool"/> value used for durable.</para>
        /// \endif
        /// </param>
        /// <param name="autoDelete">
        /// \if KO
        /// <para>auto Delete에 사용할 <see cref="bool"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="bool"/> value used for auto delete.</para>
        /// \endif
        /// </param>
        public void ExchangeDeclare(
            string exchange,
            string type,
            bool durable,
            bool autoDelete)
        {
            _channel.ExchangeDeclare(exchange, type, durable, autoDelete, arguments: null);
        }

        /// <summary>
        /// \if KO
        /// <para>Queue Declare 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the queue declare operation.</para>
        /// \endif
        /// </summary>
        /// <param name="queue">
        /// \if KO
        /// <para>queue에 사용할 <see cref="string"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="string"/> value used for queue.</para>
        /// \endif
        /// </param>
        /// <param name="durable">
        /// \if KO
        /// <para>durable에 사용할 <see cref="bool"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="bool"/> value used for durable.</para>
        /// \endif
        /// </param>
        /// <param name="exclusive">
        /// \if KO
        /// <para>exclusive에 사용할 <see cref="bool"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="bool"/> value used for exclusive.</para>
        /// \endif
        /// </param>
        /// <param name="autoDelete">
        /// \if KO
        /// <para>auto Delete에 사용할 <see cref="bool"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="bool"/> value used for auto delete.</para>
        /// \endif
        /// </param>
        public void QueueDeclare(
            string queue,
            bool durable,
            bool exclusive,
            bool autoDelete)
        {
            _channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments: null);
        }

        /// <summary>
        /// \if KO
        /// <para>Queue Bind 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the queue bind operation.</para>
        /// \endif
        /// </summary>
        /// <param name="queue">
        /// \if KO
        /// <para>queue에 사용할 <see cref="string"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="string"/> value used for queue.</para>
        /// \endif
        /// </param>
        /// <param name="exchange">
        /// \if KO
        /// <para>exchange에 사용할 <see cref="string"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="string"/> value used for exchange.</para>
        /// \endif
        /// </param>
        /// <param name="routingKey">
        /// \if KO
        /// <para>routing Key에 사용할 <see cref="string"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="string"/> value used for routing key.</para>
        /// \endif
        /// </param>
        public void QueueBind(
            string queue,
            string exchange,
            string routingKey)
        {
            _channel.QueueBind(queue, exchange, routingKey);
        }

        /// <summary>
        /// \if KO
        /// <para>Basic Properties 값을 생성합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Creates the basic properties value.</para>
        /// \endif
        /// </summary>
        /// <returns>
        /// \if KO
        /// <para>Create Basic Properties 작업에서 생성한 <see cref="IRabbitMqBasicProperties"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="IRabbitMqBasicProperties"/> result produced by the create basic properties operation.</para>
        /// \endif
        /// </returns>
        public IRabbitMqBasicProperties CreateBasicProperties()
        {
            return new RabbitMqClientBasicProperties(_channel.CreateBasicProperties());
        }

        /// <summary>
        /// \if KO
        /// <para>Basic Publish 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the basic publish operation.</para>
        /// \endif
        /// </summary>
        /// <param name="exchange">
        /// \if KO
        /// <para>exchange에 사용할 <see cref="string"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="string"/> value used for exchange.</para>
        /// \endif
        /// </param>
        /// <param name="routingKey">
        /// \if KO
        /// <para>routing Key에 사용할 <see cref="string"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="string"/> value used for routing key.</para>
        /// \endif
        /// </param>
        /// <param name="mandatory">
        /// \if KO
        /// <para>mandatory에 사용할 <see cref="bool"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="bool"/> value used for mandatory.</para>
        /// \endif
        /// </param>
        /// <param name="properties">
        /// \if KO
        /// <para>properties에 사용할 <see cref="IRabbitMqBasicProperties"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="IRabbitMqBasicProperties"/> value used for properties.</para>
        /// \endif
        /// </param>
        /// <param name="body">
        /// \if KO
        /// <para>body에 사용할 <c>ReadOnlyMemory&lt;byte&gt;</c> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <c>ReadOnlyMemory&lt;byte&gt;</c> value used for body.</para>
        /// \endif
        /// </param>
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

        /// <summary>
        /// \if KO
        /// <para>Basic Consume 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the basic consume operation.</para>
        /// \endif
        /// </summary>
        /// <param name="queue">
        /// \if KO
        /// <para>queue에 사용할 <see cref="string"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="string"/> value used for queue.</para>
        /// \endif
        /// </param>
        /// <param name="autoAck">
        /// \if KO
        /// <para>auto Ack에 사용할 <see cref="bool"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="bool"/> value used for auto ack.</para>
        /// \endif
        /// </param>
        /// <param name="onReceived">
        /// \if KO
        /// <para>on Received에 사용할 <c>Func&lt;RabbitMqDelivery, CancellationToken, Task&gt;</c> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <c>Func&lt;RabbitMqDelivery, CancellationToken, Task&gt;</c> value used for on received.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>Basic Consume 작업에서 생성한 <see cref="string"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="string"/> result produced by the basic consume operation.</para>
        /// \endif
        /// </returns>
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

        /// <summary>
        /// \if KO
        /// <para>Basic Ack 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the basic ack operation.</para>
        /// \endif
        /// </summary>
        /// <param name="deliveryTag">
        /// \if KO
        /// <para>delivery Tag에 사용할 <see cref="ulong"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="ulong"/> value used for delivery tag.</para>
        /// \endif
        /// </param>
        /// <param name="multiple">
        /// \if KO
        /// <para>multiple에 사용할 <see cref="bool"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="bool"/> value used for multiple.</para>
        /// \endif
        /// </param>
        public void BasicAck(ulong deliveryTag, bool multiple)
        {
            _channel.BasicAck(deliveryTag, multiple);
        }

        /// <summary>
        /// \if KO
        /// <para>Basic Nack 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the basic nack operation.</para>
        /// \endif
        /// </summary>
        /// <param name="deliveryTag">
        /// \if KO
        /// <para>delivery Tag에 사용할 <see cref="ulong"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="ulong"/> value used for delivery tag.</para>
        /// \endif
        /// </param>
        /// <param name="multiple">
        /// \if KO
        /// <para>multiple에 사용할 <see cref="bool"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="bool"/> value used for multiple.</para>
        /// \endif
        /// </param>
        /// <param name="requeue">
        /// \if KO
        /// <para>requeue에 사용할 <see cref="bool"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="bool"/> value used for requeue.</para>
        /// \endif
        /// </param>
        public void BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            _channel.BasicNack(deliveryTag, multiple, requeue);
        }

        /// <summary>
        /// \if KO
        /// <para>Basic Reject 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the basic reject operation.</para>
        /// \endif
        /// </summary>
        /// <param name="deliveryTag">
        /// \if KO
        /// <para>delivery Tag에 사용할 <see cref="ulong"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="ulong"/> value used for delivery tag.</para>
        /// \endif
        /// </param>
        /// <param name="requeue">
        /// \if KO
        /// <para>requeue에 사용할 <see cref="bool"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="bool"/> value used for requeue.</para>
        /// \endif
        /// </param>
        public void BasicReject(ulong deliveryTag, bool requeue)
        {
            _channel.BasicReject(deliveryTag, requeue);
        }

        /// <summary>
        /// \if KO
        /// <para>Basic Cancel 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the basic cancel operation.</para>
        /// \endif
        /// </summary>
        /// <param name="consumerTag">
        /// \if KO
        /// <para>consumer Tag에 사용할 <see cref="string"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="string"/> value used for consumer tag.</para>
        /// \endif
        /// </param>
        public void BasicCancel(string consumerTag)
        {
            _channel.BasicCancel(consumerTag);
        }

        /// <summary>
        /// \if KO
        /// <para>Close 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the close operation.</para>
        /// \endif
        /// </summary>
        public void Close()
        {
            _channel.Close();
        }

        /// <summary>
        /// \if KO
        /// <para>실제 채널 리소스를 해제합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Disposes the underlying channel resources.</para>
        /// \endif
        /// </summary>
        public void Dispose()
        {
            _channel.Dispose();
        }
    }

    /// <summary>
    /// \if KO
    /// <para>Rabbit Mq Client Basic Properties 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Encapsulates rabbit mq client basic properties functionality and related state.</para>
    /// \endif
    /// </summary>
    private sealed class RabbitMqClientBasicProperties : IRabbitMqBasicProperties
    {
        /// <summary>
        /// \if KO
        /// <para>실제 RabbitMQ 메시지 속성을 감싸는 어댑터를 초기화합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Initializes an adapter around RabbitMQ message properties.</para>
        /// \endif
        /// </summary>
        /// <param name="inner">
        /// \if KO
        /// <para>감쌀 실제 메시지 속성입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The underlying message properties to wrap.</para>
        /// \endif
        /// </param>
        public RabbitMqClientBasicProperties(IBasicProperties inner)
        {
            Inner = inner;
        }

        /// <summary>
        /// \if KO
        /// <para>감싼 실제 RabbitMQ 메시지 속성을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the wrapped RabbitMQ message properties.</para>
        /// \endif
        /// </summary>
        public IBasicProperties Inner { get; }

        /// <summary>
        /// \if KO
        /// <para>Persistent 값을 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets the persistent value.</para>
        /// \endif
        /// </summary>
        public bool Persistent
        {
            get => Inner.Persistent;
            set => Inner.Persistent = value;
        }

        /// <summary>
        /// \if KO
        /// <para>Content Type 값을 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets the content type value.</para>
        /// \endif
        /// </summary>
        public string? ContentType
        {
            get => Inner.ContentType;
            set => Inner.ContentType = value;
        }

        /// <summary>
        /// \if KO
        /// <para>Type 값을 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets the type value.</para>
        /// \endif
        /// </summary>
        public string? Type
        {
            get => Inner.Type;
            set => Inner.Type = value;
        }
    }
}
