using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class PlayerMovementSystem : MonoBehaviour
    {
        private readonly struct MovementStepInput
        {
            public PlayerInputState InputState { get; }

            public PlayerCollisionState CollisionState { get; }

            public Vector3 CurrentVelocity { get; }

            public MovementStepInput(
                PlayerInputState inputState,
                PlayerCollisionState collisionState,
                Vector3 currentVelocity)
            {
                InputState = inputState;
                CollisionState = collisionState;
                CurrentVelocity = currentVelocity;
            }
        }

        private struct MovementCalculation
        {
            public float HorizontalSpeed { get; set; }

            public float VerticalSpeed { get; set; }

            public bool IsJumpStarted { get; set; }

            public bool IsMomentumLanding { get; set; }

            public bool IsNormalLanding { get; set; }

            public E_PlayerMovementState LandingState { get; set; }
        }

        [SerializeField] private PlayerInputSystem _playerInputSystem;
        [SerializeField] private PlayerControllerSystem _playerControllerSystem;
        [SerializeField] private CollisionSystem _collisionSystem;
        [SerializeField] private RuntimeDataSystem _runtimeDataSystem;
        [SerializeField] private JumpFeature _jumpFeature;
        [SerializeField] private MomentumLandingFeature _momentumLandingFeature;
        [SerializeField] private NormalLandingFeature _normalLandingFeature;
        [SerializeField] private float _moveSpeed = 8.0f;
        [SerializeField] private float _groundAcceleration = 50.0f;
        [SerializeField] private float _airAcceleration = 25.0f;
        [SerializeField] private float _maximumHorizontalSpeed = 14.0f;
        [SerializeField] private float _gravityAcceleration = 25.0f;

        private PlayerMovementRuntimeData _runtimeData;
        private E_PlayerMovementState _movementState;
        private bool _isRunning;
        private bool _isJumpSequenceActive;
        private bool _hasJumpLeftGround;
        private bool _isLastLandingMomentum;

        public bool IsRunning => _isRunning;

        private void FixedUpdate()
        {
            if (!_isRunning)
            {
                return;
            }

            ProcessMovementStep(Time.fixedDeltaTime);
        }

        public bool Initialize()
        {
            if (!HasRequiredReferences())
            {
                _isRunning = false;
                return false;
            }

            GameRuntimeData gameRuntimeData = _runtimeDataSystem.GetRuntimeData();

            if (gameRuntimeData == null)
            {
                Debug.LogError("[PlayerMovementSystem] Runtime Data does not exist.");
                _isRunning = false;
                return false;
            }

            if (!_playerControllerSystem.IsInitialized ||
                !_collisionSystem.IsInitialized)
            {
                Debug.LogError(
                    "[PlayerMovementSystem] Controller and Collision System must be initialized first.");
                _isRunning = false;
                return false;
            }

            _runtimeData = gameRuntimeData.PlayerMovementRuntimeData;
            _jumpFeature.Initialize();
            _momentumLandingFeature.Initialize();
            _normalLandingFeature.Initialize();

            _collisionSystem.RefreshCollisionState();
            PlayerCollisionState collisionState = _collisionSystem.GetCollisionState();
            _movementState = collisionState.IsGrounded
                ? E_PlayerMovementState.Grounded
                : E_PlayerMovementState.Airborne;
            _isJumpSequenceActive = false;
            _hasJumpLeftGround = false;
            _isLastLandingMomentum = false;
            _isRunning = true;

            UpdateRuntimeData(
                _playerControllerSystem.GetVelocity(),
                collisionState);

            return true;
        }

        public void StopMovement()
        {
            _isRunning = false;
            _movementState = E_PlayerMovementState.None;
            _isJumpSequenceActive = false;
            _hasJumpLeftGround = false;
            _isLastLandingMomentum = false;

            _jumpFeature.Initialize();
            _momentumLandingFeature.Initialize();
            _normalLandingFeature.Initialize();
            _playerControllerSystem.StopMovement();

            if (_runtimeData != null)
            {
                _runtimeData.Initialize();
            }
        }

        private void ProcessMovementStep(float deltaTime)
        {
            MovementStepInput stepInput = GatherMovementStepInput();
            PlayerMovementResult movementResult = CalculateMovementResult(
                stepInput,
                deltaTime);

            ApplyMovementResult(movementResult, stepInput.CollisionState);
            _playerInputSystem.ConsumeTransientInput();
        }

        private MovementStepInput GatherMovementStepInput()
        {
            PlayerInputState inputState = _playerInputSystem.GetInputState();

            _collisionSystem.RefreshCollisionState();

            return new MovementStepInput(
                inputState,
                _collisionSystem.GetCollisionState(),
                _playerControllerSystem.GetVelocity());
        }

        private PlayerMovementResult CalculateMovementResult(
            in MovementStepInput stepInput,
            float deltaTime)
        {
            NormalizeMovementState(stepInput.CollisionState);
            _jumpFeature.UpdateCoyoteTime(_movementState, deltaTime);

            MovementCalculation calculation = CreateInitialCalculation(
                stepInput,
                deltaTime);

            CalculateJumpAndGravity(stepInput, deltaTime, ref calculation);
            UpdateAirborneProgress(stepInput.CollisionState);
            UpdateMomentumLanding(stepInput, deltaTime, calculation.VerticalSpeed);
            ResolveLanding(stepInput.CollisionState, ref calculation);

            return CreateMovementResult(calculation);
        }

        private MovementCalculation CreateInitialCalculation(
            in MovementStepInput stepInput,
            float deltaTime)
        {
            return new MovementCalculation
            {
                HorizontalSpeed = PlayerMovementMath.CalculateHorizontalSpeed(
                    stepInput.CurrentVelocity.x,
                    stepInput.InputState.HorizontalInput,
                    stepInput.CollisionState.IsGrounded,
                    deltaTime,
                    _moveSpeed,
                    _groundAcceleration,
                    _airAcceleration,
                    _maximumHorizontalSpeed),
                VerticalSpeed = stepInput.CurrentVelocity.y,
                LandingState = E_PlayerMovementState.None
            };
        }

        private void CalculateJumpAndGravity(
            in MovementStepInput stepInput,
            float deltaTime,
            ref MovementCalculation calculation)
        {
            calculation.IsJumpStarted = _jumpFeature.TryStartJump(
                stepInput.InputState,
                _gravityAcceleration,
                out float jumpVerticalSpeed);

            if (calculation.IsJumpStarted)
            {
                calculation.VerticalSpeed = jumpVerticalSpeed;
                _movementState = E_PlayerMovementState.Airborne;
                _isJumpSequenceActive = true;
                _hasJumpLeftGround = !stepInput.CollisionState.IsGrounded;
                _momentumLandingFeature.BeginJump();
                _normalLandingFeature.BeginJump();
                return;
            }

            if (!stepInput.CollisionState.IsGrounded)
            {
                calculation.VerticalSpeed -=
                    Mathf.Max(0.0f, _gravityAcceleration) * deltaTime;
            }
        }

        private void UpdateAirborneProgress(
            in PlayerCollisionState collisionState)
        {
            if (_movementState == E_PlayerMovementState.Airborne &&
                !collisionState.IsGrounded)
            {
                _hasJumpLeftGround = true;
            }
        }

        private void UpdateMomentumLanding(
            in MovementStepInput stepInput,
            float deltaTime,
            float verticalSpeed)
        {
            _momentumLandingFeature.UpdateWindow(
                verticalSpeed,
                stepInput.CollisionState,
                deltaTime);
            _momentumLandingFeature.BufferInput(stepInput.InputState);
        }

        private void ResolveLanding(
            in PlayerCollisionState collisionState,
            ref MovementCalculation calculation)
        {
            bool canResolveLanding =
                !calculation.IsJumpStarted &&
                _hasJumpLeftGround &&
                calculation.VerticalSpeed <= 0.0f;

            if (!canResolveLanding)
            {
                return;
            }

            calculation.IsMomentumLanding =
                _momentumLandingFeature.TryCompleteLanding(
                    collisionState,
                    calculation.HorizontalSpeed,
                    out float landingHorizontalSpeed);
            calculation.IsNormalLanding =
                _normalLandingFeature.TryCompleteLanding(
                    collisionState,
                    calculation.IsMomentumLanding);

            if (!calculation.IsMomentumLanding &&
                !calculation.IsNormalLanding)
            {
                return;
            }

            calculation.HorizontalSpeed = landingHorizontalSpeed;
            calculation.VerticalSpeed = 0.0f;
            calculation.LandingState = calculation.IsMomentumLanding
                ? E_PlayerMovementState.MomentumLanding
                : E_PlayerMovementState.NormalLanding;
            _movementState = calculation.LandingState;
            _isJumpSequenceActive = false;
            _hasJumpLeftGround = false;
            _isLastLandingMomentum = calculation.IsMomentumLanding;
            _jumpFeature.CompleteLanding();
        }

        private PlayerMovementResult CreateMovementResult(
            in MovementCalculation calculation)
        {
            Vector3 resultVelocity = new Vector3(
                calculation.HorizontalSpeed,
                calculation.VerticalSpeed,
                0.0f);

            return new PlayerMovementResult(
                resultVelocity,
                _movementState,
                calculation.IsJumpStarted,
                calculation.IsMomentumLanding || calculation.IsNormalLanding,
                calculation.LandingState);
        }

        private void ApplyMovementResult(
            in PlayerMovementResult movementResult,
            in PlayerCollisionState collisionState)
        {
            _playerControllerSystem.ApplyMovement(movementResult);
            UpdateRuntimeData(movementResult.Velocity, collisionState);
        }

        private void NormalizeMovementState(in PlayerCollisionState collisionState)
        {
            if (_movementState == E_PlayerMovementState.MomentumLanding ||
                _movementState == E_PlayerMovementState.NormalLanding)
            {
                _movementState = collisionState.IsGrounded
                    ? E_PlayerMovementState.Grounded
                    : E_PlayerMovementState.Airborne;
                return;
            }

            if (_movementState == E_PlayerMovementState.Grounded &&
                !collisionState.IsGrounded)
            {
                _movementState = E_PlayerMovementState.Airborne;
            }

            if (_movementState == E_PlayerMovementState.Airborne &&
                collisionState.IsGrounded &&
                !_isJumpSequenceActive)
            {
                _movementState = E_PlayerMovementState.Grounded;
            }
        }

        private void UpdateRuntimeData(
            Vector3 velocity,
            in PlayerCollisionState collisionState)
        {
            _runtimeData.UpdateState(
                _movementState,
                velocity.x,
                velocity.y,
                collisionState.IsGrounded,
                _momentumLandingFeature.IsWindowActive,
                _isLastLandingMomentum);
        }

        private bool HasRequiredReferences()
        {
            if (_playerInputSystem == null ||
                _playerControllerSystem == null ||
                _collisionSystem == null ||
                _runtimeDataSystem == null ||
                _jumpFeature == null ||
                _momentumLandingFeature == null ||
                _normalLandingFeature == null)
            {
                Debug.LogError(
                    "[PlayerMovementSystem] Required System or Feature reference is missing.");
                return false;
            }

            return true;
        }
    }
}
