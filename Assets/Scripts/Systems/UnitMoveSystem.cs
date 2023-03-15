using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(BuildingRandomSystem))]
public class UnitMoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Entities
                .WithAll<ProducerTag>()
                .ForEach((Entity e) =>
            {
                EntityManager.SetComponentData(e, new PathfindingParams
                {
                    StartPosition = new int2(0, 0),
                    EndPosition = new int2(4, 0),
                });
            });
        }
    }
}
