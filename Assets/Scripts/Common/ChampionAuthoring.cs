using Unity.Entities;

using UnityEngine;

namespace Common
{
    public class ChampionAuthoring : MonoBehaviour
    {
        public class ChampionBaker : Baker<ChampionAuthoring>
        {
            public override void Bake(ChampionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<ChampTag>(entity);
                AddComponent<NewChampTag>(entity);
                AddComponent<MobaTeam>(entity);
            }
        }
    }
}