using Unity.Entities;
using Unity.Rendering;

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
                AddComponent<URPMaterialPropertyBaseColor>(entity);
                AddComponent<ChampMoveTargetPosition>(entity);
            }
        }
    }
}