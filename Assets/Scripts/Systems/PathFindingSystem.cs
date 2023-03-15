using Pathfinding;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(BuildingRandomSystem))]
public class PathFindingSystem : ComponentSystem
{
    private const int StraightCost = 10;
    private const int DiagonalCost = 14;

    protected override void OnUpdate()
    {
        var grid = GridMono.Instance.Grid;
        var gridSize = new int2(grid.GetGridWidth(), grid.GetGridHeight());

        var findPathJobList = new List<FindPathJob>();
        var jobHandleList = new NativeList<JobHandle>(Allocator.Temp);

        Entities.ForEach((Entity e, DynamicBuffer<PathPositionBuffer> buffer, ref PathfindingParams pathParams) =>
        {
            Debug.Log("FindPath");
            var findPathJob = new FindPathJob
            {
                StartPos = pathParams.StartPosition,
                EndPos = pathParams.EndPosition,
                Nodes = GenerateGrid(gridSize),
                GridSize = gridSize,
                FollowPathDataFromEntity = GetComponentDataFromEntity<FollowPathData>(),
                CurrentEntity = e,
            };

            findPathJobList.Add(findPathJob);
            jobHandleList.Add(findPathJob.Schedule());
            PostUpdateCommands.RemoveComponent<PathfindingParams>(e);
        });

        JobHandle.CompleteAll(jobHandleList);

        foreach (var item in findPathJobList)
        {
            new SetBufferPathJob
            {
                entity = item.CurrentEntity,
                GridSize = item.GridSize,
                Nodes = item.Nodes,
                PathfindingParamsFromEntity = GetComponentDataFromEntity<PathfindingParams>(),
                FollowPathDataFromEntity = GetComponentDataFromEntity<FollowPathData>(),
                PathPositionBufferFromEntity = GetBufferFromEntity<PathPositionBuffer>(),
            }.Run();
        }
    }

