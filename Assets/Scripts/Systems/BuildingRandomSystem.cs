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
            CreateProducer(buildingData);
        }
        for (int i = 0; i < buildingData.NumberOfPrefabsConsumer; i++)
        {
            CreateConsumer(buildingData);
        }

        var seedX = (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        var seedY = (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        Entities.ForEach((Entity entity, int entityInQueryIndex, ref RandomComponent randomData) =>
        {
            randomData.ValueX = Random.CreateFromIndex((uint)entityInQueryIndex + seedX);
            randomData.ValueY = Random.CreateFromIndex((uint)entityInQueryIndex + seedY);
            var randomX = randomData.ValueX.NextInt(0, GridMono.Instance.Grid.GetGridWidth());
            var randomY = randomData.ValueY.NextInt(0, GridMono.Instance.Grid.GetGridHeight());
            while (true)
            {
                var grid = GridMono.Instance.Grid;
                if (!GridMono.Instance.Grid.GetGridObject(randomX, randomY).IsTaken)
                    break;
                randomData.ValueX = Random.CreateFromIndex((uint)entityInQueryIndex + seedX);
                randomData.ValueY = Random.CreateFromIndex((uint)entityInQueryIndex + seedY);
                randomX = randomData.ValueX.NextInt(0, GridMono.Instance.Grid.GetGridWidth());
                randomY = randomData.ValueY.NextInt(0, GridMono.Instance.Grid.GetGridHeight());
            }

            randomData.RandomValueX = randomX;
            randomData.RandomValueY = randomY;


            // take the node
            var node = GridMono.Instance.Grid.GetGridObject(randomX, randomY);
            node.TakeNode(UnitType.Building);
            //GridMono.Instance.AddBuilding(node);

        }).Run();
    }

    private void CreateConsumer(BuildingParamsData buildingData)
    {
        var newEntity = EntityManager.Instantiate(buildingData.EntityConsumerPrefab);
        EntityManager.AddComponent<BuildingParamsData>(newEntity);
        EntityManager.AddComponent<ConsumerTag>(newEntity);
        EntityManager.AddComponent<PathPositionAuthoring>(newEntity);
    }

    private void CreateProducer(BuildingParamsData buildingData)
    {
        var newEntity = EntityManager.Instantiate(buildingData.EntityProducerPrefab);
        EntityManager.AddComponent<BuildingParamsData>(newEntity);
        EntityManager.AddComponent<ProducerTag>(newEntity);

        EntityManager.AddComponent<PathPositionAuthoring>(newEntity);
        //EntityManager.AddComponent<PathfindingParams>(newEntity);
        EntityManager.AddComponent<PathPositionBuffer>(newEntity);
        EntityManager.AddComponentData(newEntity, new FollowPathData { PathIndex = -1 });
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
        translation.Value.x = GridMono.Instance.Grid.GetGridObject(randomData.RandomValueX, randomData.RandomValueY).X;
        translation.Value.y = GridMono.Instance.Grid.GetGridObject(randomData.RandomValueX, randomData.RandomValueY).Y;
    }

}
