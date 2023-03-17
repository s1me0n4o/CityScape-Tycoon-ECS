using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BuildingRandomSystem))]
public partial class VehicleSpawnSystem : ComponentSystem
{
    private const float _zPosition = -0.1f;

    protected override void OnStartRunning()
    {
        var vehData = GetSingleton<VehicleData>();

        // create vehicle
        Entities
            .WithAll<ConsumerTag>()
            .ForEach((Entity e, ref Translation eTranslation) =>
            {
                var vehicleEntity = EntityManager.Instantiate(vehData.VehicleEntityPrefab);
                EntityManager.AddComponent<Translation>(vehicleEntity);
                EntityManager.AddComponent<VehicleTag>(vehicleEntity);

                // pathfinding
                EntityManager.AddComponent<PathPositionAuthoring>(vehicleEntity);
                //EntityManager.AddComponent<PathfindingParams>(newEntity);
                EntityManager.AddComponent<PathPositionBuffer>(vehicleEntity);

                // position
                var pos = new float3(eTranslation.Value.x, eTranslation.Value.y, _zPosition);
                EntityManager.SetComponentData(vehicleEntity, new Translation { Value = pos });
                EntityManager.AddComponentData(vehicleEntity, new FollowPathData { PathIndex = -1 });
                EntityManager.AddComponentData(vehicleEntity, new VehicleData { InitialPos = new int2((int)eTranslation.Value.x, (int)eTranslation.Value.y) });
            });
    }

    protected override void OnUpdate()
    {
        // TODO: Job!
        if (Input.GetMouseButtonDown(0)) // TODO: remove
        {
            Entities
                .WithAll<VehicleTag>()
                .WithNone<RoadTag>()
                .ForEach((Entity e, ref Translation vehicleTranslation) =>
                {
                    var vehiclePos = vehicleTranslation.Value;
                    var closestTargetEntity = Entity.Null;
                    var closestTargetPos = float3.zero;

                    // find closest producer
                    Entities
                        .WithAll<ProducerTag>()
                        .WithNone<RoadTag>()
                        .ForEach((Entity targetEntity, ref Translation tTranslation) =>
                        {
                            if (closestTargetEntity == Entity.Null)
                            {
                                // no target
                                closestTargetEntity = targetEntity;
                                closestTargetPos = tTranslation.Value;
                            }
                            else
                            {
                                if (math.distance(vehiclePos, tTranslation.Value) < math.distance(vehiclePos, closestTargetPos))
                                {
                                    // closer target
                                    closestTargetEntity = targetEntity;
                                    closestTargetPos = tTranslation.Value;
                                }
                            }
                        });

                    // add pathfinding to the producer
                    if (closestTargetEntity != Entity.Null)
                    {
                        Debug.Log("VehicleSpawn adding pathfinding params");
                        PostUpdateCommands.AddComponent(e, new PathfindingParams
                        {
                            StartPosition = new int2((int)vehicleTranslation.Value.x, (int)vehicleTranslation.Value.y),
                            EndPosition = new int2((int)closestTargetPos.x, (int)closestTargetPos.y)
                        });
                    }
                });

        }
    }
}
