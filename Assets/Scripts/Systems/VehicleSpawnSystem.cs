using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
            .ForEach((Entity consumerEntity, ref Translation eTranslation) =>
            {
                var vehicleEntity = EntityManager.Instantiate(vehData.VehicleEntityPrefab);
                EntityManager.AddComponent<Translation>(vehicleEntity);
                EntityManager.AddComponent<VehicleTag>(vehicleEntity);
                EntityManager.AddComponentData(vehicleEntity, new VehicleData
                {
                    AssignedToConsumer = consumerEntity,
                    HasArrivedToConsumer = true,
                    HasArrivedToProducer = false
                });

                // pathfinding
                EntityManager.AddComponent<PathPositionAuthoring>(vehicleEntity);
                EntityManager.AddComponent<PathPositionBuffer>(vehicleEntity);

                // position
                var pos = new float3(eTranslation.Value.x, eTranslation.Value.y, _zPosition);
                EntityManager.SetComponentData(vehicleEntity, new Translation { Value = pos });
                EntityManager.AddComponentData(vehicleEntity, new FollowPathData { PathIndex = -1 });

                EntityManager.SetComponentData(consumerEntity, new ConsumerData { Vehicle = vehicleEntity });
            });
    }

    protected override void OnUpdate()
    {
        Enabled = false;
    }
}
