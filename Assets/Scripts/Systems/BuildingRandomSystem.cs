using Pathfinding;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(GridGeneratorSystem))]
public partial class BuildingRandomSystem : SystemBase
{
    protected override void OnStartRunning()
    {
        var buildingData = GetSingleton<BuildingParamsData>();
        for (int i = 0; i < buildingData.NumberOfPrefabsProducer; i++)
        {
            var newEntity = EntityManager.Instantiate(buildingData.EntityProducerPrefab);
            EntityManager.AddComponent<BuildingParamsData>(newEntity);
            EntityManager.AddComponent<ProducerTag>(newEntity);
        }
        for (int i = 0; i < buildingData.NumberOfPrefabsConsumer; i++)
        {
            var newEntity = EntityManager.Instantiate(buildingData.EntityConsumerPrefab);
            EntityManager.AddComponent<BuildingParamsData>(newEntity);
            EntityManager.AddComponent<ConsumerTag>(newEntity);
        }

        var seed = (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        Entities.ForEach((Entity entity, int entityInQueryIndex, ref RandomComponent randomData) =>
        {
            randomData.Value = Random.CreateFromIndex((uint)entityInQueryIndex + seed);
            var random = randomData.Value.NextInt(0, GridMono.Instance.GridArray.Length);
            while (true)
            {
                if (!GridMono.Instance.GridArray[random].IsTaken)
                    break;
                randomData.Value = Random.CreateFromIndex((uint)entityInQueryIndex + seed);
                random = randomData.Value.NextInt(0, GridMono.Instance.GridArray.Length);
            }
            GridMono.Instance.GridArray[random].TakeNode(); // not working have to get the entity for that node
            randomData.RandomIndex = random;
        }).Run();
    }

    protected override void OnUpdate()
    {
        Enabled = false;

        Entities
            .ForEach((ref Translation translation, ref RandomComponent randomData, ref BuildingParamsData buildingParams) =>
        {
            MoveBuilding(ref translation, ref randomData);
            translation.Value.y += buildingParams.Offset.y;
            translation.Value.x += buildingParams.Offset.x;

        }).WithoutBurst().Run();
    }

    private void MoveBuilding(ref Translation translation, ref RandomComponent randomData)
    {
        translation.Value.x = GridMono.Instance.GridArray[randomData.RandomIndex].X;
        translation.Value.y = GridMono.Instance.GridArray[randomData.RandomIndex].Y;
    }

}
