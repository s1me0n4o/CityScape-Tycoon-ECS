using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class ReturnVehicleSystem : ComponentSystem
{
    private float _minDist = .2f;

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

                var prodEntity = vehData.Destination;
                var consumerEntity = vehData.AssignedToConsumer;
                var destTranslation = EntityManager.GetComponentData<Translation>(vehData.Destination);
                var prodData = EntityManager.GetComponentData<ProducerData>(vehData.Destination);
                if (prodData.CurrentAmount <= 0)
                {
                    EntityManager.AddComponent<ReturnWithoutProductTag>(vehicleEntity);
                    return;
                }
                var consumerTranslation = EntityManager.GetComponentData<Translation>(vehData.AssignedToConsumer);
                //UnityEngine.Debug.Log(math.distance(translation.Value, destTranslation.Value));
                if (math.distance(translation.Value, destTranslation.Value) < _minDist 
                    && !vehData.HasArrivedToConsumer
                    && !vehData.HasArrivedToProducer)
                {
                    prodData.CurrentAmount--;
                    EntityManager.SetComponentData(vehData.Destination, prodData);
                    vehData.HasArrivedToProducer = true;
                    vehData.HasArrivedToConsumer = false;
                    vehData.IsReturning = true;
                    EntityManager.SetComponentData(vehicleEntity, vehData);
                    
                    UnityEngine.Debug.Log($"Vehicle prod data  = {prodData.CurrentAmount}");
                    var consumerData = EntityManager.GetComponentData<ConsumerData>(vehData.AssignedToConsumer);
                    consumerData.ProductCount++;
                    
                    
                    // add pathfinding
                    EntityManager.AddComponentData(vehicleEntity, new PathfindingParams
                    {
                        StartPosition = new int2((int)translation.Value.x, (int)translation.Value.y),
                        EndPosition = new int2((int)consumerTranslation.Value.x, (int)consumerTranslation.Value.y)
                    });


                    Entities
                    .WithAll<ProducerTag>()
                    .ForEach((Entity entity, ref ProducerData data) =>
                    {
                        if (entity == prodEntity)
                        {
                            data.CurrentAmount = prodData.CurrentAmount;
                            EntityManager.SetComponentData(entity, data);
                            UnityEngine.Debug.Log($"Update Prod data  = {data.CurrentAmount}");

                        }
                    });

                    Entities
                    .WithAll<ConsumerTag>()
                    .ForEach((Entity entity, ref ConsumerData data) =>
                    {
                        if (entity == consumerEntity)
                        {
                            data.ProductCount = consumerData.ProductCount;
                            EntityManager.SetComponentData(entity, data);
                            UnityEngine.Debug.Log($"Update Cons data  = {data.ProductCount}");
                        }
                    });

                    // update consumer,
                    // update producer
                }
            });

    }
}
