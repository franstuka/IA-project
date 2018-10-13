using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour { //By default this is for a quad grid

    #region Singleton

    public static GridMap instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one instance of grid is trying to active");
            return;
        }

        instance = this;
    }
    #endregion

    public Cell[,] grid;

    [SerializeField] private Vector2 WorldSize;
    [SerializeField] private float CellRadius;
    private List<Vector2Int> temporalGridObjects;
    private LayerMask bloquedMask;
    private LayerMask chestMask;
    private LayerMask exitMask;
    private LayerMask enemyMask;
    private float cellDiameter;
    private int gridSizeX;
    private int gridSizeY;


    private void Start()
    {
        bloquedMask = LayerMask.GetMask("Unwalkable");
        chestMask = LayerMask.GetMask("Chest");
        exitMask = LayerMask.GetMask("Exit");
        enemyMask = LayerMask.GetMask("Enemy");
        temporalGridObjects = new List<Vector2Int>();
        cellDiameter = CellRadius * 2;
        gridSizeX = Mathf.RoundToInt(WorldSize.x / cellDiameter);
        gridSizeY = Mathf.RoundToInt(WorldSize.y / cellDiameter);
        //Debug.Log(gridSizeX);
        CreateGrid();
        grid[0, 1].CellType = CellTypes.chest;
        UpdateEnemyPositions();
    }

    private void Update()
    {
        UpdateEnemyPositions();
    }

    private void CreateGrid()
    {
        grid = new Cell[gridSizeX, gridSizeY];
        Vector3 gridBottonLeft = transform.position - Vector3.right * WorldSize.x / 2 - Vector3.forward * WorldSize.y / 2;
        
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                //Debug.Log(x + ", " + y);
                Vector3 worldPoint = gridBottonLeft + Vector3.right * (x * cellDiameter + CellRadius) + Vector3.forward * (y * cellDiameter + CellRadius);

                if(Physics.CheckBox(worldPoint,Vector3.one * CellRadius,Quaternion.identity,bloquedMask)) //celltypes
                {
                    grid[x, y] = new Cell(CellTypes.blocked, worldPoint, 0);
                }
                else if (Physics.CheckBox(worldPoint, Vector3.one * CellRadius, Quaternion.identity, chestMask)) 
                {
                    grid[x, y] = new Cell(CellTypes.chest, worldPoint, 0);
                }
                else if (Physics.CheckBox(worldPoint, Vector3.one * CellRadius, Quaternion.identity, exitMask)) 
                {
                    grid[x, y] = new Cell(CellTypes.exit, worldPoint, 0);
                }
                /*else if (Physics.CheckBox(worldPoint, Vector3.one * CellRadius, Quaternion.identity, enemyMask, QueryTriggerInteraction.Ignore))
                {
                    grid[x, y] = new Cell(CellTypes.enemy, worldPoint, 0 );
                }*/
                else//last else
                {
                    grid[x, y] = new Cell(CellTypes.emphy, worldPoint, 0);
                }
            }
        }
    }

    private void UpdateEnemyPositions()
    {
        RaycastHit[] enemies = Physics.BoxCastAll(transform.position, Vector3.one * Mathf.Max(WorldSize.x, WorldSize.y), Vector3.one, 
            Quaternion.identity, Mathf.Max(WorldSize.x, WorldSize.y) * 2, enemyMask, QueryTriggerInteraction.Ignore);
        CleanNonStaticElementsOnGrid();
        Vector2Int pos;
        for (int i = 0; i<enemies.Length; i++)
        {
            pos = CellCordFromWorldPoint(enemies[i].collider.gameObject.transform.position);
            grid[pos.x, pos.y].CellType = CellTypes.enemy;
            temporalGridObjects.Add(new Vector2Int(pos.x, pos.y));
        }
    }
    private void CleanNonStaticElementsOnGrid()
    {
        for(int i = 0; i< temporalGridObjects.Count; i++)
        {
            grid[temporalGridObjects[i].x, temporalGridObjects[i].y].CellType = CellTypes.emphy;  
        }
        temporalGridObjects = new List<Vector2Int>();
    }

    public Cell CellFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + WorldSize.x / 2) / WorldSize.x;
        float percentY = (worldPosition.z + WorldSize.y / 2) / WorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public Vector2Int CellCordFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + WorldSize.x / 2) / WorldSize.x;
        float percentY = (worldPosition.z + WorldSize.y / 2) / WorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return new Vector2Int(x, y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(WorldSize.x, 1, WorldSize.y));

        if(grid != null)
        {
            foreach (Cell n in grid)
            {
                if (n.CellType == CellTypes.emphy)
                {
                    Gizmos.color = Color.white;
                }
                else if (n.CellType == CellTypes.blocked)
                {
                    Gizmos.color = Color.gray;
                }
                else if (n.CellType == CellTypes.enemy)
                {
                    Gizmos.color = Color.red;
                }
                else if (n.CellType == CellTypes.chest)
                {
                    Gizmos.color = Color.yellow;
                }
                else if (n.CellType == CellTypes.exit)
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = Color.black;
                }

                Gizmos.DrawCube(n.GlobalPosition, Vector3.one * (cellDiameter *19/20 ));
            }
        }

    }

    public int GetGridSizeX()
    {
        return gridSizeX;
    }

    public int GetGridSizeY()
    {
        return gridSizeY;
    }
}
