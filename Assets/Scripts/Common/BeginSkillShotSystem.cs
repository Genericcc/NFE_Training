using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Common
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct BeginSkillShotSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            if (!networkTime.IsFirstTimeFullyPredictingTick) { return; }
            var currentTick = networkTime.ServerTick;
            var isServer = state.WorldUnmanaged.IsServer();

            foreach (var skillShot in SystemAPI.Query<SkillShotAspect>().WithAll<Simulate>().WithNone<AimSkillShotTag>())
            {
                var isOnCooldown = true;

                for (var i = 0u; i < networkTime.SimulationStepBatchSize; i++)
                {
                    var testTick = currentTick;
                    testTick.Subtract(i);

                    if (!skillShot.AbilityCooldownTargetTicks.GetDataAtTick(currentTick, out var currentTargetTicks))
                    {
                        currentTargetTicks.SkillShotAbilityEndCooldownTick = NetworkTick.Invalid;
                    }

                    if (currentTargetTicks.SkillShotAbilityEndCooldownTick == NetworkTick.Invalid ||
                        !currentTargetTicks.SkillShotAbilityEndCooldownTick.IsNewerThan(currentTick))
                    {
                        isOnCooldown = false;
                        break;
                    }
                }
                
                if (isOnCooldown) continue;
                if (!skillShot.BeginAttack) continue;
                
                ecb.AddComponent<AimSkillShotTag>(skillShot.ChampionEntity);
            }

            foreach (var skillShot in SystemAPI.Query<SkillShotAspect>().WithAll<AimSkillShotTag, Simulate>())
            {
                if (!skillShot.ConfirmAttack) { continue; }

                var skillShotAbility = ecb.Instantiate(skillShot.AbilityPrefab);
                
                var spawnPosition = skillShot.SpawnPosition;
                ecb.SetComponent(skillShotAbility, spawnPosition);
                ecb.SetComponent(skillShotAbility, skillShot.Team);
                ecb.RemoveComponent<AimSkillShotTag>(skillShot.ChampionEntity);

                if (isServer) { continue; }
                
                skillShot.AbilityCooldownTargetTicks.GetDataAtTick(currentTick, out var currentTargetTicks);

                var newCooldownTargetTick = currentTick;
                newCooldownTargetTick.Add(skillShot.CooldownTicks);
                currentTargetTicks.SkillShotAbilityEndCooldownTick = newCooldownTargetTick;

                var nextTick = currentTick;
                nextTick.Add(1u);
                currentTargetTicks.Tick = nextTick;
                
                skillShot.AbilityCooldownTargetTicks.AddCommandData(currentTargetTicks);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}