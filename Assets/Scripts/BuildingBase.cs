using UnityEngine;

public enum UnitType
{
    None,
    Building,
    Road,
    Vehicle
}

public enum BuildingType
{
    None,
    Producer,
    Consumer
}

public abstract class Unit : MonoBehaviour
{
    public UnitType Type;
}

public abstract class BuildingBase : Unit
{
    public Color Color;
    public BuildingType BuildingType;
}
