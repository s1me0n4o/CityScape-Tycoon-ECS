using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class DispatchVehicleSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<VehicleData>();
    }
    protected override void OnUpdate()
    {
        // TODO: Job
        Entities
            .WithAll<ConsumerTag>()
            .WithNone<RoadTag>()
            .ForEach((Entity consumerEntity, ref Translation consumerTranslation, ref ConsumerData consumerData) =>
            {
                if (consumerData.SendVehicle)
                {
                    var producer = consumerData.AssignedProducer;
                    var currentVeh = consumerData.Vehicle;
                    Entities
                        .WithAll<VehicleTag>()
                        .WithNone<RoadTag>().
                        ForEach((Entity e, ref VehicleData vehData, ref Translation vehTranslation) =>
                        {
                            if (currentVeh != e)
                                return;

                            if (vehData.Destination != Entity.Null)
                                return;

                            var producerTranslation = EntityManager.GetComponentData<Translation>(producer);
                            EntityManager.AddComponentData(e, new PathfindingParams
                            {
                                StartPosition = new int2((int)vehTranslation.Value.x, (int)vehTranslation.Value.y),
                                EndPosition = new int2((int)producerTranslation.Value.x, (int)producerTranslation.Value.y)
                            });
                            vehData.Destination = producer;
                        });
                }
            });

    }
}
