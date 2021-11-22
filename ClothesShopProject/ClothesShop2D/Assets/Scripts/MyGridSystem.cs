using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;


namespace GripPlacement
{
    public enum TileType
    {
        Empty,
        White,
        Green,
        Red
    };

    public class MyGridSystem : MonoBehaviour
    {
        public static MyGridSystem current;

        public static Dictionary<TileType, TileBase> TileTypeToResourceTileDict = new Dictionary<TileType, TileBase>();

        public GridLayout gridLayout;
        public Tilemap mainTilemap;
        public Tilemap tempTilemap;

        Placeable placeObject = null;
        Vector3 previousPosition;
        BoundsInt previousArea;

        Vector3 offset = new Vector3(0.5f, 0.5f, 0);

        void Awake()
        {
            current = this;
        }

        void Start()
        {
            string path = @"Tiles\";
            TileTypeToResourceTileDict.Add(TileType.Empty, null);
            TileTypeToResourceTileDict.Add(TileType.White, Resources.Load<TileBase>(path + "white"));
            TileTypeToResourceTileDict.Add(TileType.Green, Resources.Load<TileBase>(path + "green"));
            TileTypeToResourceTileDict.Add(TileType.Red, Resources.Load<TileBase>(path + "red"));
        }

        public void Initialize(GameObject prefab)
        {
            placeObject = Instantiate<GameObject>(prefab,
                    Vector3.zero, Quaternion.identity).GetComponent<Placeable>();
            Vector3 cellPos = gridLayout.LocalToCell(placeObject.transform.position);
            placeObject.transform.localPosition = gridLayout.CellToLocalInterpolated(cellPos + offset);
            UpdateTiles();
        }

        private void Update()
        {
            if (placeObject == null) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (!placeObject.placed)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    // get the mouse position and the cell from the main tilemap
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    // get the cell position from the main tilemap
                    Vector3Int cellPosition = gridLayout.LocalToCell(mousePos);

                    if (previousPosition != cellPosition) // only if it's a new position do we move the object and update the tiles
                    {
                        // move the placeable object
                        // was originally in local space
                        placeObject.transform.localPosition = gridLayout.CellToLocalInterpolated(cellPosition +
                                    offset); // to make the object stand in the middle of the cell

                        previousPosition = cellPosition;
                        UpdateTiles();

                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    // place object
                    if (placeObject.CanBePlaced())
                        placeObject.Place();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ClearArea();
                    Destroy(placeObject.gameObject);
                }
            }
        }

        public bool CheckIfTilesAreAvailable(BoundsInt area)
        {
            TileBase[] tiles = GetTilesFromArea(area, mainTilemap);

            foreach(TileBase tile in tiles)
            {
                // if not every tile in the area is white, then return false
                if (tile != TileTypeToResourceTileDict[TileType.White])
                {
                    print("Cannot be placed here");
                    return false;
                }
            }
            print("Placing object");
            return true;

        }

        public void TakeArea(BoundsInt areaUnderneath)
        {
            // set the tiles on the temp tilemap to empty
            SetTilesFromArea(areaUnderneath, TileType.Empty, tempTilemap);
            // set the tiles on the main tilemap to green TODO: make a "confirmed" or "selected" tile
            SetTilesFromArea(areaUnderneath, TileType.Green, mainTilemap);
        }

        private void SetTilesFromArea(BoundsInt area, TileType tileType, Tilemap tilemap)
        {
            int size = area.size.x * area.size.y * area.size.z;
            TileBase[] tiles = new TileBase[size];
            FillTiles(tiles, tileType);
            tilemap.SetTilesBlock(area, tiles);
        }

        private void ClearArea()
        {
            // create an array the size of the previous area
            TileBase[] tilesToClear = new TileBase[previousArea.size.x * previousArea.size.y * previousArea.size.z];
            // fill that array with the tiles that were in the previous area
            FillTiles(tilesToClear, TileType.Empty);
            tempTilemap.SetTilesBlock(previousArea, tilesToClear);

        }

        private void UpdateTiles()
        {
            // clear the tiles of the previous area
            ClearArea();

            // get the tiles under the placeable object
            // but first set its area to the new position
            placeObject.areaUnderneath.position = gridLayout.WorldToCell(placeObject.transform.position);

            // get the tiles from the main and temp tilemaps in order to compare them
            TileBase[] tilesUnderneath_MainTilemap = GetTilesFromArea(placeObject.areaUnderneath, mainTilemap);
            // compare them with the tiles in the tempTilemap
            int size = tilesUnderneath_MainTilemap.Length;
            TileBase[] tilesUnderneath_TempTilemap = new TileBase[size];

            // go through each tile on the main tilemap and see if it is white
            for (int i = 0; i < tilesUnderneath_MainTilemap.Length; i++)
            {
                // if tiles on the main tilemap are white
                if (tilesUnderneath_MainTilemap[i] == TileTypeToResourceTileDict[TileType.White])
                {
                    // then set the same tiles in the temp tilemap to green
                    // we aren't ready to set them to main tilemap until we confirm
                    tilesUnderneath_TempTilemap[i] = TileTypeToResourceTileDict[TileType.Green];
                }
                else
                {
                    FillTiles(tilesUnderneath_TempTilemap, TileType.Red);
                    break;

                }
            }

            // now actually set the temp tile array to the temp tilemap
            tempTilemap.SetTilesBlock(placeObject.areaUnderneath, tilesUnderneath_TempTilemap);
            // set the new previous area
            previousArea = placeObject.areaUnderneath;
        }

        private static void FillTiles(TileBase[] array, TileType tileType)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = TileTypeToResourceTileDict[tileType];
            }
        }

        // AKA GetTilesBlock()
        private static TileBase[] GetTilesFromArea(BoundsInt area, Tilemap tilemap)
        {
            TileBase[] tileArray = new TileBase[area.size.x * area.size.y * area.size.z];
            int counter = 0;

            foreach (Vector3Int vec in area.allPositionsWithin)
            {
                // zero out the vec
                Vector3Int pos = new Vector3Int(vec.x, vec.y, 0);
                tileArray[counter] = tilemap.GetTile(vec);
                counter++;
            }

            return tileArray;
        }
    }
}
