using System.Text.Json;
using Dreamine.Communication.Abstractions.Enums;
using Dreamine.Communication.Abstractions.Interfaces;
using Dreamine.Communication.Abstractions.Models;
using Dreamine.Communication.RabbitMQ.Infrastructure;
using Dreamine.Communication.RabbitMQ.Options;

namespace Dreamine.Communication.RabbitMQ.Buses;

/// <summary>
/// \brief RabbitMQ 기반 메시지 버스입니다.
/// </summary>
/// <remarks>
/// IMessageBus 계약을 RabbitMQ exchange / queue / routing key 기반으로 구현합니다.
/// MessageEnvelope는 JSON으로 직렬화되어 RabbitMQ 메시지 Body에 저장됩니다.
/// </remarks>
public sealed class RabbitMqMessageBus : IMessageBus
{
    private readonly RabbitMqMessageBusOptions _options;
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly Dictionary<string, Func<MessageEnvelope, CancellationToken, Task>> _handlers = new();
    private readonly object _syncRoot = new();

    private IRabbitMqConnection? _connection;
    private IRabbitMqChannel? _channel;
    private string? _consumerTag;

    /// <summary>
    /// \brief RabbitMqMessageBus 클래스의 새 인스턴스를 초기화합니다.
    /// </summary>
    /// <param name="options">RabbitMQ 메시지 버스 설정입니다.</param>
    public RabbitMqMessageBus(RabbitMqMessageBusOptions options)
        : this(options, new RabbitMqClientConnectionFactory())
    {
    }

