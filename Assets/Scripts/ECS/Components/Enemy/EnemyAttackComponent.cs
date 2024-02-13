using Unity.Entities;

[System.Serializable]
public struct EnemyAttackComponent : IComponentData
{
    public float DamageAmount;
    public GenericCooldownComponent AttackCooldown;
}
