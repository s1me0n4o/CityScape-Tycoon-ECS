using Unity.Entities;
using Unity.Mathematics;

public struct VehicleData : IComponentData
{
    public Entity VehicleEntityPrefab;
    public int2 InitialPos;
}
