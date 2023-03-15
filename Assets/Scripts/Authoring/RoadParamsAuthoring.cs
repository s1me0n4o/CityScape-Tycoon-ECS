using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RoadParamsAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject RoadPrefab;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var roadData = new RoadParamsData
        {
            RoadEntityPrefab = conversionSystem.GetPrimaryEntity(RoadPrefab),
            //StartBuilding = entity,
            //EndBuilding = entity,
        };
        dstManager.AddComponentData(entity, roadData);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(RoadPrefab);
    }
}