    private NativeArray<Node> GenerateGrid(int2 gridSize)
    {
        var grid = GridMono.Instance.Grid;
        var nodes = new NativeArray<Node>(gridSize.x * gridSize.y, Allocator.TempJob);

        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                var node = new Node();
                node.X = i;
                node.Y = j;
                node.WorldIndex = CalculateWorldNodeIndex(i, j, gridSize.x);
                node.GCost = int.MaxValue;

                node.IsWalkable = grid.GetGridObject(i, j).IsWalkable;
                node.PreviousNodeIndex = -1;

                nodes[node.WorldIndex] = node;
            }
        }
        return nodes;
    }

    private static int CalculateWorldNodeIndex(int x, int y, int gridWidth) => x + y * gridWidth;

    [BurstCompatible]
    private struct SetBufferPathJob : IJob
    {
        public int2 GridSize;
        [DeallocateOnJobCompletion] public NativeArray<Node> Nodes;
        public Entity entity;
        public ComponentDataFromEntity<PathfindingParams> PathfindingParamsFromEntity;
        public ComponentDataFromEntity<FollowPathData> FollowPathDataFromEntity;
        public BufferFromEntity<PathPositionBuffer> PathPositionBufferFromEntity;

        public void Execute()
        {
            var buffer = PathPositionBufferFromEntity[entity];
            buffer.Clear();

            var pathfindingParams = PathfindingParamsFromEntity[entity];
            var endNodeIndex = CalculateWorldNodeIndex(pathfindingParams.EndPosition.x, pathfindingParams.EndPosition.y, GridSize.x);
            var endNode = Nodes[endNodeIndex];
            if (endNode.PreviousNodeIndex == -1)
            {
                // no path
                Debug.Log("No path found!");
                FollowPathDataFromEntity[entity] = new FollowPathData { PathIndex = -1 };
            }
            else
            {
                // found it
                CalculatePath(Nodes, endNode, buffer);
                FollowPathDataFromEntity[entity] = new FollowPathData { PathIndex = buffer.Length - 1 };
            }
        }

        private void CalculatePath(NativeArray<Node> nodes, Node endNode, DynamicBuffer<PathPositionBuffer> pathBuffer)
        {
            if (endNode.PreviousNodeIndex == -1)
            {
                // no path
            }
            else
            {
                pathBuffer.Add(new PathPositionBuffer { Position = new int2(endNode.X, endNode.Y) });

                var currentNode = endNode;
                while (currentNode.PreviousNodeIndex != -1)
                {
                    var cameFromNode = nodes[currentNode.PreviousNodeIndex];
                    pathBuffer.Add(new PathPositionBuffer { Position = new int2(cameFromNode.X, cameFromNode.Y) });
                    currentNode = cameFromNode;
                }
            }
        }
    }

    [BurstCompatible]
    private struct FindPathJob : IJob
    {
        public int2 GridSize;

        public int2 StartPos;
        public int2 EndPos;

        public NativeArray<Node> Nodes;
        [NativeDisableContainerSafetyRestriction] public ComponentDataFromEntity<FollowPathData> FollowPathDataFromEntity;
        public Entity CurrentEntity;

        public void Execute()
        {
            for (int i = 0; i < Nodes.Length; i++)
            {
                var node = Nodes[i];
                node.PreviousNodeIndex = -1;
                node.HCost = CalculateDistanceCost(new int2(node.X, node.Y), EndPos);
                node.CalculateFCost();
                Nodes[i] = node;
            }

            var neighbourOffsetArray = new NativeArray<int2>(new int2[8], Allocator.Temp);
            neighbourOffsetArray[0] = new int2(-1, 0); // left
            neighbourOffsetArray[1] = new int2(+1, 0); // right
            neighbourOffsetArray[2] = new int2(0, +1); // up
            neighbourOffsetArray[3] = new int2(0, -1); // down
            neighbourOffsetArray[4] = new int2(-1, -1); // buttom left
            neighbourOffsetArray[5] = new int2(-1, +1); // top left
            neighbourOffsetArray[6] = new int2(+1, -1); // buttom right
            neighbourOffsetArray[7] = new int2(+1, +1); // top right

            var endNodeIndex = CalculateWorldNodeIndex(EndPos.x, EndPos.y, GridSize.x);

            var startNode = Nodes[CalculateWorldNodeIndex(StartPos.x, StartPos.y, GridSize.x)];
            startNode.SetGCost(0);
            startNode.CalculateFCost();
            Nodes[startNode.WorldIndex] = startNode; // using ref types -> update the array


            var openList = new NativeList<int>(Allocator.Temp);
            var closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.WorldIndex);

            while (openList.Length > 0)
            {
                var currentNodeIndex = GetLowestFCostNodeIndex(openList, Nodes);
                var currentNode = Nodes[currentNodeIndex];

                if (currentNodeIndex == endNodeIndex)
                {
                    // reached our destination
                    break;
                }

                // remove current node from open list
                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(currentNodeIndex);

                // cicle between neighbours
                for (int i = 0; i < neighbourOffsetArray.Length; i++)
                {
                    var neighbour = neighbourOffsetArray[i];
                    var neighbourPos = new int2(currentNode.X + neighbour.x, currentNode.Y + neighbour.y);
                    if (!IsValidPosition(neighbourPos, GridSize))
                    {
                        // neighbour is outside of the grid
                        continue;
                    }

                    var neighbourIndex = CalculateWorldNodeIndex(neighbourPos.x, neighbourPos.y, GridSize.x);

                    if (closedList.Contains(neighbourIndex))
                    {
                        // already checked that node
                        continue;
                    }

                    var neigbourNode = Nodes[neighbourIndex];
                    if (!neigbourNode.IsWalkable)
                    {
                        continue;
                    }

                    var currentNodePos = new int2(currentNode.X, currentNode.Y);
                    var tentitiveGCost = currentNode.GCost + CalculateDistanceCost(currentNodePos, neighbourPos);
                    if (tentitiveGCost < neigbourNode.GCost)
                    {
                        neigbourNode.SetPreviousNodeIndex(currentNodeIndex);
                        neigbourNode.SetGCost(tentitiveGCost);
                        neigbourNode.CalculateFCost();
                        Nodes[neighbourIndex] = neigbourNode;

                        if (!openList.Contains(neigbourNode.WorldIndex))
                        {
                            openList.Add(neigbourNode.WorldIndex);
                        }
                    }
                }
            }

            // dispose native arrays
            neighbourOffsetArray.Dispose();
            openList.Dispose();
            closedList.Dispose();
        }



        private int CalculateDistanceCost(int2 a, int2 b)
        {
            var x = math.abs(a.x - b.x);
            var y = math.abs(a.y - b.y);
            var remaining = math.abs(x - y);
            return DiagonalCost * math.min(x, y) + StraightCost * remaining;
        }

        private int GetLowestFCostNodeIndex(NativeArray<int> openList, NativeArray<Node> nodes)
        {
            var lowestCostNode = nodes[openList.FirstOrDefault()];
            for (int i = 0; i < openList.Length; i++)
            {
                var testNode = nodes[openList[i]];
                if (testNode.FCost < lowestCostNode.FCost)
                {
                    lowestCostNode = testNode;
                }
            }
            return lowestCostNode.WorldIndex;
        }

        private bool IsValidPosition(int2 gridPos, int2 gridSize)
        {
            return gridPos.x >= 0
                   && gridPos.y >= 0
                   && gridPos.x < gridSize.x
                   && gridPos.y < gridSize.y;
        }

        private NativeList<int2> CalculatePath(NativeArray<Node> nodes, Node endNode)
        {
            if (endNode.PreviousNodeIndex == -1)
            {
                return new NativeList<int2>(Allocator.Temp);
            }
            else
            {
                var path = new NativeList<int2>(Allocator.Temp);
                path.Add(new int2(endNode.X, endNode.Y));
                var currentNode = endNode;
                while (currentNode.PreviousNodeIndex != -1)
                {
                    var cameFromNode = nodes[currentNode.PreviousNodeIndex];
                    path.Add(new int2(cameFromNode.X, cameFromNode.Y));
                    currentNode = cameFromNode;
                }

                return path;
            }
        }
    }

}
