using UnityEngine;

public class Building : BuildingBase
{
    private void Start()
    {
        var mat = GetComponentInChildren<Renderer>().material;
        switch (BuildingType)
        {
            case BuildingType.Consumer:
                Color = UnityEngine.Color.red;
                break;
            case BuildingType.Producer:
                Color = UnityEngine.Color.green;
                break;
            default:
                // code for other cases
                break;
        }
        mat.color = Color;
    }
}
