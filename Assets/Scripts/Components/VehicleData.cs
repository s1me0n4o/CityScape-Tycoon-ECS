using Unity.Entities;

public struct VehicleData : IComponentData
{
    public Entity VehicleEntityPrefab;
    public bool IsReturning;
    public Entity AssignedToConsumer;
    public Entity Destination;
}
