using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BuildingRandomSystem))]
public partial class VehicleSpawnSystem : ComponentSystem
{
    protected override void OnStartRunning()
    {
        var vehData = GetSingleton<VehicleData>();

        Entities
            .WithAll<ConsumerTag>()
            .ForEach((Entity e, ref Translation eTranslation) =>
            {
                var newEntity = EntityManager.Instantiate(vehData.VehicleEntityPrefab);
                EntityManager.AddComponent<Translation>(newEntity);
                EntityManager.AddComponent<VehicleTag>(newEntity);

                // pathfinding
                EntityManager.AddComponent<PathPositionAuthoring>(newEntity);
                //EntityManager.AddComponent<PathfindingParams>(newEntity);
                EntityManager.AddComponent<PathPositionBuffer>(newEntity);
                EntityManager.AddComponentData(newEntity, new FollowPathData { PathIndex = -1 });

                // position
                var pos = new float3(eTranslation.Value.x, eTranslation.Value.y - .2f, -0.1f);
                EntityManager.SetComponentData(newEntity, new Translation { Value = pos });
            });
    }

    protected override void OnUpdate()
    {
        // TODO: Job!
        if (Input.GetMouseButtonDown(0))
        {
            Entities
                .WithAll<VehicleTag>()
                .WithNone<RoadTag>()
                .ForEach((Entity e, ref Translation eTranslation) =>
                {
                    var vehiclePos = eTranslation.Value;
                    var closestTargetEntity = Entity.Null;
                    var closestTargetPos = float3.zero;

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

                    if (closestTargetEntity != Entity.Null)
                    {
                        PostUpdateCommands.AddComponent(e, new PathfindingParams
                        {
                            StartPosition = new int2((int)eTranslation.Value.x, (int)eTranslation.Value.y),
                            EndPosition = new int2((int)closestTargetPos.x, (int)closestTargetPos.y)
                        });
                    }
                });

        }
    }
}
