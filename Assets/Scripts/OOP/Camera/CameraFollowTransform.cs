using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTransform : MonoBehaviour
{
    [SerializeField]
    Transform _transformToFollow;

    [SerializeField]
    float _cameraZPosition;

    private void Awake()
    {
        _cameraZPosition = transform.position.z;
    }
    
    private void LateUpdate()
    {
        Vector3 position = (_transformToFollow?.position).GetValueOrDefault();
        position.z = _cameraZPosition;
        transform.position = position;
    }
}
