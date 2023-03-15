using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class VehicleParamsAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject VehiclePrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var vehData = new VehicleData
        {
            VehicleEntityPrefab = conversionSystem.GetPrimaryEntity(VehiclePrefab)
        };
        dstManager.AddComponentData(entity, vehData);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(VehiclePrefab);
    }
}
