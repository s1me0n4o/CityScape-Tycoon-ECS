using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class CheckVehicleAvailabilitySystem : ComponentSystem
{
    private readonly float _minDist = .2f;
    protected override void OnUpdate()
    {
        Entities
        .WithAll<VehicleTag>()
        .WithNone<RoadTag>()
        .ForEach((Entity e, ref VehicleData vehData, ref Translation vehTranslation) =>
        {
            var consTranslation = EntityManager.GetComponentData<Translation>(vehData.AssignedToConsumer);
            if (math.distance(vehTranslation.Value, consTranslation.Value) <= _minDist 
                && !vehData.HasArrivedToConsumer 
                && vehData.IsReturning)
            {
                Debug.Log("Test");
                vehData.IsReturning = false;
                vehData.HasArrivedToProducer = false;
                vehData.HasArrivedToConsumer = true;
                EntityManager.SetComponentData(e, vehData);
                var conData = EntityManager.GetComponentData<ConsumerData>(vehData.AssignedToConsumer);
                conData.SendVehicle = false;
                Entities
                    .WithAll<ConsumerTag>()
                    .ForEach((Entity e, ref ConsumerData consumerData) =>
                    {
                        consumerData.SendVehicle = false;
                        EntityManager.SetComponentData(e, consumerData);
                    });
                
                EntityManager.RemoveComponent<ReturnWithoutProductTag>(e);
            }
        });

    }
}
