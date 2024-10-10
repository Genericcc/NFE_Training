using Unity.Entities;
using Unity.NetCode;

namespace Common
{
    public struct GamePlayingTag : IComponentData { }

    public struct GameStartTick : IComponentData
    {
        public NetworkTick Value;
    }
}