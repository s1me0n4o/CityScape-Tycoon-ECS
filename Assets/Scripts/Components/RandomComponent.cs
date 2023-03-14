using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct RandomComponent : IComponentData
{
    public Random ValueX;
    public Random ValueY;
    public int RandomValueX;
    public int RandomValueY;
}
