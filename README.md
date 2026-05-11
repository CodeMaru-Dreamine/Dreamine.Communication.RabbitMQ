# Dreamine.Communication.RabbitMQ

`Dreamine.Communication.RabbitMQ` is part of the Dreamine Communication package family.

This package defines the RabbitMQ adapter boundary. The current stage keeps RabbitMQ isolated as an optional broker adapter.

[➡️ 한국어 문서 보기](README_ko.md)

## Description

RabbitMQ message bus adapter boundary for Dreamine Communication.

## Features

- RabbitMQ message bus boundary
- RabbitMQ options model
- RabbitMQ communication exception
- IMessageBus-based adapter skeleton

## Design Principles

- Keep concrete transport implementations isolated from upper layers.
- Depend on `Dreamine.Communication.Abstractions` contracts.
- Keep package responsibilities small and explicit.
- Preserve one-way dependency flow.
- Allow future adapters to be added without changing application logic.

## Package Role

```text
Dreamine.Communication.Abstractions
    ↑
Dreamine.Communication.RabbitMQ
```

## Dependencies

- `Dreamine.Communication.Abstractions`

## Target Framework

```text
net8.0
```

## Note

RabbitMQ.Client integration is intentionally reserved for a later implementation stage.

## Related Packages

- `Dreamine.Communication.Abstractions`
- `Dreamine.Communication.Core`
- `Dreamine.Communication.Sockets`
- `Dreamine.Communication.Serial`
- `Dreamine.Communication.RabbitMQ`
- `Dreamine.Communication.FullKit`
- `Dreamine.Communication.Wpf`

## License

This project is licensed under the MIT License.
