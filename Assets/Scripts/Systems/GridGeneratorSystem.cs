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

        protected override void OnCreate()
        {
            _blobAssetStore = new BlobAssetStore();
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

            var job = new GridGeneratorJob
            {
                EntityManager = EntityManager,
                GoEntity = goEntity,
                GridSize = GridSize,
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
                    var node = new Node(i, j, worldIndex, isWalkable, prevIndex, 0, 0, 0);

                    var newEntity = ecb.Instantiate(GoEntity);
                    ecb.AddComponent<Node>(newEntity);
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