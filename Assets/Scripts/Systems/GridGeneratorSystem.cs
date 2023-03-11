using Unity.Burst;
using Unity.Collections;
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

        protected override void OnStartRunning()
        {
            Entities.WithAll<GridTag>().ForEach(e =>
            {
                EntityManager.AddComponentData(e, new GridTag
                {
                    GridArray = new NativeArray<Node>(GridSize.x * GridSize.y, Allocator.Persistent)
                });
            });

        }
        protected override void OnDestroy()
        {
            _blobAssetStore.Dispose();
            var entity = _query.GetSingletonEntity();
            var grid = EntityManager.GetComponentData<GridTag>(entity);
            grid.GridArray.Dispose();
            EntityManager.RemoveComponent<GridTag>(entity);
        }

        protected override void OnUpdate()
        {
            Enabled = false;

            var gridEntity = _query.GetSingletonEntity();
            var grid = EntityManager.GetComponentData<GridTag>(gridEntity);
            var goEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(GoPrefab,
                GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore));

            var job = new GridGeneratorJob
            {
                EntityManager = EntityManager,
                GoEntity = goEntity,
                GridSize = GridSize,
                GridArray = grid.GridArray
            };
            var handle = job.Schedule();
            handle.Complete();
        }
    }

    [BurstCompile]
    public struct GridGeneratorJob : IJob
    {
        public Entity GoEntity;
        public int2 GridSize;
        public EntityManager EntityManager;
        public NativeArray<Node> GridArray;

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
                    var worldIndex = CalculateWorldNodeIndex(i, j, x);
                    var isWalkable = true;
                    var prevIndex = -1;
                    var node = new Node(i, j, worldIndex, isWalkable, prevIndex, 0, 0, 0, NodeType.None);

                    GridArray[worldIndex] = node;

                    var newEntity = ecb.Instantiate(GoEntity);
                    ecb.AddComponent(newEntity, node);
                    ecb.AddComponent<Translation>(newEntity);
                    ecb.SetComponent(newEntity, new Translation { Value = new float3(i, j, 0) });
                }
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();

        }

        private int CalculateWorldNodeIndex(int x, int y, int gridWidth) => x + y * gridWidth;
    }
}