    /// <summary>
    /// \brief RabbitMqMessageBus 클래스의 새 인스턴스를 초기화합니다.
    /// </summary>
    /// <param name="options">RabbitMQ 메시지 버스 설정입니다.</param>
    /// <param name="connectionFactory">RabbitMQ 연결 팩토리입니다.</param>
    public RabbitMqMessageBus(
        RabbitMqMessageBusOptions options,
        IRabbitMqConnectionFactory connectionFactory)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        ValidateOptions(_options);
    }

    /// <summary>
    /// \brief 메시지 버스의 현재 연결 상태를 가져옵니다.
    /// </summary>
    public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

    /// <summary>
    /// \brief 메시지 버스 종류를 가져옵니다.
    /// </summary>
    public TransportKind Kind => TransportKind.RabbitMq;

    /// <summary>
    /// \brief RabbitMQ 서버에 연결합니다.
    /// </summary>
    /// <param name="cancellationToken">취소 토큰입니다.</param>
    public Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (State == ConnectionState.Connected)
        {
            return Task.CompletedTask;
        }

        State = ConnectionState.Connecting;

        try
        {
            _connection = _connectionFactory.CreateConnection(_options);
            _channel = _connection.CreateChannel();

            DeclareTopology(_channel);

            State = ConnectionState.Connected;
            return Task.CompletedTask;
        }
        catch
        {
            State = ConnectionState.Faulted;
            Cleanup();
            throw;
        }
    }

    /// <summary>
    /// \brief RabbitMQ로 메시지를 발행합니다.
    /// </summary>
    /// <param name="message">발행할 메시지입니다.</param>
    /// <param name="cancellationToken">취소 토큰입니다.</param>
    public Task PublishAsync(
        MessageEnvelope message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        cancellationToken.ThrowIfCancellationRequested();

        if (_channel is null || State != ConnectionState.Connected)
        {
            throw new InvalidOperationException("RabbitMQ message bus is not connected.");
        }

        var route = string.IsNullOrWhiteSpace(message.Route)
            ? _options.RoutingKey
            : message.Route;

        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = _options.PersistentMessages;
        properties.ContentType = "application/json";
        properties.Type = nameof(MessageEnvelope);

        _channel.BasicPublish(
            exchange: _options.ExchangeName,
            routingKey: route,
            mandatory: false,
            properties: properties,
            body: body);

        return Task.CompletedTask;
    }

    /// <summary>
    /// \brief 지정한 라우트의 RabbitMQ 메시지를 구독합니다.
    /// </summary>
    /// <param name="route">구독할 라우트입니다.</param>
    /// <param name="handler">메시지 처리기입니다.</param>
    /// <param name="cancellationToken">취소 토큰입니다.</param>
    public Task SubscribeAsync(
        string route,
        Func<MessageEnvelope, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        ArgumentNullException.ThrowIfNull(handler);
        cancellationToken.ThrowIfCancellationRequested();

        if (_channel is null || State != ConnectionState.Connected)
        {
            throw new InvalidOperationException("RabbitMQ message bus is not connected.");
        }

        lock (_syncRoot)
        {
            _handlers[route] = handler;
        }

        _channel.QueueBind(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: route);

        if (!string.IsNullOrWhiteSpace(_consumerTag))
        {
            return Task.CompletedTask;
        }

        _consumerTag = _channel.BasicConsume(
            queue: _options.QueueName,
            autoAck: false,
            onReceived: (delivery, token) => HandleReceivedAsync(
                delivery,
                token.CanBeCanceled ? token : cancellationToken));

        return Task.CompletedTask;
    }

    /// <summary>
    /// \brief RabbitMQ 연결을 해제합니다.
    /// </summary>
    /// <param name="cancellationToken">취소 토큰입니다.</param>
    public Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        State = ConnectionState.Disconnecting;

        try
        {
            Cleanup();
            State = ConnectionState.Disconnected;
            return Task.CompletedTask;
        }
        catch
        {
            State = ConnectionState.Faulted;
            throw;
        }
    }

    /// <summary>
    /// \brief RabbitMQ 메시지 버스 리소스를 비동기로 해제합니다.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync().ConfigureAwait(false);
    }

    private async Task HandleReceivedAsync(
        RabbitMqDelivery delivery,
        CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            return;
        }

        try
        {
            var message = JsonSerializer.Deserialize<MessageEnvelope>(delivery.Body.Span);

            if (message is null)
            {
                _channel.BasicReject(delivery.DeliveryTag, requeue: false);
                return;
            }

            var route = string.IsNullOrWhiteSpace(message.Route)
                ? delivery.RoutingKey
                : message.Route;

            Func<MessageEnvelope, CancellationToken, Task>? handler;

            lock (_syncRoot)
            {
                if (!_handlers.TryGetValue(route, out handler) &&
                    !_handlers.TryGetValue(delivery.RoutingKey, out handler))
                {
                    handler = null;
                }
            }

            if (handler is null)
            {
                _channel.BasicAck(delivery.DeliveryTag, multiple: false);
                return;
            }

            await handler(message, cancellationToken).ConfigureAwait(false);

            _channel.BasicAck(delivery.DeliveryTag, multiple: false);
        }
        catch
        {
            try
            {
                _channel.BasicNack(
                    delivery.DeliveryTag,
                    multiple: false,
                    requeue: _options.RequeueOnHandlerError);
            }
            catch
            {
                // Ignore acknowledgement failure during shutdown or channel fault.
            }

            throw;
        }
    }

    private void DeclareTopology(IRabbitMqChannel channel)
    {
        channel.ExchangeDeclare(
            exchange: _options.ExchangeName,
            type: _options.ExchangeType,
            durable: _options.Durable,
            autoDelete: _options.AutoDelete);

        channel.QueueDeclare(
            queue: _options.QueueName,
            durable: _options.Durable,
            exclusive: _options.Exclusive,
            autoDelete: _options.AutoDelete);

        channel.QueueBind(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey);
    }

    private void Cleanup()
    {
        lock (_syncRoot)
        {
            _handlers.Clear();
        }

        if (_channel is not null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_consumerTag))
                {
                    _channel.BasicCancel(_consumerTag);
                }
            }
            catch
            {
                // Ignore consumer cancel failure during cleanup.
            }

            try
            {
                if (_channel.IsOpen)
                {
                    _channel.Close();
                }
            }
            catch
            {
                // Ignore channel close failure during cleanup.
            }

            _channel.Dispose();
            _channel = null;
        }

        if (_connection is not null)
        {
            try
            {
                if (_connection.IsOpen)
                {
                    _connection.Close();
                }
            }
            catch
            {
                // Ignore connection close failure during cleanup.
            }

            _connection.Dispose();
            _connection = null;
        }

        _consumerTag = null;
    }

    private static void ValidateOptions(RabbitMqMessageBusOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.HostName);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.VirtualHost);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.UserName);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Password);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ExchangeName);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.QueueName);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.RoutingKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ExchangeType);

        if (options.Port <= 0 || options.Port > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(options.Port));
        }
    }
}
