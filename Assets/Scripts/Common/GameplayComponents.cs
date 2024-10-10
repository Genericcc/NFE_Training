using Unity.Entities;
using Unity.NetCode;

namespace Common
{
    public struct GamePlayingTag : IComponentData { }

    public struct GameStartTick : IComponentData
    {
        public NetworkTick Value;
    }
    
    public struct GameOverTag : IComponentData { }

    public struct WinningTeam : IComponentData
    {
        [GhostField] public TeamType Value;
    }
}