using Pathfinding;
using Unity.Collections;
using Unity.Entities;

[GenerateAuthoringComponent]
public class GridTag : IComponentData
{
    public NativeArray<Node> GridArray;
}
