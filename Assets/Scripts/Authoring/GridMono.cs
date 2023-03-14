using GridSystem;
using Pathfinding;
using UnityEngine;

public class GridMono : MonoSingleton<GridMono>
{
    [SerializeField] private GridConfig GridConfig;
    private Grid<Node> _grid;

    public Grid<Node> Grid => _grid;

    private void Start()
    {
        _grid = new Grid<Node>(GridConfig.GridSize.x, GridConfig.GridSize.y, 1f, Vector3.zero, null, (grid, x, y)
            => new Node());
    }
}
