using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Spine.Unity;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Physics;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Sirenix.OdinInspector.ShowInInspector]
    Entity _connectedPlayerEntity;
    EntityManager _entityManager;

    [SerializeField]
    SubScene _generalSubScene;

    [Sirenix.OdinInspector.ShowInInspector]
    Entity _subSceneEntity;

    [SerializeField]
    EntitySceneReference _subSceneReference;

    [SerializeField]
    SkeletonAnimation _skeletonAnimation;

    [SerializeField]
    TMPro.TextMeshProUGUI _playerHealthText;

    bool _wasMoving;
    bool _isMoving;
    bool _hasGotConnectedEntity;

    private IEnumerator Start()
    {
        _hasGotConnectedEntity = false;

        //_subSceneEntity = SceneSystem.LoadSceneAsync(World.DefaultGameObjectInjectionWorld.Unmanaged, _subSceneReference);

        foreach (var system in World.DefaultGameObjectInjectionWorld.Systems)
        {
            system.Enabled = true;
        }

        //Very bad hack to make controller wait entity systems to be loaded.
        yield return new WaitForSeconds(0.25f);
        GetConnectedEntity();
    }

    bool GetConnectedEntity()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var query = _entityManager.CreateEntityQuery(new ComponentType[] { typeof(PlayerTagComponent) });

        if (!query.TryGetSingletonEntity<PlayerTagComponent>(out _connectedPlayerEntity))
        {
            Debug.LogError("Cannot find player entity");
            return false;
        }

        _entityManager.AddComponentObject(_connectedPlayerEntity, new PlayerControllerComponent
        {
            PlayerController = this,
        });

        return true;
    }

    private void LateUpdate()
    {
        if (!TryGetConnectedEntity())
            return;

        CheckMovementStatus();
        SetPlayerDirection();
        CheckPlayerHealth();
    }

    private bool TryGetConnectedEntity()
    {
        if (_hasGotConnectedEntity)
            return true;

        try
        {
            _hasGotConnectedEntity = GetConnectedEntity();

            if (_hasGotConnectedEntity)
            {
                foreach (var system in World.DefaultGameObjectInjectionWorld.Systems)
                {
                    system.Enabled = true;
                }
            }
            return _hasGotConnectedEntity;
        }
        catch
        {
            return false;
        }
    }

    void CheckMovementStatus()
    {
        try
        {
            if (!_entityManager.Exists(_connectedPlayerEntity))
                return;

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
        catch
        {

        }
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

    void CheckPlayerHealth()
    {
        try
        {
            if (!_entityManager.Exists(_connectedPlayerEntity))
                return;

            var playerHealth = _entityManager.GetComponentData<EntityComponent_Health>(_connectedPlayerEntity);

            _playerHealthText.text = playerHealth.CurrentHealth.ToString();

            if (!playerHealth.IsDead)
                return;

            CleanAndRestartECS();
        }
        catch
        {

        }
    }

    //Very bad hack for restarting game scene! But i didn't had time to implement a cleaner solution :/
    public void CleanAndRestartECS()
    {
        var defaultWorld = World.DefaultGameObjectInjectionWorld;
        defaultWorld.EntityManager.CompleteAllTrackedJobs();

        foreach (var system in defaultWorld.Systems)
        {
            system.Enabled = false;
        }

        // DefaultWorldInitialization.Initialize("Default World", false);

        // if (!ScriptBehaviourUpdateOrder.IsWorldInCurrentPlayerLoop(World.DefaultGameObjectInjectionWorld))
        // {
        //     ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(World.DefaultGameObjectInjectionWorld);
        // }

        SceneSystem.UnloadScene(defaultWorld.Unmanaged, _subSceneEntity, SceneSystem.UnloadParameters.DestroyMetaEntities);
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }
}
