using Unity.Collections;
using Unity.Entities;

public partial class RobinRoundDispatchSystem : SystemBase
{
    private int _lastAssignedIndex = 0;
    private int _requiredProduct = 1;

    protected override void OnUpdate()
    {
        var consumerEntities = EntityManager.CreateEntityQuery(typeof(ConsumerData)).ToEntityArray(Allocator.TempJob);
        var producerEntities = EntityManager.CreateEntityQuery(typeof(ProducerData)).ToEntityArray(Allocator.TempJob);

        // calculate total available product count from all producers
        var totalAvailableProduct = 0;
        for (var i = 0; i < producerEntities.Length; i++)
        {
            var producerData = EntityManager.GetComponentData<ProducerData>(producerEntities[i]);
            totalAvailableProduct += producerData.CurrentAmount;
        }

        // calculate required product count based on available products and number of producers
        var numProducers = producerEntities.Length;
        _requiredProduct = totalAvailableProduct / numProducers;
        if (totalAvailableProduct % numProducers > 0)
            _requiredProduct++;

        Entities
            .WithAll<ProducerTag>()
            .ForEach((Entity producerEntity, ref ProducerData producer) =>
            {
                if (producer.CurrentAmount <= 0)
                    return;

                var availableProduct = producer.CurrentAmount;

                if (availableProduct < _requiredProduct) 
                    return;
                
                // enough product available, assign the next consumer to the producer
                var consumerEntity = consumerEntities[_lastAssignedIndex % consumerEntities.Length];
                _lastAssignedIndex++;

                var consumerData = EntityManager.GetComponentData<ConsumerData>(consumerEntity);
                
                if (consumerData.SendVehicle)
                    return;
                
                consumerData.AssignedProducer = producerEntity;
                consumerData.ProductCount = _requiredProduct;
                consumerData.SendVehicle = true;
                EntityManager.SetComponentData(consumerEntity, consumerData);
                //_hasBeenSent = true;
            }).WithoutBurst().Run();

        consumerEntities.Dispose();
        producerEntities.Dispose();
    }
}
