using Helpers;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SpawnMinionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var minionSpawnAspect in SystemAPI.Query<MinionSpawnAspect>())
            {
                minionSpawnAspect.DecrementTimers(deltaTime);
                if (minionSpawnAspect.ShouldSpawn)
                {
                    SpawnOnEachLane(ref state);
                    minionSpawnAspect.CountSpawnedInWave++;

                    if (minionSpawnAspect.IsWaveSpawned)
                    {
                        minionSpawnAspect.ResetMinionTimer();
                        minionSpawnAspect.ResetWaveTimer();
                        minionSpawnAspect.ResetSpawnCounter();
                    }
                    else
                    {
                        minionSpawnAspect.ResetMinionTimer();
                    }
                }
            }
        }

        private void SpawnOnEachLane(ref SystemState state)
        {
            //This won't work without ref SystemState 
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            Debug.Log("Spawning on each lane");
        }
    }
}