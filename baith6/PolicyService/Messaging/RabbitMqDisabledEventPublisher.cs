using System;
using System.Threading.Tasks;

namespace PolicyService.Messaging;

public sealed class RabbitMqDisabledEventPublisher : IEventPublisher
{
    public Task PublishMessage<T>(T message)
    {
        throw new ApplicationException(
            "RabbitMQ is disabled for this local run; policy lifecycle events cannot be published.");
    }
}
