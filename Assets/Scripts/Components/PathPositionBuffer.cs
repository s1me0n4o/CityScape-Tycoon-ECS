using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(20)]
public struct PathPositionBuffer : IBufferElementData
{
    public int2 Position;
}
