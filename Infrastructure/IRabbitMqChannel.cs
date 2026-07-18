namespace Dreamine.Communication.RabbitMQ.Infrastructure;

/// <summary>
/// \if KO
/// <para>메시지 버스가 토폴로지, 발행, 소비 및 확인에 사용하는 최소 RabbitMQ 채널 계약입니다.</para>
/// \endif
/// \if EN
/// <para>Defines the minimal RabbitMQ channel contract used for topology, publishing, consumption, and acknowledgements.</para>
/// \endif
/// </summary>
public interface IRabbitMqChannel : IDisposable
{
    /// <summary>
    /// \if KO
    /// <para>채널이 현재 열려 있는지 여부를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets whether the channel is currently open.</para>
    /// \endif
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// \if KO
    /// <para>지정한 형식과 수명 설정으로 Exchange를 선언합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Declares an exchange with the specified type and lifetime settings.</para>
    /// \endif
    /// <param name="exchange">
    /// \if KO
    /// <para>Exchange 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The exchange name.</para>
    /// \endif
    /// </param>
    /// <param name="type">
    /// \if KO
    /// <para>Exchange 형식입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The exchange type.</para>
    /// \endif
    /// </param>
    /// <param name="durable">
    /// \if KO
    /// <para>영속 선언 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether the exchange is durable.</para>
    /// \endif
    /// </param>
    /// <param name="autoDelete">
    /// \if KO
    /// <para>미사용 시 자동 삭제 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether the exchange is deleted when unused.</para>
    /// \endif
    /// </param>
    /// </summary>
    void ExchangeDeclare(
        string exchange,
        string type,
        bool durable,
        bool autoDelete);

    /// <summary>
    /// \if KO
    /// <para>지정한 수명 및 소유권 설정으로 Queue를 선언합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Declares a queue with the specified lifetime and ownership settings.</para>
    /// \endif
    /// <param name="queue">
    /// \if KO
    /// <para>Queue 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The queue name.</para>
    /// \endif
    /// </param>
    /// <param name="durable">
    /// \if KO
    /// <para>영속 선언 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether the queue is durable.</para>
    /// \endif
    /// </param>
    /// <param name="exclusive">
    /// \if KO
    /// <para>현재 연결 전용 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether the queue is exclusive to the current connection.</para>
    /// \endif
    /// </param>
    /// <param name="autoDelete">
    /// \if KO
    /// <para>미사용 시 자동 삭제 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether the queue is deleted when unused.</para>
    /// \endif
    /// </param>
    /// </summary>
    void QueueDeclare(
        string queue,
        bool durable,
        bool exclusive,
        bool autoDelete);

    /// <summary>
    /// \if KO
    /// <para>라우팅 키로 Queue를 Exchange에 바인딩합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Binds a queue to an exchange using a routing key.</para>
    /// \endif
    /// <param name="queue">
    /// \if KO
    /// <para>바인딩할 Queue 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The queue to bind.</para>
    /// \endif
    /// </param>
    /// <param name="exchange">
    /// \if KO
    /// <para>대상 Exchange 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The target exchange.</para>
    /// \endif
    /// </param>
    /// <param name="routingKey">
    /// \if KO
    /// <para>바인딩 라우팅 키입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The binding routing key.</para>
    /// \endif
    /// </param>
    /// </summary>
    void QueueBind(
        string queue,
        string exchange,
        string routingKey);

    /// <summary>
    /// \if KO
    /// <para>발행 작업에 사용할 새 메시지 속성을 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates message properties for a publish operation.</para>
    /// \endif
    /// <returns>
    /// \if KO
    /// <para>채널에 연결된 메시지 속성입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Message properties associated with the channel.</para>
    /// \endif
    /// </returns>
    /// </summary>
    IRabbitMqBasicProperties CreateBasicProperties();

