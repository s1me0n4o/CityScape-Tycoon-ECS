using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "GridProps", menuName = "ScriptableObjects")]
public class GridConfig : ScriptableObject
{
    public int2 GridSize;
}

