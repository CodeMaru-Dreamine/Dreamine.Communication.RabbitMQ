namespace Dreamine.Communication.RabbitMQ.Infrastructure;

/// <summary>
/// \if KO
/// <para>RabbitMQ 채널 어댑터에서 메시지 버스로 전달되는 브로커 배달 정보를 나타냅니다.</para>
/// \endif
/// \if EN
/// <para>Represents broker delivery data passed from the RabbitMQ channel adapter to the message bus.</para>
/// \endif
/// </summary>
public sealed class RabbitMqDelivery
{
    /// <summary>
    /// \if KO
    /// <para>배달 태그, 라우팅 키 및 본문으로 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes an instance with a delivery tag, routing key, and body.</para>
    /// \endif
    /// </summary>
    /// <param name="deliveryTag">
    /// \if KO
    /// <para>브로커 배달 태그입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The broker delivery tag.</para>
    /// \endif
    /// </param>
    /// <param name="routingKey">
    /// \if KO
    /// <para>배달 라우팅 키이며 <see langword="null"/>이면 빈 문자열로 저장됩니다.</para>
    /// \endif
    /// \if EN
    /// <para>The delivery routing key; <see langword="null"/> is stored as an empty string.</para>
    /// \endif
    /// </param>
    /// <param name="body">
    /// \if KO
    /// <para>배달된 메시지 본문입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The delivered message body.</para>
    /// \endif
    /// </param>
    public RabbitMqDelivery(
        ulong deliveryTag,
        string routingKey,
        ReadOnlyMemory<byte> body)
    {
        DeliveryTag = deliveryTag;
        RoutingKey = routingKey ?? string.Empty;
        Body = body;
    }

    /// <summary>
    /// \if KO
    /// <para>브로커 배달 태그를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the broker delivery tag.</para>
    /// \endif
    /// </summary>
    public ulong DeliveryTag { get; }

    /// <summary>
    /// \if KO
    /// <para>배달 라우팅 키를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the delivery routing key.</para>
    /// \endif
    /// </summary>
    public string RoutingKey { get; }

    /// <summary>
    /// \if KO
    /// <para>배달된 메시지 본문을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the delivery body.</para>
    /// \endif
    /// </summary>
    public ReadOnlyMemory<byte> Body { get; }
}
