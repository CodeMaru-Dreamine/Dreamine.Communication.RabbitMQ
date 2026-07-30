namespace Dreamine.Communication.RabbitMQ.Infrastructure;

/// <summary>
/// \if KO
/// <para>브로커 없이 메시지 버스를 테스트할 수 있도록 RabbitMQ 메시지 속성을 추상화합니다.</para>
/// \endif
/// \if EN
/// <para>Abstracts RabbitMQ message properties so the bus can be tested without a broker.</para>
/// \endif
/// </summary>
public interface IRabbitMqBasicProperties
{
    /// <summary>
    /// \if KO
    /// <para>브로커가 메시지를 영속화할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the broker persists the message.</para>
    /// \endif
    /// </summary>
    bool Persistent { get; set; }

    /// <summary>
    /// \if KO
    /// <para>메시지 콘텐츠 형식을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the message content type.</para>
    /// \endif
    /// </summary>
    string? ContentType { get; set; }

    /// <summary>
    /// \if KO
    /// <para>논리적 메시지 형식을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the logical message type.</para>
    /// \endif
    /// </summary>
    string? Type { get; set; }
}
