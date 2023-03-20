using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public partial class RobinRoundDispatchSystem : SystemBase
{
    private int _lastAssignedIndex = 0;
    private int _requiredProduct = 1;
    protected override void OnUpdate()
    {
        var consumerEntities = EntityManager.CreateEntityQuery(typeof(ConsumerData)).ToEntityArray(Allocator.TempJob);

        Entities
            .WithAll<ProducerTag>()
            .ForEach((Entity producerEntity, ref ProducerData producer, in Translation producerPos) =>
            {
                if (producer.CurrentAmount <= 0)
                    return;

                int availableProduct = producer.CurrentAmount;

                if (availableProduct >= _requiredProduct)
                {
                    // enough product available, assign the next consumer to the producer
                    var consumerEntity = consumerEntities[_lastAssignedIndex % consumerEntities.Length];
                    _lastAssignedIndex++;

                    var consumer = EntityManager.GetComponentData<ConsumerData>(consumerEntity);
                    var consumerTranslation = EntityManager.GetComponentData<Translation>(consumerEntity);
                    consumer.AssignedProducer = producerEntity;
                    consumer.ProductCount = _requiredProduct;
                    consumer.SendVehicle = true;
                    EntityManager.SetComponentData(consumerEntity, consumer);
                    UnityEngine.Debug.Log($"Send Vehicle from consumer {consumerEntity} - veh: {consumer.Vehicle}!");

                    producer.CurrentAmount -= _requiredProduct;
                }
            }).WithoutBurst().Run();

        consumerEntities.Dispose();
    }
}
