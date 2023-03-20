using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class ReturnVehicleSystem : ComponentSystem
{
    private float _minDist = .13f;

    protected override void OnUpdate()
    {
        Entities
            .WithAll<VehicleTag>()
            .WithNone<RoadTag>()
            .WithNone<ReturnWithoutProductTag>()
            .ForEach((Entity vehicleEntity, ref VehicleData vehData, ref Translation translation) =>
            {
                if (vehData.Destination == Entity.Null)
                    return;

                var destTranslation = EntityManager.GetComponentData<Translation>(vehData.Destination);
                var prodData = EntityManager.GetComponentData<ProducerData>(vehData.Destination);
                if (prodData.CurrentAmount <= 0)
                {
                    EntityManager.AddComponent<ReturnWithoutProductTag>(vehicleEntity);
                    return;
                }
                var consumerTranslation = EntityManager.GetComponentData<Translation>(vehData.AssignedToConsumer);
                //UnityEngine.Debug.Log(math.distance(translation.Value, destTranslation.Value));
                if (math.distance(translation.Value, destTranslation.Value) < _minDist)
                {
                    EntityManager.AddComponentData(vehicleEntity, new PathfindingParams
                    {
                        StartPosition = new int2((int)translation.Value.x, (int)translation.Value.y),
                        EndPosition = new int2((int)consumerTranslation.Value.x, (int)consumerTranslation.Value.y)
                    });
                    prodData.CurrentAmount--;
                    EntityManager.SetComponentData(vehData.Destination, prodData);
                    var consumerData = EntityManager.GetComponentData<ConsumerData>(vehData.AssignedToConsumer);
                    consumerData.ProductCount++;

                    // update consumer,
                    // update producer
                }
            });

    }
}
