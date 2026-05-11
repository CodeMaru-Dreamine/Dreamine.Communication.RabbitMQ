# Dreamine.Communication.RabbitMQ

`Dreamine.Communication.RabbitMQ`는 Dreamine Communication 계열 패키지의 일부입니다.

이 패키지는 RabbitMQ 어댑터의 경계를 정의합니다. 현재 단계에서는 RabbitMQ를 선택형 Broker Adapter로 분리된 상태로 유지합니다.

[➡️ English Version](./README.md)

## Description

RabbitMQ message bus adapter boundary for Dreamine Communication.

## 주요 기능

- RabbitMQ 메시지 버스 경계
- RabbitMQ 옵션 모델
- RabbitMQ 통신 예외
- IMessageBus 기반 어댑터 뼈대

## 설계 원칙

- 구체 통신 구현체를 상위 레이어와 분리합니다.
- `Dreamine.Communication.Abstractions`의 계약에 의존합니다.
- 패키지 책임을 작고 명확하게 유지합니다.
- 단방향 의존성 흐름을 유지합니다.
- 향후 어댑터를 추가해도 애플리케이션 로직을 변경하지 않도록 합니다.

## 패키지 역할

```text
Dreamine.Communication.Abstractions
    ↑
Dreamine.Communication.RabbitMQ
```

## 의존성

- `Dreamine.Communication.Abstractions`

## 대상 프레임워크

```text
net8.0
```

## 참고

RabbitMQ.Client 연동은 이후 구현 단계로 의도적으로 보류되어 있습니다.

## 관련 패키지

- `Dreamine.Communication.Abstractions`
- `Dreamine.Communication.Core`
- `Dreamine.Communication.Sockets`
- `Dreamine.Communication.Serial`
- `Dreamine.Communication.RabbitMQ`
- `Dreamine.Communication.FullKit`
- `Dreamine.Communication.Wpf`

## 라이선스

이 프로젝트는 MIT 라이선스를 따릅니다.
