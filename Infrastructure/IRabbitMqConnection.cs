namespace Dreamine.Communication.RabbitMQ.Infrastructure;

/// <summary>
/// \if KO
/// <para>메시지 버스가 사용하는 최소 RabbitMQ 연결 계약입니다.</para>
/// \endif
/// \if EN
/// <para>Defines the minimal RabbitMQ connection contract used by the message bus.</para>
/// \endif
/// </summary>
public interface IRabbitMqConnection : IDisposable
{
    /// <summary>
    /// \if KO
    /// <para>연결이 현재 열려 있는지 여부를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets whether the connection is currently open.</para>
    /// \endif
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// \if KO
    /// <para>브로커 작업에 사용할 채널을 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a channel for broker operations.</para>
    /// \endif
    /// <returns>
    /// \if KO
    /// <para>새 RabbitMQ 채널입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A new RabbitMQ channel.</para>
    /// \endif
    /// </returns>
    /// </summary>
    IRabbitMqChannel CreateChannel();

    /// <summary>
    /// \if KO
    /// <para>RabbitMQ 연결을 닫습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Closes the RabbitMQ connection.</para>
    /// \endif
    /// </summary>
    void Close();
}
