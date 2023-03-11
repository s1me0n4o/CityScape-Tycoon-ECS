using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BuildingParams : IComponentData
{
    public uint RandomSeed;
    public float2 Offset;
}
