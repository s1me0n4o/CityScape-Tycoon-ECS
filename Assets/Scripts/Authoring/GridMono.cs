using GridSystem;
using UnityEngine;

public class GridMono : MonoBehaviour
{
    [SerializeField] private GridConfig GridConfig;


    private void Start()
    {

        var grid = new Grid<int>(GridConfig.GridSize.x, GridConfig.GridSize.y, 1f, Vector3.zero, null, (grid, x, y) => 0);
    }

}
