using Unity.Entities;

using UnityEngine;

namespace Common
{
    public class AbilityAuthoring : MonoBehaviour
    {
        public GameObject aoeAbilityPrefab;
        private class AbilityBaker : Baker<AbilityAuthoring>
        {
            public override void Bake(AbilityAuthoring authoring)
            {
                var abilitesContainerPrefab = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(abilitesContainerPrefab, new AbilityPrefabs
                {
                    AoeAbility = GetEntity(authoring.aoeAbilityPrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}