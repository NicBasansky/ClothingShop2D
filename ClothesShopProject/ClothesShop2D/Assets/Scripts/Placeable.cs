using System;
using UnityEngine;

namespace GripPlacement
{
    public class Placeable : MonoBehaviour
    {
        [Tooltip("z value must be at least 1")]
        public BoundsInt areaUnderneath;
        public bool placed { get; private set; }


        public bool CanBePlaced()
        {

            return MyGridSystem.current.CheckIfTilesAreAvailable(areaUnderneath);
        }

        public void Place()
        {
            // set the tiles on main tilemap to those on the temp in the area underneath placeable object
            placed = true;
            MyGridSystem.current.TakeArea(areaUnderneath);

        }
    }
}