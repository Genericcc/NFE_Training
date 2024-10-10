using System;

using Common;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

using UnityEngine;

namespace Server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessGameEntryRequestSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameStartProperties>();
            state.RequireForUpdate<MobaPrefabs>();
            
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<MobaTeamRequest, ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var championPrefab = SystemAPI.GetSingleton<MobaPrefabs>().Champion;

            var gamePropertiesEntity = SystemAPI.GetSingletonEntity<GameStartProperties>();
            var gameStartProperties = SystemAPI.GetComponent<GameStartProperties>(gamePropertiesEntity);
            var teamPlayerCounter = SystemAPI.GetComponent<TeamPlayerCounter>(gamePropertiesEntity);
            var spawnOffsets = SystemAPI.GetBuffer<SpawnOffset>(gamePropertiesEntity);
            
            foreach (var (teamRequest, requestSource, requestEntity) in 
                     SystemAPI.Query<MobaTeamRequest, ReceiveRpcCommandRequest>().WithEntityAccess())
            {
                ecb.DestroyEntity(requestEntity);
                ecb.AddComponent<NetworkStreamInGame>(requestSource.SourceConnection);
                
                var requestedTeamType = teamRequest.Value;
                
                if (requestedTeamType == TeamType.AutoAssign)
                {
                    requestedTeamType = TeamType.Blue;
                }
                
                var clientId = SystemAPI.GetComponent<NetworkId>(requestSource.SourceConnection).Value;
                Debug.Log($"Server is assigning Client ID: {clientId} to the {requestedTeamType.ToString()} team;");
                
                float3 spawnPosition;

                switch (requestedTeamType)
                {
                    case TeamType.Blue:
                        if (teamPlayerCounter.BlueTeamPlayers >= gameStartProperties.MaxPlayersPerTeam)
                        {
                            Debug.Log($"Blue team is full, Client ID: {clientId} is spectating the game.");
                            continue;
                        }
                        spawnPosition = new float3(-50f, 1f, -50f);
                        spawnPosition += spawnOffsets[teamPlayerCounter.BlueTeamPlayers].Value;
                        teamPlayerCounter.BlueTeamPlayers++;
                        break;

                    case TeamType.Red:
                        if (teamPlayerCounter.RedTeamPlayers >= gameStartProperties.MaxPlayersPerTeam)
                        {
                            Debug.Log($"Red team is full, Client ID: {clientId} is spectating the game.");
                            continue;
                        }
                        spawnPosition = new float3(50f, 1f, 50f);
                        spawnPosition += spawnOffsets[teamPlayerCounter.RedTeamPlayers].Value;
                        teamPlayerCounter.RedTeamPlayers++;
                        break;

                    default:
                        continue;
                }
                
                var newChamp = ecb.Instantiate(championPrefab);
                ecb.SetName(newChamp, "Champion");
                
                var newTransform = LocalTransform.FromPosition(spawnPosition);
                ecb.SetComponent(newChamp, newTransform);
                ecb.SetComponent(newChamp, new GhostOwner { NetworkId = clientId });
                ecb.SetComponent(newChamp, new MobaTeam { Value = requestedTeamType});
                
                ecb.AppendToBuffer(requestSource.SourceConnection, new LinkedEntityGroup { Value = newChamp });
            }
            
            //ECB ZAWSZE PO FOREACHU, MORDO
            ecb.Playback(state.EntityManager);
            
            //Incrementing teamPlayerCounter does not actually change the component, it has to be set like this
            SystemAPI.SetSingleton(teamPlayerCounter);
        }
    }
}