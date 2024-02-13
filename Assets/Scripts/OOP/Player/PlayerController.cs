using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Spine.Unity;
using Unity.Entities;
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

    [SerializeField]
    SkeletonAnimation _skeletonAnimation;

    [SerializeField]
    TMPro.TextMeshProUGUI _playerHealthText;

    bool _wasMoving;
    bool _isMoving;

    private IEnumerator Start()
    {
        //Very bad hack to make controller wait entity systems to be loaded.
        yield return new WaitForSeconds(0.25f);
        GetConnectedEntity();
    }

    void GetConnectedEntity()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var query = _entityManager.CreateEntityQuery(new ComponentType[] { typeof(PlayerTagComponent) });

        if (!query.TryGetSingletonEntity<PlayerTagComponent>(out _connectedPlayerEntity))
        {
            Debug.LogError("Cannot find player entity");
        }

        _entityManager.AddComponentObject(_connectedPlayerEntity, new PlayerControllerComponent
        {
            PlayerController = this,
        });
    }

    private void LateUpdate()
    {
        CheckMovementStatus();
        SetPlayerDirection();
        CheckPlayerHealth();
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

    public void CleanAndRestartECS()
    {
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }
}
