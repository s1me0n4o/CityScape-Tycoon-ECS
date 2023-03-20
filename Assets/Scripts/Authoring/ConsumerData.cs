using Unity.Entities;

public struct ConsumerData : IComponentData
{
    public Entity AssignedProducer;
    public Entity Vehicle;
    public int ProductCount;
    public bool SendVehicle;
}
