using System.Text.Json;
using Dreamine.Communication.Abstractions.Enums;
using Dreamine.Communication.Abstractions.Interfaces;
using Dreamine.Communication.Abstractions.Models;
using Dreamine.Communication.RabbitMQ.Infrastructure;
using Dreamine.Communication.RabbitMQ.Options;

namespace Dreamine.Communication.RabbitMQ.Buses;

/// <summary>
/// \if KO
/// <para>RabbitMQ Exchange, Queue 및 라우팅 키를 사용하는 메시지 버스입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a message bus using RabbitMQ exchanges, queues, and routing keys.</para>
/// \endif
/// </summary>
/// <remarks>
/// \if KO
/// <para>메시지 봉투는 JSON으로 직렬화되어 RabbitMQ 본문에 저장됩니다.</para>
/// \endif
/// \if EN
/// <para>Message envelopes are serialized as JSON and stored in RabbitMQ message bodies.</para>
/// \endif
/// </remarks>
public sealed class RabbitMqMessageBus : IMessageBus
{
    /// <summary>
    /// \if KO
    /// <para>options 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the options value.</para>
    /// \endif
    /// </summary>
    private readonly RabbitMqMessageBusOptions _options;
    /// <summary>
    /// \if KO
    /// <para>connection Factory 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the connection factory value.</para>
    /// \endif
    /// </summary>
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    /// <summary>
    /// \if KO
    /// <para>handlers 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the handlers value.</para>
    /// \endif
    /// </summary>
    private readonly Dictionary<string, List<Func<MessageEnvelope, CancellationToken, Task>>> _handlers = new();
    /// <summary>
    /// \if KO
    /// <para>sync Root 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the sync root value.</para>
    /// \endif
    /// </summary>
    private readonly object _syncRoot = new();

    /// <summary>
    /// \if KO
    /// <para>connection 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the connection value.</para>
    /// \endif
    /// </summary>
    private IRabbitMqConnection? _connection;
    /// <summary>
    /// \if KO
    /// <para>channel 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the channel value.</para>
    /// \endif
    /// </summary>
    private IRabbitMqChannel? _channel;
    /// <summary>
    /// \if KO
    /// <para>consumer Tag 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the consumer tag value.</para>
    /// \endif
    /// </summary>
    private string? _consumerTag;

    /// <summary>
    /// \if KO
    /// <para>기본 연결 팩토리와 지정한 설정으로 메시지 버스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes the message bus with the default connection factory and specified options.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>RabbitMQ 연결 및 토폴로지 설정입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The RabbitMQ connection and topology options.</para>
    /// \endif
    /// </param>
    public RabbitMqMessageBus(RabbitMqMessageBusOptions options)
        : this(options, new RabbitMqClientConnectionFactory())
    {
    }

