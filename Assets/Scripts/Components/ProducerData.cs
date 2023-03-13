using Unity.Entities;

[GenerateAuthoringComponent]
public struct ProducerData : IComponentData
{
    public int GenerateOnSeconds;
    public int CurrentAmount;
    public float TimerValue;
}
