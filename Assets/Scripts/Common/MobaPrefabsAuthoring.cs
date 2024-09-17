using Unity.Entities;

using UnityEngine;

namespace Common
{
    public class MobaPrefabsAuthoring : MonoBehaviour
    {
        [Header("Entities")]
        public GameObject champion;

        [Header("GameObjects")]
        public GameObject healthBarPrefab;
        
        private class MobaPrefabsAuthoringBaker : Baker<MobaPrefabsAuthoring>
        {
            public override void Bake(MobaPrefabsAuthoring authoring)
            {
                var prefabContainerEntity = GetEntity(TransformUsageFlags.None);
                AddComponent(prefabContainerEntity, new MobaPrefabs
                {
                    Champion = GetEntity(authoring.champion, TransformUsageFlags.Dynamic),
                });
                
                AddComponentObject(prefabContainerEntity, new UIPrefabs
                {
                    HealthBar = authoring.healthBarPrefab,
                });
            }
        }
    }
}