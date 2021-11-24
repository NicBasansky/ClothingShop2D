using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public enum TileType
{
    empty,
    white,
    green,
    red
};

public class TutorialGridBuildingSystem : MonoBehaviour
{
    public static TutorialGridBuildingSystem current;

    public GridLayout gridLayout;
    public Tilemap mainTilemap;
    public Tilemap tempTilemap;

    // This will be used to compare the tiles or set them to the grid
    private static Dictionary<TileType, TileBase> tileBasesDict = new Dictionary<TileType, TileBase>();

    private Furniture tempFurniture;
    private Vector3 previousPos;
    private BoundsInt prevArea;
    private Vector3 offset = new Vector3(0.5f, 0f, 0f);

    // Unity Methods
    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        string tilePath = @"Tiles\";
        tileBasesDict.Add(TileType.empty, null);
        tileBasesDict.Add(TileType.white, Resources.Load<TileBase>(tilePath + "white"));
        tileBasesDict.Add(TileType.green, Resources.Load<TileBase>(tilePath + "green"));
        tileBasesDict.Add(TileType.red, Resources.Load<TileBase>(tilePath + "red"));
    }

    private void Update() 
    {
        if (!tempFurniture) return;

        if (Input.GetMouseButtonDown(0))
        {
            // if not over UI gameobject
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // if not placed, then move it
            if (!tempFurniture.placed)
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPos = gridLayout.LocalToCell(touchPos);

                // if the positions are the same then we don't want to colour the tile each time
                if (cellPos != previousPos)
                {
                    // move the furniture
                    tempFurniture.transform.localPosition = gridLayout.CellToLocalInterpolated(cellPos
                        + offset); // small offset to make the sprite
                                    // snap to the centre of the grid cell TODO: check with other furniture
                    previousPos = cellPos;
                    // colour the tiles underneath
                    ColourTiles();
                }
            }

        }
        else if (Input.GetMouseButtonDown(1)) // right click to place
        {
            if (tempFurniture.CanBePlaced())
            {
                tempFurniture.Place();
            }                
                
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearPreviousAreaOnTempTilemap();
            Destroy(tempFurniture.gameObject);
        }
        // else if escape, then destroy temp furniture
    }

    // called from button
    public void InitializeWithFurniture(GameObject furniture)
    {
        tempFurniture = Instantiate<GameObject>(furniture, Vector3.zero, Quaternion.identity).GetComponent<Furniture>();
        Vector3 cellPos = gridLayout.LocalToCell(tempFurniture.transform.position);
        tempFurniture.transform.localPosition = gridLayout.CellToLocalInterpolated(cellPos + offset);
        ColourTiles();
    }

    private void ClearPreviousAreaOnTempTilemap()
    {
        TileBase[] arrayToClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FillTiles(arrayToClear, TileType.empty);
        tempTilemap.SetTilesBlock(prevArea, arrayToClear);
    }

    private void ColourTiles()
    {
        ClearPreviousAreaOnTempTilemap();

        tempFurniture.areaUnderneath.position = gridLayout.WorldToCell(tempFurniture.gameObject.transform.position);
        BoundsInt furnitureArea = tempFurniture.areaUnderneath;

        // using our own GetTilesBlock() get the tiles within the BoundsInt area
        TileBase[] furnitureTilesOnMainTilemap = GetTilesBlock(furnitureArea, mainTilemap);

        int size = furnitureTilesOnMainTilemap.Length;
        TileBase[] tilesUnderOnTempTileMap = new TileBase[size];

        // This will help us determine if a placement is possible here

        //go through each tile in the main tile map and see if it is white
        for (int i = 0; i < furnitureTilesOnMainTilemap.Length; i++)
        {
            if (furnitureTilesOnMainTilemap[i] == tileBasesDict[TileType.white])
            {
                tilesUnderOnTempTileMap[i] = tileBasesDict[TileType.green];
            }
            else
            {
                // if the furniture is over any tile that is not white then make all the tiles 
                // red and break out of the loop
                FillTiles(tilesUnderOnTempTileMap, TileType.red);
                break;
            }
        }

        tempTilemap.SetTilesBlock(furnitureArea, tilesUnderOnTempTileMap);
        prevArea = furnitureArea;
    }

    // TileMap Management

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] tileBaseArray = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach(Vector3Int vec in area.allPositionsWithin)
        {
            // get the position of a single point
            Vector3Int pos = new Vector3Int(vec.x, vec.y, 0);
            tileBaseArray[counter] = tilemap.GetTile(pos);
            counter++;
        }

        return tileBaseArray;
    }

    private static void SetTilesBlock(BoundsInt area, TileType type, Tilemap tilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
    }

    /// <summary>
    /// Fill the incoming array with tiles of a particular TileType
    /// </summary>
    /// <param name="tileBaseArray"></param>
    /// <param name="type"></param>
    private static void FillTiles(TileBase[] tileBaseArray, TileType type)
    {
        for (int i = 0; i < tileBaseArray.Length; i++)
        {
            // fill the array with the tileBases from the dictionary using type as key
            tileBaseArray[i] = tileBasesDict[type];
        }
    }

    // Building placement
    public bool CanAcceptPlacementArea(BoundsInt area)
    {
        TileBase[] tilesArray = GetTilesBlock(area, mainTilemap);
        foreach(TileBase tile in tilesArray)
        {
            if (tile != tileBasesDict[TileType.white])
            {
                Debug.Log("Can't place here");
                return false;
            }
        }

        return true;
         
    }

    public void TakeArea(BoundsInt area)
    {
        // set the tiles on the temporary tilemap to empty
        SetTilesBlock(area, TileType.empty, tempTilemap);
        // set the tiles on the main tilemap to green
        SetTilesBlock(area, TileType.green, mainTilemap);

    }

}