using Pathfinding;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class NodeAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField] private GameObject NodePrefab;
    [SerializeField] private int2 GridSize;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(NodePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        World world = World.DefaultGameObjectInjectionWorld;
        var spawnerSystem = world.GetOrCreateSystem<GridGeneratorSystem>();
        spawnerSystem.GoPrefab = NodePrefab;
        spawnerSystem.GridSize = GridSize;

        world.EntityManager.CompleteAllJobs();
        spawnerSystem.Update();
    }
}
