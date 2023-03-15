using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class FollowPathSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((DynamicBuffer<PathPositionBuffer> buffer, ref Translation translation, ref FollowPathData followPathData) =>
        {
            if (followPathData.PathIndex >= 0)
            {
                var pathPos = buffer[followPathData.PathIndex].Position;

                var targetPos = new float3(pathPos.x, pathPos.y, 0);
                var moveDir = math.normalizesafe(targetPos - translation.Value);
                var moveSpeed = 3f;

                translation.Value += moveDir * moveSpeed * Time.DeltaTime;

                if (math.distance(translation.Value, targetPos) < .1f)
                {
                    followPathData.PathIndex--;
                }
            }
        });
    }
}
