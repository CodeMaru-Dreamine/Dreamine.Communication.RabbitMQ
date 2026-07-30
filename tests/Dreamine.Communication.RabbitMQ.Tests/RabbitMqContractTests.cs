using Dreamine.Communication.RabbitMQ.Buses;
using Dreamine.Communication.RabbitMQ.Exceptions;
using Dreamine.Communication.RabbitMQ.Infrastructure;
using Dreamine.Communication.RabbitMQ.Options;
using Xunit;

namespace Dreamine.Communication.RabbitMQ.Tests;

public sealed class RabbitMqContractTests
{
    [Fact]
    public void OptionsExposeSafeDefaults()
    {
        var options = new RabbitMqMessageBusOptions();

        Assert.Equal("localhost", options.HostName);
        Assert.Equal(5672, options.Port);
        Assert.Equal("/", options.VirtualHost);
        Assert.Equal("direct", options.ExchangeType);
        Assert.False(options.PersistentMessages);
    }

    [Fact]
    public void DeliveryNormalizesNullRoutingKey()
    {
        var body = new byte[] { 1, 2, 3 };
        var delivery = new RabbitMqDelivery(42, null!, body);

        Assert.Equal(42UL, delivery.DeliveryTag);
        Assert.Equal(string.Empty, delivery.RoutingKey);
        Assert.Equal(body, delivery.Body.ToArray());
    }

    [Fact]
    public void ExceptionPreservesMessageAndInnerException()
    {
        var inner = new InvalidOperationException("broker");
        var error = new RabbitMqCommunicationException("failed", inner);

        Assert.Equal("failed", error.Message);
        Assert.Same(inner, error.InnerException);
    }

    [Fact]
    public void BusRejectsInvalidPort()
    {
        var options = new RabbitMqMessageBusOptions { Port = 0 };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new RabbitMqMessageBus(options, new UnusedConnectionFactory()));
    }

    [Fact]
    public void BusRejectsMissingHost()
    {
        var options = new RabbitMqMessageBusOptions { HostName = "" };

        Assert.Throws<ArgumentException>(() =>
            new RabbitMqMessageBus(options, new UnusedConnectionFactory()));
    }

    private sealed class UnusedConnectionFactory : IRabbitMqConnectionFactory
    {
        public IRabbitMqConnection CreateConnection(RabbitMqMessageBusOptions options) =>
            throw new InvalidOperationException("Connection should not be created by these tests.");
    }
}
