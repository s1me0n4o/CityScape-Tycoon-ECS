using Unity.Entities;

public partial class GenerateResourcesSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities
            .WithAll<ProducerTag>()
            .ForEach((Entity e, int entityInQueryIndex, ref ProducerData producerData) =>
        {
            producerData.TimerValue += deltaTime;
            if (producerData.TimerValue >= producerData.GenerateOnSeconds)
            {
                producerData.CurrentAmount += 1;
                producerData.TimerValue = 0f;
            }
        }).ScheduleParallel();

        Dependency.Complete();
    }
}
