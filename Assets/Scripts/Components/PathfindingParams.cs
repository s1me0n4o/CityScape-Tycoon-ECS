using Unity.Entities;
using Unity.Mathematics;

public class PathfindingParams : IComponentData
{
    public int2 StartPosition;
    public int2 EndPosition;
}
