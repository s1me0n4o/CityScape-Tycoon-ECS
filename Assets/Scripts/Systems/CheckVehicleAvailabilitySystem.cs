using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class CheckVehicleAvailabilitySystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
        .WithAll<VehicleTag>()
        .WithNone<RoadTag>()
        .ForEach((Entity e, ref VehicleData vehData, ref Translation vehTranslation) =>
        {
            var consTranslation = EntityManager.GetComponentData<Translation>(vehData.AssignedToConsumer);
            if (math.distance(vehTranslation.Value, consTranslation.Value) <= .1f)
            {
                vehData.IsMoving = false;
                vehData.IsReturning = false;
                var conData = EntityManager.GetComponentData<ConsumerData>(vehData.AssignedToConsumer);
                conData.SendVehicle = false;
                EntityManager.RemoveComponent<ReturnWithoutProductTag>(e);
            }
            else
            {
                vehData.IsMoving = true;
            }
        });

    }
}
