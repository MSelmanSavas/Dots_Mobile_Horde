using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Sirenix.OdinInspector.ShowInInspector]
    Entity _connectedPlayerEntity;
    EntityManager _entityManager;

    [SerializeField]
    SkeletonAnimation _skeletonAnimation;

    bool _wasMoving;
    bool _isMoving;

    private void Awake()
    {
        CreateConnectedEntity();
    }

    void CreateConnectedEntity()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _connectedPlayerEntity = _entityManager.CreateSingleton<PlayerTagComponent>();

        _entityManager.AddComponent<PlayerTagComponent>(_connectedPlayerEntity);
        _entityManager.AddComponent<PlayerControllerComponent>(_connectedPlayerEntity);
        _entityManager.SetComponentData(_connectedPlayerEntity, new PlayerControllerComponent
        {
            PlayerController = this,
        });

        _entityManager.AddComponent<PlayerMovementConfigComponent>(_connectedPlayerEntity);
        _entityManager.SetComponentData(_connectedPlayerEntity, new PlayerMovementConfigComponent
        {
            Speed = 15,
        });

        _entityManager.AddComponent<LocalTransform>(_connectedPlayerEntity);
        _entityManager.AddComponent<LocalToWorld>(_connectedPlayerEntity);

        _entityManager.AddComponent<EntityComponent_Health>(_connectedPlayerEntity);
        _entityManager.SetComponentData(_connectedPlayerEntity, new EntityComponent_Health
        {
            CurrentHealth = 100,
            MaxHealth = 100,
            IsDead = false,
        });

        _entityManager.AddComponent<PlayerMovementComponent>(_connectedPlayerEntity);


#if UNITY_EDITOR
        _entityManager.SetName(_connectedPlayerEntity, "Player");
#endif
    }

    private void LateUpdate()
    {
        CheckMovementStatus();
        SetPlayerDirection();
    }

    void CheckMovementStatus()
    {
        var playerMovement = _entityManager.GetComponentData<PlayerMovementComponent>(_connectedPlayerEntity);

        if (playerMovement.IsMoving && _wasMoving)
            return;

        if (!playerMovement.IsMoving && !_wasMoving)
            return;

        if (playerMovement.IsMoving && !_wasMoving)
            StartMoving();
        else if (!playerMovement.IsMoving && _wasMoving)
            StopMoving();

        _wasMoving = _isMoving;
    }

    void StartMoving()
    {
        _isMoving = true;
        _skeletonAnimation.AnimationState.SetAnimation(0, "run", true);
    }

    void StopMoving()
    {
        _isMoving = false;
        _skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
    }

    void SetPlayerDirection()
    {
        if (!_isMoving)
            return;

        bool isAnimationFlipped = _skeletonAnimation.skeleton.ScaleX < 0;
        var playerMovement = _entityManager.GetComponentData<PlayerMovementComponent>(_connectedPlayerEntity);
        float movementXDirection = Mathf.Sign(playerMovement.MovementVector.x);

        if (movementXDirection < 0f && isAnimationFlipped)
            return;

        if (movementXDirection > 0f && !isAnimationFlipped)
            return;

        if (movementXDirection < 0f && !isAnimationFlipped)
        {
            _skeletonAnimation.skeleton.ScaleX = -1f;
        }
        else if (movementXDirection > 0f && isAnimationFlipped)
        {
            _skeletonAnimation.skeleton.ScaleX = 1f;
        }
    }
}
