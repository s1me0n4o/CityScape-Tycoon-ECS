using Pathfinding;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(GridGeneratorSystem))]
public partial class BuildingRandomSystem : SystemBase
{
    // TODO: try to refactor this with Job in order to use Burst
    public int2 GridSize;
    private GridTag _grid;

    protected override void OnStartRunning()
    {
        var _query = GetEntityQuery(typeof(GridTag));
        var gridEntity = _query.GetSingletonEntity();
        _grid = EntityManager.GetComponentData<GridTag>(gridEntity);
        var hash = new HashSet<int>[5];
        var seed = (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        Entities.ForEach((Entity entity, int entityInQueryIndex, ref RandomComponent randomData) =>
        {
            randomData.Value = Random.CreateFromIndex((uint)entityInQueryIndex + seed);
            var random = randomData.Value.NextInt(0, _grid.GridArray.Length);
            while (true)
            {
                if (!_grid.GridArray[random].IsTaken)
                    break;
                UnityEngine.Debug.Log("Taken");
                randomData.Value = Random.CreateFromIndex((uint)entityInQueryIndex + seed);
                random = randomData.Value.NextInt(0, _grid.GridArray.Length);
            }
            _grid.GridArray[random].TakeNode(); // not working have to get the entity for that node
            // EntityManager.SetComponentData(_grid.GridArray[random], new GridComponent { IsTaken = true });
            randomData.RandomIndex = random;
        }).WithoutBurst().Run();
    }

    protected override void OnUpdate()
    {
        Enabled = false;

        Entities.ForEach((ref Translation translation, ref RandomComponent randomData, ref BuildingParams buildingParams) =>
        {
            MoveBuilding(ref translation, ref randomData);
            translation.Value.y += buildingParams.Offset.y;
            translation.Value.x += buildingParams.Offset.x;

        }).WithoutBurst().Run();
    }

    private void MoveBuilding(ref Translation translation, ref RandomComponent randomData)
    {
        translation.Value.x = _grid.GridArray[randomData.RandomIndex].X;
        translation.Value.y = _grid.GridArray[randomData.RandomIndex].Y;
    }

}