    /// <summary>
    /// \if KO
    /// <para>메시지 본문과 속성을 지정한 Exchange 및 라우팅 키로 발행합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Publishes a body and properties to an exchange with a routing key.</para>
    /// \endif
    /// <param name="exchange">
    /// \if KO
    /// <para>대상 Exchange입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The target exchange.</para>
    /// \endif
    /// </param>
    /// <param name="routingKey">
    /// \if KO
    /// <para>발행 라우팅 키입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The publishing routing key.</para>
    /// \endif
    /// </param>
    /// <param name="mandatory">
    /// \if KO
    /// <para>라우팅 실패 시 반환을 요구할지 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether unroutable messages must be returned.</para>
    /// \endif
    /// </param>
    /// <param name="properties">
    /// \if KO
    /// <para>메시지 속성입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message properties.</para>
    /// \endif
    /// </param>
    /// <param name="body">
    /// \if KO
    /// <para>발행할 메시지 본문입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message body to publish.</para>
    /// \endif
    /// </param>
    /// </summary>
    void BasicPublish(
        string exchange,
        string routingKey,
        bool mandatory,
        IRabbitMqBasicProperties properties,
        ReadOnlyMemory<byte> body);

    /// <summary>
    /// \if KO
    /// <para>Queue의 배달 메시지를 처리하는 비동기 소비자를 시작합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Starts an asynchronous consumer for deliveries from a queue.</para>
    /// \endif
    /// <param name="queue">
    /// \if KO
    /// <para>소비할 Queue입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The queue to consume.</para>
    /// \endif
    /// </param>
    /// <param name="autoAck">
    /// \if KO
    /// <para>자동 확인 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether deliveries are acknowledged automatically.</para>
    /// \endif
    /// </param>
    /// <param name="onReceived">
    /// \if KO
    /// <para>배달 메시지를 처리할 비동기 콜백입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The asynchronous callback that processes deliveries.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>생성된 소비자 태그입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The generated consumer tag.</para>
    /// \endif
    /// </returns>
    /// </summary>
    string BasicConsume(
        string queue,
        bool autoAck,
        Func<RabbitMqDelivery, CancellationToken, Task> onReceived);

    /// <summary>
    /// \if KO
    /// <para>배달 메시지의 성공 처리를 확인합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Acknowledges successful processing of a delivery.</para>
    /// \endif
    /// <param name="deliveryTag">
    /// \if KO
    /// <para>확인할 배달 태그입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The delivery tag to acknowledge.</para>
    /// \endif
    /// </param>
    /// <param name="multiple">
    /// \if KO
    /// <para>이전 태그까지 함께 확인할지 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether to acknowledge all tags up to this one.</para>
    /// \endif
    /// </param>
    /// </summary>
    void BasicAck(ulong deliveryTag, bool multiple);

    /// <summary>
    /// \if KO
    /// <para>배달 메시지의 처리 실패를 확인합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Negatively acknowledges a failed delivery.</para>
    /// \endif
    /// <param name="deliveryTag">
    /// \if KO
    /// <para>실패 확인할 배달 태그입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The delivery tag to reject.</para>
    /// \endif
    /// </param>
    /// <param name="multiple">
    /// \if KO
    /// <para>이전 태그까지 함께 처리할지 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether to include all tags up to this one.</para>
    /// \endif
    /// </param>
    /// <param name="requeue">
    /// \if KO
    /// <para>메시지를 Queue에 다시 넣을지 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether to requeue the message.</para>
    /// \endif
    /// </param>
    /// </summary>
    void BasicNack(ulong deliveryTag, bool multiple, bool requeue);

    /// <summary>
    /// \if KO
    /// <para>단일 배달 메시지를 거부합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Rejects a single delivery.</para>
    /// \endif
    /// <param name="deliveryTag">
    /// \if KO
    /// <para>거부할 배달 태그입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The delivery tag to reject.</para>
    /// \endif
    /// </param>
    /// <param name="requeue">
    /// \if KO
    /// <para>메시지를 Queue에 다시 넣을지 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether to requeue the message.</para>
    /// \endif
    /// </param>
    /// </summary>
    void BasicReject(ulong deliveryTag, bool requeue);

    /// <summary>
    /// \if KO
    /// <para>지정한 소비자를 취소합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Cancels the specified consumer.</para>
    /// \endif
    /// <param name="consumerTag">
    /// \if KO
    /// <para>취소할 소비자 태그입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The consumer tag to cancel.</para>
    /// \endif
    /// </param>
    /// </summary>
    void BasicCancel(string consumerTag);

    /// <summary>
    /// \if KO
    /// <para>RabbitMQ 채널을 닫습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Closes the RabbitMQ channel.</para>
    /// \endif
    /// </summary>
    void Close();
}
