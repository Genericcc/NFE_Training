using Unity.Entities;
using Unity.NetCode;

using UnityEngine;

namespace Common
{
    public class AbilityAuthoring : MonoBehaviour
    {
        public GameObject aoeAbilityPrefab;
        public float aoeAbilityCooldown;

        public NetCodeConfig netCodeConfig;
        private int SimulationTickRate => netCodeConfig.ClientServerTickRate.SimulationTickRate;
        
        private class AbilityBaker : Baker<AbilityAuthoring>
        {
            public override void Bake(AbilityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AbilityPrefabs
                {
                    AoeAbility = GetEntity(authoring.aoeAbilityPrefab, TransformUsageFlags.Dynamic),
                });
                AddComponent(entity, new AbilityCooldownTicks
                {
                    AoeAbilityCooldownTicks = (uint)(authoring.aoeAbilityCooldown * authoring.SimulationTickRate),
                });
                AddBuffer<AbilityCooldownTargetTicks>(entity);
            }
        }
    }
}