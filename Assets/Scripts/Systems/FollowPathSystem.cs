using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class FollowPathSystem : ComponentSystem
{
    private float _minDist = .1f;
    private float _moveSpeed = 3f;
    private float _targetZPos = -0.1f;

    protected override void OnUpdate()
    {
        Entities
            .WithAll<VehicleTag>()
            .WithNone<RoadTag>()
            .ForEach((DynamicBuffer<PathPositionBuffer> buffer, ref Translation translation, ref FollowPathData followPathData) =>
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
                }
            }
        });
    }
}
