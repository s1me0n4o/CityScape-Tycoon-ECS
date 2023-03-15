using GridSystem;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class GridMono : MonoSingleton<GridMono>
{
    [SerializeField] private GridConfig GridConfig;
    private Grid<NodeMono> _grid;

    public Grid<NodeMono> Grid => _grid;
    public List<NodeMono> Buildings => _buildings;

    private List<NodeMono> _buildings;

    private void Start()
    {
        _grid = new Grid<NodeMono>(GridConfig.GridSize.x, GridConfig.GridSize.y, 1f, Vector3.zero, null, (grid, x, y)
            => new NodeMono());
    }
}
