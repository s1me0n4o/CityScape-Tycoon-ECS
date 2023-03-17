using Unity.Entities;
using UnityEngine;

public class ProducerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private int _generateOnSeconds;
    [SerializeField] private int _currentAmount;
    [SerializeField] private float _timerValue;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var producerData = new ProducerData
        {
            CurrentAmount = _currentAmount,
            GenerateOnSeconds = _generateOnSeconds,
            TimerValue = _timerValue,
        };
        dstManager.AddComponentData(entity, producerData);
    }
}
