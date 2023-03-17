using Unity.Entities;

[GenerateAuthoringComponent]
public class ConsumerDataAuthoring : IComponentData
{
    public Entity AssignedProducer;
    public int ProductCount;
}
