using System;
using Unity.Entities;

public class PlayerControllerComponent : IComponentData, IDisposable, ICloneable
{
    public PlayerController PlayerController;

    public object Clone()
    {
        return new PlayerControllerComponent { PlayerController = UnityEngine.Object.Instantiate(PlayerController) };
    }

    public void Dispose()
    {
       
    }
}
