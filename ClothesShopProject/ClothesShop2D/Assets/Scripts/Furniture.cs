using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public bool placed { get; private set; }

    // the area underneath the furniture
    [Tooltip("the area of tiles under the furniture")]
    public BoundsInt areaUnderneath;

    public bool CanBePlaced()
    {
        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(transform.position);
        
        BoundsInt areaTemp = areaUnderneath;
        areaTemp.position = positionInt;

        if (GridBuildingSystem.current.CanAcceptPlacementArea(areaTemp))
        {
            return true;
        }
        return false;
    }

    public void Place()
    {
        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(transform.position);

        BoundsInt areaTemp = areaUnderneath;
        areaTemp.position = positionInt;

        placed = true;

        GridBuildingSystem.current.TakeArea(areaTemp);
    }

}
