# 목적

프로젝트에서 사용하는 코드 작성 규칙을 정의한다.

프로젝트 전체에서 동일하게 적용되는 코드 스타일을 정의한다.

코드의 일관성과 유지보수성을 위한 기준을 정의한다.

---

# 적용 범위

이 문서의 규칙은 프로젝트의 모든 코드에 적용한다.

예외가 필요한 경우 별도의 Rule 문서에서 정의한다.

---

# 네이밍 규칙

## Class

PascalCase를 사용한다.

예시

```text
PlayerMovementSystem
```

---

## Struct

PascalCase를 사용한다.

예시

```text
PlayerState
```

---

## Interface

`I` 접두어와 PascalCase를 사용한다.

예시

```text
IMovementProvider
```

---

## Enum

`E_` 접두어와 PascalCase를 사용한다.

예시

```text
E_PlayerState
```

---

## Field

Private Field는 `_` 접두어와 camelCase를 사용한다.

예시

```text
_player
_moveSpeed
```

---

## Property

PascalCase를 사용한다.

예시

```text
CurrentSpeed
```

---

## Method

PascalCase를 사용한다.

동사로 시작하는 이름을 사용한다.

예시

```text
Move
Initialize
Reset
```

---

## Local Variable

camelCase를 사용한다.

예시

```text
moveDirection
currentSpeed
```

---

## Bool Variable

`Is`, `Has`, `Can` 접두어를 사용한다.

예시

```text
_isGrounded
_hasTarget
_canJump

IsGrounded
HasTarget
CanJump
```

---

## Constant

PascalCase를 사용한다.

예시

```text
MaxSpeed
```

---

## Namespace

PascalCase를 사용한다.

프로젝트 구조를 반영한다.

예시

```text
Project.Player
Project.Stage
```

---

# 파일 구성 규칙

파일명과 Class 이름은 동일해야 한다.

하나의 `.cs` 파일에는 Class를 하나만 정의한다.

Struct, Enum, Interface는 필요한 경우 같은 파일에 함께 정의할 수 있다.

Class를 둘 이상 하나의 파일에 정의하지 않는다.

---

# 클래스 구성 규칙

멤버는 아래 순서를 따른다.

```text
Constant

Field

Property

Constructor

Awake

OnEnable

Start

Update

LateUpdate

FixedUpdate

OnDisable

OnDestroy

Public Method

Private Method
```

접근 제한자는 가능한 가장 좁은 범위를 사용한다.

외부에서 변경할 필요가 없는 Member는 `private`으로 선언한다.

Field를 `public`으로 노출하는 대신 Property 또는 Method를 통해 접근한다.

Static은 상태를 가지지 않는 기능에만 사용한다.

Partial Class는 특별한 이유가 없는 한 사용하지 않는다.

---

# 코드 작성 규칙

함수는 하나의 책임만 수행한다.

Method의 매개변수는 필요한 최소 개수만 사용한다.

Method의 반환값은 하나의 의미만 가진다.

매직 넘버를 직접 사용하지 않는다.

Null이 가능한 경우 명확하게 처리한다.

예외 상황은 무시하지 않는다.

객체 생성 후 반드시 초기화가 필요한 구조는 `Initialize()`를 사용한다.

생성자와 `Initialize()`의 책임을 혼합하지 않는다.

주석은 "무엇을 하는지"보다 "왜 필요한지"를 설명한다.

불필요한 주석은 작성하지 않는다.

---

# 직렬화 규칙

Inspector에 노출이 필요한 경우에만 `[SerializeField]`를 사용한다.

Field를 Inspector에 노출해야 하는 경우 `public` 대신 `[SerializeField] private`을 우선 사용한다.

`public` Field는 외부에서 접근이 반드시 필요한 경우에만 사용한다.

`[SerializeField]`와 `public` Field를 동시에 사용하지 않는다.

---

# 의존성 관리 규칙

기존 구조를 우선 재사용한다.

불필요한 의존성을 추가하지 않는다.

순환 참조를 만들지 않는다.

결합도를 최소화하는 방향으로 구조를 유지한다.

---

# 성능 작성 규칙

불필요한 최적화를 수행하지 않는다.

성능 문제가 확인된 경우에만 최적화를 수행한다.

동일한 연산을 반복 수행하지 않도록 작성한다.

반복적으로 GC Allocation이 발생하는 구조를 지양한다.

---

# 금지 사항

동일한 책임을 여러 Class에 분산하여 구현하지 않는다.

동일한 목적의 구조를 중복 생성하지 않는다.

사용하지 않는 코드를 남겨두지 않는다.

주석 처리된 코드를 유지하지 않는다.

경고를 무시한 채 코드를 작성하지 않는다.

컴파일 오류가 존재하는 상태에서 작업 완료를 선언하지 않는다.
