using Unity.Entities;

using UnityEngine;

namespace Common
{
    public struct MobaPrefabs : IComponentData
    {
        public Entity Champion;
        public Entity Minion;
    }

    public class UIPrefabs : IComponentData
    {
        public GameObject HealthBar;
        public GameObject SkillShot;
    }
}