namespace Dreamine.Communication.RabbitMQ.Options;

/// <summary>
/// \brief RabbitMQ 메시지 버스 설정입니다.
/// </summary>
public sealed class RabbitMqMessageBusOptions
{
    /// <summary>
    /// \brief RabbitMQ Host 이름입니다.
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// \brief RabbitMQ Port입니다.
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// \brief RabbitMQ VirtualHost입니다.
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
    /// \brief RabbitMQ Exchange 이름입니다.
    /// </summary>
    public string ExchangeName { get; set; } = "dreamine.sample.exchange";

    /// <summary>
    /// \brief RabbitMQ Queue 이름입니다.
    /// </summary>
    public string QueueName { get; set; } = "dreamine.sample.queue";

    /// <summary>
    /// \brief RabbitMQ RoutingKey입니다.
    /// </summary>
    public string RoutingKey { get; set; } = "dreamine.sample.route";
}