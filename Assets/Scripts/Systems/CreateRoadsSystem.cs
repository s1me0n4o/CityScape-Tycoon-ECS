using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(BuildingRandomSystem))]
public class CreateRoadsSystem : ComponentSystem
{
    private RoadParamsData _roadData;

    protected override void OnStartRunning()
    {
        _roadData = GetSingleton<RoadParamsData>();

        var entityManager = EntityManager;

        // Get all building entities
        var buildingEntities = new NativeList<Entity>(7, Allocator.Temp);

        Entities
            .WithAll<BuildingParamsData>()
            .ForEach((Entity e) =>
            {
                buildingEntities.Add(e);
            });

        for (int i = 0; i < buildingEntities.Length; i++)
        {
            for (int j = i + 1; j < buildingEntities.Length; j++)
            {
                var roadData = new RoadParamsData
                {
                    RoadEntityPrefab = _roadData.RoadEntityPrefab,
                };
                var startBuildingPosition = EntityManager.GetComponentData<Translation>(buildingEntities[i]).Value;
                var endBuildingPosition = EntityManager.GetComponentData<Translation>(buildingEntities[j]).Value;
                if (startBuildingPosition.x == 0)
                {
                    continue;
                }

                var entity = CreateRoad(roadData);
                //pathfinding
                EntityManager.AddComponent<PathPositionAuthoring>(entity);
                EntityManager.AddComponent<PathfindingParams>(entity);
                EntityManager.AddComponent<PathPositionBuffer>(entity);
                EntityManager.AddComponentData(entity, new FollowPathData { PathIndex = -1 });
                EntityManager.SetComponentData(entity, new PathfindingParams
                {
                    StartPosition = new int2((int)startBuildingPosition.x, (int)startBuildingPosition.y),
                    EndPosition = new int2((int)endBuildingPosition.x, (int)endBuildingPosition.y),
                });

                // set start road position
                entityManager.SetComponentData(entity, new Translation { Value = new float3(startBuildingPosition.x, startBuildingPosition.y, 0) });
            }
        }

        buildingEntities.Dispose();
    }

    protected override void OnUpdate()
    {
        Entities
         .WithAll<RoadTag>()
         .WithNone<VehicleTag>()
        .ForEach((Entity e, DynamicBuffer<PathPositionBuffer> buffer, ref FollowPathData followPathData) =>
        {
            if (followPathData.PathIndex >= 0)
            {
                var pathPos = buffer[followPathData.PathIndex].Position;

                var targetPos = new float3(pathPos.x, pathPos.y, -0.01f);
                if (!GridMono.Instance.Grid.GetGridObject((int)targetPos.x, (int)targetPos.y).IsTaken)
                {
                    var roadData = new RoadParamsData
                    {
                        RoadEntityPrefab = _roadData.RoadEntityPrefab,
                    };
                    var entity = CreateRoad(roadData);
                    EntityManager.SetComponentData(entity, new Translation { Value = new float3(targetPos.x, targetPos.y, targetPos.z) });

                    // update grid
                    GridMono.Instance.Grid.GetGridObject((int)targetPos.x, (int)targetPos.y).TakeNode(UnitType.Road);
                    GridMono.Instance.Grid.GetGridObject((int)targetPos.x, (int)targetPos.y).SetWalkable(true);
                    if (followPathData.PathIndex <= 0)
                    {
                        PostUpdateCommands.RemoveComponent<PathPositionAuthoring>(e);
                        PostUpdateCommands.RemoveComponent<FollowPathData>(e);
                        PostUpdateCommands.RemoveComponent<PathfindingParams>(e);
                    }
                }
                followPathData.PathIndex--;
            }
        });
    }

    private Entity CreateRoad(RoadParamsData roadParamsData)
    {
        var newEntity = EntityManager.Instantiate(roadParamsData.RoadEntityPrefab);
        EntityManager.AddComponent<RoadParamsData>(newEntity);
        EntityManager.AddComponent<Translation>(newEntity);
        EntityManager.AddComponent<RoadTag>(newEntity);
        return newEntity;
    }
}
