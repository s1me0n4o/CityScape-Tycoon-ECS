using GridSystem;
using Pathfinding;
using UnityEngine;

public class GridMono : MonoSingleton<GridMono>
{
    [SerializeField] private GridConfig GridConfig;
    private Grid<NodeMono> _grid;

    public Grid<NodeMono> Grid => _grid;

    private void Start()
    {
        _grid = new Grid<NodeMono>(GridConfig.GridSize.x, GridConfig.GridSize.y, 1f, Vector3.zero, null, (grid, x, y)
            => new NodeMono());

        //foreach (var item in Grid.GetGridArray())
        //{
        //    item.SetWalkable(true);
        //}

        var node = _grid.GetGridObject(1, 0);
        node.SetWalkable(false);
        node = _grid.GetGridObject(1, 1);
        node.SetWalkable(false);
        node = _grid.GetGridObject(1, 2);
        node.SetWalkable(false);
    }
}
