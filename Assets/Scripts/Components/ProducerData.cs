using Unity.Entities;

public struct ProducerData : IComponentData
{
    public int GenerateOnSeconds;
    public int CurrentAmount;
    public float TimerValue;
}
