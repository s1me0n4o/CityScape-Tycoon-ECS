using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class ReturnWithoutProductSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
        .WithAll<ReturnWithoutProductTag>()
        .WithNone<RoadTag>()
        .ForEach((Entity e, ref VehicleData vehData, ref Translation vehTranslation) =>
        {
            if (!vehData.IsReturning)
            {
                EntityManager.RemoveComponent<FollowPathData>(e);
                EntityManager.RemoveComponent<PathfindingParams>(e);

                var consumerTranslation = EntityManager.GetComponentData<Translation>(vehData.AssignedToConsumer);

                EntityManager.AddComponentData(e, new PathfindingParams
                {
                    StartPosition = new int2((int)vehTranslation.Value.x, (int)vehTranslation.Value.y),
                    EndPosition = new int2((int)consumerTranslation.Value.x, (int)consumerTranslation.Value.y)
                });
                EntityManager.AddComponent<FollowPathData>(e);
                vehData.IsReturning = true;
            }
        });

    }
}
