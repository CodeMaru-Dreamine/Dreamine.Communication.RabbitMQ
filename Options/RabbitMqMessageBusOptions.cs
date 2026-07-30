namespace Dreamine.Communication.RabbitMQ.Options;

/// <summary>
/// \if KO
/// <para>RabbitMQ 연결, 토폴로지, 발행 및 오류 처리 동작을 구성합니다.</para>
/// \endif
/// \if EN
/// <para>Configures RabbitMQ connection, topology, publishing, and error-handling behavior.</para>
/// \endif
/// </summary>
public sealed class RabbitMqMessageBusOptions
{
    /// <summary>
    /// \if KO
    /// <para>RabbitMQ 브로커 호스트 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the RabbitMQ broker host name.</para>
    /// \endif
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// \if KO
    /// <para>RabbitMQ 브로커 포트를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the RabbitMQ broker port.</para>
    /// \endif
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// \if KO
    /// <para>RabbitMQ 가상 호스트를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the RabbitMQ virtual host.</para>
    /// \endif
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// \if KO
    /// <para>브로커 인증 사용자 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the broker authentication user name.</para>
    /// \endif
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// \if KO
    /// <para>브로커 인증 비밀번호를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the broker authentication password.</para>
    /// \endif
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// \if KO
    /// <para>선언하고 사용할 Exchange 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the exchange name to declare and use.</para>
    /// \endif
    /// </summary>
    public string ExchangeName { get; set; } = "dreamine.default.exchange";

    /// <summary>
    /// \if KO
    /// <para>선언하고 사용할 Queue 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the queue name to declare and use.</para>
    /// \endif
    /// </summary>
    public string QueueName { get; set; } = "dreamine.default.queue";

    /// <summary>
    /// \if KO
    /// <para>기본 바인딩 및 발행 라우팅 키를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the default binding and publishing routing key.</para>
    /// \endif
    /// </summary>
    public string RoutingKey { get; set; } = "dreamine.default.route";

    /// <summary>
    /// \if KO
    /// <para>Exchange 형식을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the exchange type.</para>
    /// \endif
    /// </summary>
    public string ExchangeType { get; set; } = "direct";

    /// <summary>
    /// \if KO
    /// <para>Exchange와 Queue를 영속 항목으로 선언할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the exchange and queue are declared durable.</para>
    /// \endif
    /// </summary>
    public bool Durable { get; set; }

    /// <summary>
    /// \if KO
    /// <para>Queue를 현재 연결 전용으로 선언할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the queue is exclusive to the current connection.</para>
    /// \endif
    /// </summary>
    public bool Exclusive { get; set; }

    /// <summary>
    /// \if KO
    /// <para>미사용 시 Exchange와 Queue를 자동 삭제할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the exchange and queue are automatically deleted when unused.</para>
    /// \endif
    /// </summary>
    public bool AutoDelete { get; set; }

    /// <summary>
    /// \if KO
    /// <para>발행 메시지를 브로커 영속 메시지로 표시할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether published messages are marked as persistent.</para>
    /// \endif
    /// </summary>
    public bool PersistentMessages { get; set; }

    /// <summary>
    /// \if KO
    /// <para>처리기 실패 시 메시지를 Queue에 다시 넣을지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether a message is requeued after handler failure.</para>
    /// \endif
    /// </summary>
    public bool RequeueOnHandlerError { get; set; }
}
