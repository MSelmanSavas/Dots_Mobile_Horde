using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BulletsConfigsAuthoring : MonoBehaviour
{
    [SerializeField]
    GameObject _bulletPrefab;

    [SerializeField]
    GameObject _rocketPrefab;
    
    class Baker : Baker<BulletsConfigsAuthoring>
    {
        public override void Bake(BulletsConfigsAuthoring authoring)
        {
            
        }
    }
}
