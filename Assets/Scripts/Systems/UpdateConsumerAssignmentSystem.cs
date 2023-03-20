using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(BuildingRandomSystem))]

public partial class UpdateConsumerAssignmentSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Enabled = false;
        Entities
            .WithAll<ConsumerTag>()
            .ForEach((Entity consumerEntity, ConsumerData consumer, ref Translation consumerPos) =>
            {
                if (consumer.AssignedProducer != Entity.Null)
                    return;

                Entity newProducer = FindClosestProducer(new float3(consumerPos.Value.x, consumerPos.Value.y, consumerPos.Value.z));

                if (newProducer != Entity.Null)
                {
                    // update the consumer's assignment
                    consumer.AssignedProducer = newProducer;
                    UnityEngine.Debug.Log("Assigned!");
                }
                else
                {
                    // no available producers found, reset assignment
                    consumer.AssignedProducer = Entity.Null;
                    UnityEngine.Debug.Log($"still null");
                }
            }).WithoutBurst().Run();

        Dependency.Complete();
    }

    private Entity FindClosestProducer(float3 consumerPos)
    {
        var closestTargetEntity = Entity.Null;
        var closestTargetPos = float3.zero;

        // find closest producer
        Entities
            .WithAll<ProducerTag>()
            .WithNone<RoadTag>()
            .ForEach((Entity targetEntity, ref Translation tTranslation) =>
            {
                if (closestTargetEntity == Entity.Null)
                {
                    // no target
                    closestTargetEntity = targetEntity;
                    closestTargetPos = tTranslation.Value;
                }
                else
                {
                    if (math.distance(consumerPos, tTranslation.Value) < math.distance(consumerPos, closestTargetPos))
                    {
                        // closer target
                        closestTargetEntity = targetEntity;
                        closestTargetPos = tTranslation.Value;
                    }
                }
            }).WithoutBurst().Run();

        return closestTargetEntity;
    }
}