    /// <summary>
    /// \if KO
    /// <para>설정과 사용자 지정 연결 팩토리로 메시지 버스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes the message bus with options and a custom connection factory.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>RabbitMQ 연결 및 토폴로지 설정입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The RabbitMQ connection and topology options.</para>
    /// \endif
    /// </param>
    /// <param name="connectionFactory">
    /// \if KO
    /// <para>브로커 연결을 생성할 팩토리입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The factory used to create broker connections.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para>필수 입력 인자 중 하나가 <see langword="null"/>인 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when a required input argument is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    public RabbitMqMessageBus(
        RabbitMqMessageBusOptions options,
        IRabbitMqConnectionFactory connectionFactory)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        ValidateOptions(_options);
    }

    /// <summary>
    /// \if KO
    /// <para>메시지 버스의 현재 연결 상태를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the current connection state of the message bus.</para>
    /// \endif
    /// </summary>
    public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

    /// <summary>
    /// \if KO
    /// <para>RabbitMQ 전송 방식을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the RabbitMQ transport kind.</para>
    /// \endif
    /// </summary>
    public TransportKind Kind => TransportKind.RabbitMq;

    /// <summary>
    /// \if KO
    /// <para>브로커 연결과 채널을 생성하고 구성된 토폴로지를 선언합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates the broker connection and channel and declares the configured topology.</para>
    /// \endif
    /// </summary>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>연결 취소 요청을 감시하는 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A token used to observe connection cancellation.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>비동기 연결 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task representing the connection operation.</para>
    /// \endif
    /// </returns>
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
    /// \if KO
    /// <para>메시지를 JSON으로 직렬화하여 구성된 Exchange와 라우팅 키로 발행합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Serializes a message as JSON and publishes it to the configured exchange and routing key.</para>
    /// \endif
    /// </summary>
    /// <param name="message">
    /// \if KO
    /// <para>발행할 메시지입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message to publish.</para>
    /// \endif
    /// </param>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>발행 취소 요청을 감시하는 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A token used to observe publishing cancellation.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>비동기 발행 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task representing the publish operation.</para>
    /// \endif
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>현재 객체 상태에서 Publish Async 작업을 수행할 수 없는 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the publish async operation is not valid for the current object state.</para>
    /// \endif
    /// </exception>
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
    /// \if KO
    /// <para>라우팅 키를 Queue에 바인딩하고 비동기 처리기를 등록합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Binds a routing key to the queue and registers an asynchronous handler.</para>
    /// \endif
    /// </summary>
    /// <param name="route">
    /// \if KO
    /// <para>구독할 라우팅 키입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The routing key to subscribe to.</para>
    /// \endif
    /// </param>
    /// <param name="handler">
    /// \if KO
    /// <para>수신 메시지 처리기입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The received-message handler.</para>
    /// \endif
    /// </param>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>구독 취소 요청을 감시하는 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A token used to observe subscription cancellation.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>비동기 구독 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task representing the subscription operation.</para>
    /// \endif
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>현재 객체 상태에서 Subscribe Async 작업을 수행할 수 없는 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the subscribe async operation is not valid for the current object state.</para>
    /// \endif
    /// </exception>
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
            if (!_handlers.TryGetValue(route, out var handlers))
            {
                handlers = [];
                _handlers[route] = handlers;
            }

            handlers.Add(handler);
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
    /// \if KO
    /// <para>소비자, 채널 및 브로커 연결을 정리합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Cleans up the consumer, channel, and broker connection.</para>
    /// \endif
    /// </summary>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>연결 해제 취소 요청을 감시하는 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A token used to observe disconnection cancellation.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>비동기 연결 해제 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task representing the disconnection operation.</para>
    /// \endif
    /// </returns>
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
    /// \if KO
    /// <para>RabbitMQ 메시지 버스 연결과 관련 리소스를 비동기적으로 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Asynchronously releases the RabbitMQ connection and related resources.</para>
    /// \endif
    /// <returns>
    /// \if KO
    /// <para>비동기 리소스 해제 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A value task representing asynchronous disposal.</para>
    /// \endif
    /// </returns>
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// \if KO
    /// <para>브로커 배달 본문을 메시지로 역직렬화하고 라우트 처리기를 실행한 뒤 성공 또는 실패 확인을 전송합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Deserializes a broker delivery, invokes route handlers, and sends a positive or negative acknowledgement.</para>
    /// \endif
    /// </summary>
    /// <param name="delivery">
    /// \if KO
    /// <para>처리할 RabbitMQ 배달 정보입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The RabbitMQ delivery to process.</para>
    /// \endif
    /// </param>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>처리기 실행 취소 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A token used to cancel handler execution.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>비동기 배달 처리 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task representing asynchronous delivery processing.</para>
    /// \endif
    /// </returns>
    /// <exception cref="JsonException">
    /// \if KO
    /// <para>배달 본문이 올바른 메시지 JSON이 아닌 경우 발생할 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown when the delivery body is not valid message JSON.</para>
    /// \endif
    /// </exception>
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

            List<Func<MessageEnvelope, CancellationToken, Task>> handlers;

            lock (_syncRoot)
            {
                if (!_handlers.TryGetValue(route, out var routeHandlers) &&
                    !_handlers.TryGetValue(delivery.RoutingKey, out routeHandlers))
                {
                    handlers = [];
                }
                else
                {
                    handlers = new List<Func<MessageEnvelope, CancellationToken, Task>>(routeHandlers);
                }
            }

            if (handlers.Count == 0)
            {
                _channel.BasicAck(delivery.DeliveryTag, multiple: false);
                return;
            }

            foreach (var handler in handlers)
            {
                await handler(message, cancellationToken).ConfigureAwait(false);
            }

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

    /// <summary>
    /// \if KO
    /// <para>구성된 Exchange와 Queue를 선언하고 기본 라우팅 키로 바인딩합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Declares the configured exchange and queue and binds them with the default routing key.</para>
    /// \endif
    /// </summary>
    /// <param name="channel">
    /// \if KO
    /// <para>토폴로지를 선언할 열린 채널입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The open channel on which topology is declared.</para>
    /// \endif
    /// </param>
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

    /// <summary>
    /// \if KO
    /// <para>처리기와 소비자를 정리하고 채널 및 연결을 안전하게 닫아 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Clears handlers and consumers and safely closes and disposes the channel and connection.</para>
    /// \endif
    /// </summary>
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

    /// <summary>
    /// \if KO
    /// <para>필수 RabbitMQ 연결 및 토폴로지 설정과 포트 범위를 검증합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Validates required RabbitMQ connection and topology values and the port range.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>검증할 메시지 버스 설정입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message bus options to validate.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentException">
    /// \if KO
    /// <para>필수 문자열 설정이 비어 있는 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when a required string option is empty.</para>
    /// \endif
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// \if KO
    /// <para>포트가 1~65535 범위를 벗어난 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the port is outside the range 1 through 65535.</para>
    /// \endif
    /// </exception>
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
