using Unity.Entities;
using Unity.Mathematics;

public struct BuildingParamsData : IComponentData
{
    public uint RandomSeed;
    public float2 Offset;
    public Entity EntityConsumerPrefab;
    public Entity EntityProducerPrefab;
    public int NumberOfPrefabsProducer;
    public int NumberOfPrefabsConsumer;
}
