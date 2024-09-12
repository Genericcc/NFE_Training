﻿using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Common
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct BeginAoeAbilitySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //Robimy to w BeginSimulation żeby był od razu dobry transform
            //a nie że przed 1 klatkę pojawi się w (0,0,0) i dopiero ustawi na dobrą pozycje
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var networkTime = SystemAPI.GetSingleton<NetworkTime>();

            if (!networkTime.IsFirstTimeFullyPredictingTick)
            {
                return;                
            }

            var currentTick = networkTime.ServerTick;

            foreach (var aoeAspect in SystemAPI.Query<AoeAspect>().WithAll<Simulate>())
            {
                if (aoeAspect.ShouldAttack)
                {
                    var newAoeAbility = ecb.Instantiate(aoeAspect.AbilityPrefab);
                    var abilityTransform = LocalTransform.FromPosition(aoeAspect.AttackPosition);
                    ecb.SetComponent(newAoeAbility, abilityTransform);
                    ecb.SetComponent(newAoeAbility, aoeAspect.Team);
                }
            }

        }
    }
}