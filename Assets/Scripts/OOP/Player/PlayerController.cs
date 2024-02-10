using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Sirenix.OdinInspector.ShowInInspector]
    Entity _connectedPlayerEntity;
    EntityManager _entityManager;

    private void Awake()
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

#if UNITY_EDITOR
        _entityManager.SetName(_connectedPlayerEntity, "Player");
#endif
    }
}
