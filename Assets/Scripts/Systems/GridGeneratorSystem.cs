using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Pathfinding
{
    public partial class GridGeneratorSystem : ComponentSystem
    {
        public GameObject GoPrefab;
        public int2 GridSize;
        private BlobAssetStore _blobAssetStore;
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _blobAssetStore = new BlobAssetStore();
            _query = GetEntityQuery(typeof(GridTag));
        }

        protected override void OnDestroy()
        {
            _blobAssetStore.Dispose();
        }

        protected override void OnUpdate()
        {
            Enabled = false;

            var goEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(GoPrefab,
                GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore));
            GenerateGrid(GridSize);
            var job = new GridGeneratorJob
            {
                EntityManager = EntityManager,
                GoEntity = goEntity,
                GridSize = GridSize,
            };
            var handle = job.Schedule();
            handle.Complete();
        }

        private void GenerateGrid(int2 gridSize)
        {
            var grid = GridMono.Instance.Grid;

            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    var node = grid.GetGridObject(i, j);
                    node.X = i;
                    node.Y = j;
                    node.WorldIndex = CalculateWorldNodeIndex(i, j, gridSize.x);
                    node.IsWalkable = grid.GetGridObject(i, j).IsWalkable;
                    node.PreviousNodeIndex = -1;

                    grid.SetGridObject(i, j, node);
                }
            }
        }

        public static int CalculateWorldNodeIndex(int x, int y, int gridWidth) => x + y * gridWidth;

        [BurstCompile]
        public struct GridGeneratorJob : IJob
        {
            public Entity GoEntity;
            public int2 GridSize;
            public EntityManager EntityManager;

            [BurstCompile]
            public void Execute()
            {
                var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

                var x = GridSize.x;
                var y = GridSize.y;

                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        var newEntity = ecb.Instantiate(GoEntity);
                        ecb.AddComponent<Translation>(newEntity);
                        ecb.SetComponent(newEntity, new Translation { Value = new float3(i, j, 0) });
                    }
                }

                ecb.Playback(EntityManager);
                ecb.Dispose();
            }
        }
    }
}