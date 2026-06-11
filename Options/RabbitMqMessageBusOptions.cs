namespace Dreamine.Communication.RabbitMQ.Options;

/// <summary>
/// RabbitMQ 메시지 버스 설정입니다.
/// </summary>
public sealed class RabbitMqMessageBusOptions
{
    /// <summary>
    /// RabbitMQ Host 이름입니다.
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// RabbitMQ Port입니다.
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// RabbitMQ VirtualHost입니다.
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// RabbitMQ 사용자 이름입니다.
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// RabbitMQ 비밀번호입니다.
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// RabbitMQ Exchange 이름입니다.
    /// </summary>
    public string ExchangeName { get; set; } = "dreamine.default.exchange";

    /// <summary>
    /// RabbitMQ Queue 이름입니다.
    /// </summary>
    public string QueueName { get; set; } = "dreamine.default.queue";

    /// <summary>
    /// RabbitMQ RoutingKey입니다.
    /// </summary>
    public string RoutingKey { get; set; } = "dreamine.default.route";

    /// <summary>
    /// RabbitMQ Exchange 타입입니다.
    /// </summary>
    public string ExchangeType { get; set; } = "direct";

    /// <summary>
    /// Exchange와 Queue를 Durable로 선언할지 여부입니다.
    /// </summary>
    public bool Durable { get; set; }

    /// <summary>
    /// Queue를 Exclusive로 선언할지 여부입니다.
    /// </summary>
    public bool Exclusive { get; set; }

    /// <summary>
    /// Exchange와 Queue를 AutoDelete로 선언할지 여부입니다.
    /// </summary>
    public bool AutoDelete { get; set; }

    /// <summary>
    /// 발행 메시지를 Persistent로 설정할지 여부입니다.
    /// </summary>
    public bool PersistentMessages { get; set; }

    /// <summary>
    /// 처리 실패 시 메시지를 Queue에 다시 넣을지 여부입니다.
    /// </summary>
    public bool RequeueOnHandlerError { get; set; }
}
