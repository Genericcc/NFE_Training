using Unity.Entities;
using Unity.NetCode;

namespace Common
{
    public struct MaxHitPoints : IComponentData
    {
        public int Value;
    }
    public struct CurrentHitPoints : IComponentData
    {
        [GhostField] public int Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct DamageBufferElement : IBufferElementData
    {
        public int Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted, OwnerSendType = SendToOwnerType.SendToNonOwner)]
    public struct DamageThisTick : ICommandData
    {
        public NetworkTick Tick { get; set; }
        public int Value;
    }

    public struct AbilityPrefabs : IComponentData
    {
        public Entity AoeAbility;
    }

    public struct DestroyOnTimer : IComponentData
    {
        public float Value;
    }
    
    public struct DestroyAtTick : IComponentData
    {
        [GhostField] public NetworkTick Value;
    }
    
    public struct DestroyEntityTag : IComponentData {}

    public struct DamageOnTrigger : IComponentData
    {
        public int Value;
    }
    public struct AlreadyDamagedEntity : IBufferElementData
    {
        public Entity Value;
    }

    //How many ticks before cooldown ends
    public struct AbilityCooldownTicks : IComponentData
    {
        public uint AoeAbilityCooldownTicks;
    }

    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct AbilityCooldownTargetTicks : ICommandData
    {
        public NetworkTick Tick { get; set; }
        public NetworkTick AoeAbilityEndCooldownTick;
    }
}














































