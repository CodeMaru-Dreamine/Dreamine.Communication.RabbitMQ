using System;
using Dreamine.Communication.Abstractions.Exceptions;

namespace Dreamine.Communication.RabbitMQ.Exceptions;

/// <summary>
/// \brief RabbitMQ 통신 계층에서 발생하는 예외입니다.
/// </summary>
public sealed class RabbitMqCommunicationException : CommunicationException
{
    /// <summary>
    /// \brief RabbitMqCommunicationException 클래스의 새 인스턴스를 초기화합니다.
    /// </summary>
    public RabbitMqCommunicationException()
    {
    }

    /// <summary>
    /// \brief 지정한 오류 메시지를 사용하여 RabbitMqCommunicationException 클래스의 새 인스턴스를 초기화합니다.
    /// </summary>
    /// <param name="message">오류 메시지입니다.</param>
    public RabbitMqCommunicationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// \brief 지정한 오류 메시지와 내부 예외를 사용하여 RabbitMqCommunicationException 클래스의 새 인스턴스를 초기화합니다.
    /// </summary>
    /// <param name="message">오류 메시지입니다.</param>
    /// <param name="innerException">내부 예외입니다.</param>
    public RabbitMqCommunicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}