using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
            .ForEach((ref ConsumerData consumerData) =>
            {
                if (!consumerData.SendVehicle) 
                    return;
                
                var producer = consumerData.AssignedProducer;
                var currentVeh = consumerData.Vehicle;
                Entities
                    .WithAll<VehicleTag>()
                    .WithNone<RoadTag>()
                    .WithNone<ReturnWithoutProductTag>()
                    .ForEach((Entity e, ref VehicleData vehData, ref Translation vehTranslation) =>
                    {
                        if (currentVeh != e)
                            return;

                        if (vehData.Destination != Entity.Null || !vehData.HasArrivedToConsumer)
                            return;

                        vehData.Destination = producer;
                        vehData.HasArrivedToConsumer = false;
                        vehData.HasArrivedToProducer = false;
                        Debug.Log("Vehicle dispatched everything should be false ");
                        EntityManager.SetComponentData(e, vehData);

                        // adding pathfinding in order the vehicle to move towards the target
                        var producerTranslation = EntityManager.GetComponentData<Translation>(producer);
                        EntityManager.AddComponentData(e, new PathfindingParams
                        {
                            StartPosition = new int2((int)vehTranslation.Value.x, (int)vehTranslation.Value.y),
                            EndPosition = new int2((int)producerTranslation.Value.x, (int)producerTranslation.Value.y)
                        });
                    });
            });

    }
}
