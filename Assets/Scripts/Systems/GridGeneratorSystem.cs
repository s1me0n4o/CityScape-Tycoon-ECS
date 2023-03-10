using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Pathfinding
{
    public partial class GridGeneratorSystem : SystemBase
    {
        public GameObject GoPrefab;
        public int2 GridSize;

        protected override void OnUpdate()
        {
            Enabled = false;
            var blobAssetStore = new BlobAssetStore();
            Entity goEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(GoPrefab,
                GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));
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

                    var newEntity = ecb.Instantiate(goEntity);
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