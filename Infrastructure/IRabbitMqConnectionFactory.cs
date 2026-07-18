using Dreamine.Communication.RabbitMQ.Options;

namespace Dreamine.Communication.RabbitMQ.Infrastructure;

/// <summary>
/// \if KO
/// <para>Dreamine RabbitMQ 설정에서 브로커 연결을 생성하는 계약입니다.</para>
/// \endif
/// \if EN
/// <para>Defines creation of broker connections from Dreamine RabbitMQ options.</para>
/// \endif
/// </summary>
public interface IRabbitMqConnectionFactory
{
    /// <summary>
    /// \if KO
    /// <para>지정한 연결 및 토폴로지 설정으로 RabbitMQ 연결을 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a RabbitMQ connection from the specified connection and topology options.</para>
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
    /// <returns>
    /// \if KO
    /// <para>생성된 RabbitMQ 연결 추상화입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The created RabbitMQ connection abstraction.</para>
    /// \endif
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="options"/>가 <see langword="null"/>인 경우 발생할 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown when <paramref name="options"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    IRabbitMqConnection CreateConnection(RabbitMqMessageBusOptions options);
}
