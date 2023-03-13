using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BuildingParamsAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject ProducerPrefab;
    public GameObject ConsumerPrefab;
    public int NumberOfPrefabsConsumer;
    public int NumberOfPrefabsProducer;
    public uint RandomSeed;
    public float2 Offset;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var buildingData = new BuildingParamsData
        {
            RandomSeed = RandomSeed,
            Offset = Offset,
            EntityProducerPrefab = conversionSystem.GetPrimaryEntity(ProducerPrefab),
            EntityConsumerPrefab = conversionSystem.GetPrimaryEntity(ConsumerPrefab),
            NumberOfPrefabsProducer = NumberOfPrefabsProducer,
            NumberOfPrefabsConsumer = NumberOfPrefabsConsumer
        };
        dstManager.AddComponentData(entity, buildingData);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ProducerPrefab);
        referencedPrefabs.Add(ConsumerPrefab);
    }
}
