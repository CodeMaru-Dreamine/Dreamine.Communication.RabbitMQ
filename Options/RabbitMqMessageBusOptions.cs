namespace Dreamine.Communication.RabbitMQ.Options;

/// <summary>
/// \brief RabbitMQ 메시지 버스 설정입니다.
/// </summary>
public sealed class RabbitMqMessageBusOptions
{
    /// <summary>
    /// \brief RabbitMQ 서버 호스트입니다.
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// \brief RabbitMQ 서버 포트입니다.
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// \brief RabbitMQ 가상 호스트입니다.
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// \brief RabbitMQ 사용자 이름입니다.
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// \brief RabbitMQ 비밀번호입니다.
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// \brief 사용할 Exchange 이름입니다.
    /// </summary>
    public string ExchangeName { get; set; } = string.Empty;

    /// <summary>
    /// \brief 사용할 Queue 이름입니다.
    /// </summary>
    public string QueueName { get; set; } = string.Empty;

    /// <summary>
    /// \brief 기본 Routing Key입니다.
    /// </summary>
    public string RoutingKey { get; set; } = string.Empty;

    /// <summary>
    /// \brief Exchange 유형입니다.
    /// </summary>
    public string ExchangeType { get; set; } = "direct";

    /// <summary>
    /// \brief Exchange를 durable로 생성할지 여부입니다.
    /// </summary>
    public bool Durable { get; set; } = true;

    /// <summary>
    /// \brief Queue를 exclusive로 생성할지 여부입니다.
    /// </summary>
    public bool Exclusive { get; set; } = false;

    /// <summary>
    /// \brief Queue를 auto-delete로 생성할지 여부입니다.
    /// </summary>
    public bool AutoDelete { get; set; } = false;
}