using Pathfinding;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class NodeAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GridConfig GridConfig;
    [SerializeField] private GameObject NodePrefab;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(NodePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        World world = World.DefaultGameObjectInjectionWorld;
        var spawnerSystem = world.GetOrCreateSystem<GridGeneratorSystem>();
        spawnerSystem.GoPrefab = NodePrefab;
        spawnerSystem.GridSize = GridConfig.GridSize;

        world.EntityManager.CompleteAllJobs();
        spawnerSystem.Update();
    }
}
