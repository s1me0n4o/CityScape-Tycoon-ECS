using Unity.Entities;

public struct VehicleData : IComponentData
{
    public Entity VehicleEntityPrefab;
    public Entity AssignedToConsumer;
    public Entity Destination;
    public bool IsReturning;
    public bool HasArrivedToConsumer;
    public bool HasArrivedToProducer;
}
