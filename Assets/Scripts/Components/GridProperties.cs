using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct GridProperties : IComponentData
    {
        public float2 GridSize;
    }
}