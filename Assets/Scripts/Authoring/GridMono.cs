using GridSystem;
using Pathfinding;
using UnityEngine;

public class GridMono : MonoSingleton<GridMono>
{
    [SerializeField] private GridConfig GridConfig;
    private Node[] _gridArray;

    public Node[] GridArray => _gridArray;

    private void Start()
    {
        var grid = new Grid<Node>(GridConfig.GridSize.x, GridConfig.GridSize.y, 1f, Vector3.zero, null, (grid, x, y)
            => new Node(x, y, CalculateWorldIndex(x, y, GridConfig.GridSize.x), true, -1, 0, 0, 0, NodeType.None));

        var gridArr = grid.GetGridArray();
        _gridArray = new Node[GridConfig.GridSize.x * GridConfig.GridSize.y];
        foreach (var item in grid.GetGridArray())
        {
            _gridArray[item.WorldIndex] = item;
        }
    }
    private int CalculateWorldIndex(int x, int y, int gridWidth) => x + y * gridWidth;

}
