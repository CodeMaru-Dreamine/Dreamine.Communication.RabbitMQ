using System;
using Dreamine.Communication.Abstractions.Exceptions;

namespace Dreamine.Communication.RabbitMQ.Exceptions;

/// <summary>
/// \if KO
/// <para>RabbitMQ 연결, 토폴로지 또는 메시지 처리 과정에서 발생한 통신 오류를 나타냅니다.</para>
/// \endif
/// \if EN
/// <para>Represents a communication error raised during RabbitMQ connection, topology, or message processing.</para>
/// \endif
/// </summary>
public sealed class RabbitMqCommunicationException : CommunicationException
{
    /// <summary>
    /// \if KO
    /// <para>기본 메시지로 새 RabbitMQ 통신 예외를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new RabbitMQ communication exception with the default message.</para>
    /// \endif
    /// </summary>
    public RabbitMqCommunicationException()
    {
    }

    /// <summary>
    /// \if KO
    /// <para>지정한 오류 메시지로 새 RabbitMQ 통신 예외를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new RabbitMQ communication exception with the specified error message.</para>
    /// \endif
    /// </summary>
    /// <param name="message">
    /// \if KO
    /// <para>오류 원인을 설명하는 메시지입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message that describes the cause of the error.</para>
    /// \endif
    /// </param>
    public RabbitMqCommunicationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// \if KO
    /// <para>지정한 오류 메시지와 내부 예외로 새 RabbitMQ 통신 예외를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new RabbitMQ communication exception with an error message and inner exception.</para>
    /// \endif
    /// </summary>
    /// <param name="message">
    /// \if KO
    /// <para>오류 원인을 설명하는 메시지입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message that describes the cause of the error.</para>
    /// \endif
    /// </param>
    /// <param name="innerException">
    /// \if KO
    /// <para>현재 오류의 원인이 된 예외입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The exception that caused the current error.</para>
    /// \endif
    /// </param>
    public RabbitMqCommunicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
