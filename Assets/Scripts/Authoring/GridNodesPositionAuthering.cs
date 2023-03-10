using Unity.Entities;
using UnityEngine;

public class GridNodesPositionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<PathPositionBuffer>(entity);
    }
}
