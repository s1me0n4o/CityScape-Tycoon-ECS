// using Unity.Entities;
//
// [UpdateAfter(typeof(VehicleSpawnSystem))]
// public partial class VehicleReachTargetSystem : SystemBase
// {
//     private float _minDist = .1f;
//     private float _moveSpeed = 3f;
//     private float _targetZPos = -0.1f;
//
//     protected override void OnUpdate()
//     {
//         //var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
//         //Entities
//         //    .WithAll<VehicleReachTargetEventData>()
//         //    .WithAll<VehicleTag>()
//         //    .ForEach((Entity entity, DynamicBuffer<PathPositionBuffer> buffer, ref Translation translation, ref FollowPathData followPathData) =>
//         //    {
//         //        if (Input.GetMouseButtonDown(0))
//         //        {
//         //            var lastIndex = buffer.Length - 1;
//         //            Debug.Log("Last " + (int)buffer[lastIndex].Position.x + ", " + (int)buffer[lastIndex].Position.y);
//         //            Debug.Log("First" + (int)buffer[0].Position.x + ", " + (int)buffer[0].Position.y);
//         //            Debug.Log("current" + (int)translation.Value.x + ", " + translation.Value.y);
//         //            ecb.AddComponent<PathfindingParams>(entity);
//         //            ecb.SetComponent<PathfindingParams>(entity, new PathfindingParams
//         //            {
//         //                StartPosition = new int2((int)translation.Value.x, (int)translation.Value.y),
//         //                EndPosition = new int2((int)buffer[0].Position.x, (int)buffer[0].Position.y)
//         //            });
//         //        }
//
//
//         //        //UnityEngine.Debug.Log("test");
//         //        // Handle the event here
//
//         //        // decreace producer with one
//         //        // increace customer with one
//         //        // move vehicle to the start pos
//
//
//         //        // Remove the event component from the entity to indicate that the event has been handled
//         //    }).WithoutBurst().Run();
//
//         //ecb.Dispose();
//     }
// }
