using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class FollowPathSystem : ComponentSystem
{
    private float _minDist = .1f;
    private float _moveSpeed = 3f;
    private float _targetZPos = -0.1f;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<VehicleTag>();
    }

    protected override void OnUpdate()
    {
        Entities
            .WithAll<VehicleTag>()
            .WithNone<RoadTag>()
            .ForEach((
                Entity e,
                DynamicBuffer<PathPositionBuffer> buffer,
                ref Translation translation,
                ref FollowPathData followPathData,
                ref VehicleData vData) =>
        {
            if (followPathData.PathIndex >= 0)
            {
                var pathPos = buffer[followPathData.PathIndex].Position;

                var targetPos = new float3(pathPos.x, pathPos.y, _targetZPos);
                var moveDir = math.normalizesafe(targetPos - translation.Value);

                translation.Value += moveDir * _moveSpeed * Time.DeltaTime;

                if (math.distance(translation.Value, targetPos) < _minDist)
                {
                    followPathData.PathIndex--;
                    if (followPathData.PathIndex <= 0 && !vData.IsReturning)
                    {
                        // producer reached
                        UnityEngine.Debug.Log($"AddingReturnPath!");
                        vData.IsReturning = true;
                        // adding pathfinding to the consumer
                        var consumerTranslation = EntityManager.GetComponentData<Translation>(vData.AssignedToConsumer);
                        EntityManager.AddComponentData(e, new PathfindingParams
                        {
                            StartPosition = new int2((int)translation.Value.x, (int)translation.Value.y),
                            EndPosition = new int2((int)consumerTranslation.Value.x, (int)consumerTranslation.Value.y)
                        });
                    }
                }
            }
        });
    }
}
