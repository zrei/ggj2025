using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
public enum TileType
{
    CLEAN,
    NEUTRAL,
    DIRTY
}

public class TileState
{
    private TileType[,] m_TileStates;
    private int m_NumRows;
    private int m_NumCols;

    public TileState(int numRows, int numCols)
    {
        m_NumRows = numRows;
        m_NumCols = numCols;
        m_TileStates = new TileType[numRows, numCols];
        for (int r = 0; r < numRows; ++r)
        {
            for (int c = 0; c < numCols; ++c)
            {
                m_TileStates[r, c] = TileType.NEUTRAL;
            }
        }
    }

    public TileType GetTileTypeAtTile(Vector2Int tileCoordinates)
    {
        return m_TileStates[tileCoordinates.y, tileCoordinates.x];
    }

    public void SetTileTypeAtTile(Vector2Int tileCoordinates, TileType tileType)
    {
        m_TileStates[tileCoordinates.y, tileCoordinates.x] = tileType;
    }

    public float GetCleanedTiles(int numObstacles)
    {
        // TODO: Account for obstacles
        int numCleanedTiles = 0;
        for (int r = 0; r < m_NumRows; ++r)
        {
            for (int c = 0; c < m_NumCols; ++c)
            {
                if (GetTileTypeAtTile(new Vector2Int(m_NumCols, m_NumRows)) == TileType.CLEAN)
                    ++numCleanedTiles;
            }
        }
        return (float)numCleanedTiles / (m_NumRows * m_NumCols - numObstacles);
    }
}

public class GridManager : Singleton<GridManager>
{
    class PathNode
    {
        public PathNode m_PrevNode;
        public Vector2Int m_Coordinates;

        public PathNode(Vector2Int coordinates, PathNode pathNode = null)
        {
            m_PrevNode = pathNode;
            m_Coordinates = coordinates;
        }

        public Stack<Vector2Int> GetTilesToMoveTo()
        {
            PathNode curr = this;
            Stack<Vector2Int> tilesToMoveTo = new();

            while (curr != null)
            {
                tilesToMoveTo.Push(curr.m_Coordinates);
                curr = curr.m_PrevNode;
            }

            return tilesToMoveTo;
        }
    };

    [Header("Dimensions")]
    [SerializeField] private int m_NumRows = 10;
    [SerializeField] private int m_NumCols = 10;
    [Tooltip("Scale will be set to this")]
    [SerializeField] private float m_TileLength = 1;

    [Header("Tile Visuals")]
    [SerializeField] private Grid m_FloorGrid = null;
    [SerializeField] private Tilemap m_FloorTilemap = null;
    [SerializeField] private TileBase m_PaintedTile = null;
    [SerializeField] private TileBase m_DirtyTile = null;
    [SerializeField] private TileBase m_NeutralTile = null;

    [Header("Obstacles")]
    [SerializeField] private Grid m_ObstacleGrid = null;
    [SerializeField] private Tilemap m_ObstacleTilemap = null;

    [Header("Outer walls")]
    [SerializeField] private Grid m_WallGrid = null;
    [SerializeField] private Tilemap m_WallTilemap = null;
    [SerializeField] private TileBase m_WallTile = null;

    // tiles
    private TileState m_TileState;

    // grid calculations
    private float m_LeftXPos;
    private float m_BottomYPos;
    private float m_LeftSquareCenter;
    private float m_BottomSquareCenter;

    // obstacles
    private int m_NumObstacles = 0;

    protected override void HandleAwake()
    {
        Vector2 centerPos = transform.position;
        m_LeftXPos = centerPos.x - ((float)(m_NumCols) / 2) * m_TileLength;
        m_BottomYPos = centerPos.y - ((float) (m_NumRows) / 2) * m_TileLength;
        m_LeftSquareCenter = centerPos.x - ((float)(m_NumCols - 1) / 2) * m_TileLength;
        m_BottomSquareCenter = centerPos.y - ((float)(m_NumRows - 1) / 2) * m_TileLength;

        m_TileState = new TileState(m_NumRows, m_NumCols);

        for (int r = 0; r < m_NumRows; ++r)
        {
            for (int c = 0; c < m_NumCols; ++c)
            {
                Vector2Int coordinates = new Vector2Int(c, r);
                SetTileVisuals(coordinates, TileType.NEUTRAL);

                if (HasObstacleAtTile(coordinates))
                {
                    ++m_NumObstacles;
                }
            }
        }
    }

    #region Tile Status
    public TileType GetTileTypeAtWorldCoordinates(Vector2 worldCoordinates)
    {
        Vector2Int tileCoordinates = CalculateTileCoordinates(worldCoordinates);
        return m_TileState.GetTileTypeAtTile(tileCoordinates);
    }

    public void SetTileStatus(Vector2 worldCoordinates, TileType tileType)
    {
        Vector2Int tileCoordinates = CalculateTileCoordinates(worldCoordinates);

        if (m_TileState.GetTileTypeAtTile(tileCoordinates) == tileType)
            return;
        
        // update state
        m_TileState.SetTileTypeAtTile(tileCoordinates, tileType);

        // update visuals
        SetTileVisuals(tileCoordinates, tileType);
    }
    #endregion

    #region Coordinate Helpers
    private Vector2Int CalculateTileCoordinates(Vector2 worldCoordinates)
    {
        int rowNum = Mathf.FloorToInt((worldCoordinates.y - m_BottomYPos) / m_TileLength);
        int colNum = Mathf.FloorToInt((worldCoordinates.x - m_LeftXPos) / m_TileLength);
        return new Vector2Int(colNum, rowNum);
    }

    private Vector3Int ConvertToTilemapCoordinates(Vector2Int tileCoordinates)
    {
        return new Vector3Int(tileCoordinates.x - Mathf.FloorToInt(m_NumCols / 2), tileCoordinates.y - Mathf.FloorToInt(m_NumRows / 2), 0);
    }
    #endregion

    #region Visual Helpers
    private TileBase GetTile(TileType tileType)
    {
        return tileType switch
        {
            TileType.NEUTRAL => m_NeutralTile,
            TileType.DIRTY => m_DirtyTile,
            TileType.CLEAN => m_PaintedTile,
            _ => null
        };
    }
    private void SetTileVisuals(Vector2Int tileCoordinates, TileType tileType)
    {
        // update visuals
        m_FloorTilemap.SetTile(ConvertToTilemapCoordinates(tileCoordinates), GetTile(tileType));
    }
    #endregion

    #region Position Helpers
    public Vector2 GetWorldPositionOfTile(int rowNum, int colNum)
    {
        return new Vector2(m_LeftSquareCenter + colNum * m_TileLength, m_BottomSquareCenter + rowNum * m_TileLength);
    }

    private Vector2 GetWorldPositionOfTile(Vector2Int tileCoordinates)
    {
        return GetWorldPositionOfTile(tileCoordinates.y, tileCoordinates.x);
    }
    #endregion

    #region Obstacle
    private bool HasObstacleAtTile(Vector2Int tileCoordinates)
    {
        return m_ObstacleTilemap.GetTile(ConvertToTilemapCoordinates(tileCoordinates)) != null;
    }
    #endregion

    #region Quota
    public float GetCleanedPercentage => m_TileState.GetCleanedTiles(m_NumObstacles);
    #endregion

#if UNITY_EDITOR
    #region Setup
    public void SetupGrids()
    {
        m_FloorGrid.cellSize = new Vector3(m_TileLength, m_TileLength, 0);
        m_ObstacleGrid.cellSize = new Vector3(m_TileLength, m_TileLength, 0);
        m_WallGrid.cellSize = new Vector3(m_TileLength, m_TileLength, 0);

        SetupWalls();
    }

    private void SetupWalls()
    {
        m_WallTilemap.ClearAllTiles();
        for (int r = 0; r < m_NumRows; ++r)
        {
            m_WallTilemap.SetTile(ConvertToTilemapCoordinates(new Vector2Int(-1, r)), m_WallTile);
            m_WallTilemap.SetTile(ConvertToTilemapCoordinates(new Vector2Int(m_NumCols, r)), m_WallTile);
        }
        for (int c = 0; c < m_NumCols; ++c)
        {
            m_WallTilemap.SetTile(ConvertToTilemapCoordinates(new Vector2Int(c, -1)), m_WallTile);
            m_WallTilemap.SetTile(ConvertToTilemapCoordinates(new Vector2Int(c, m_NumRows)), m_WallTile);
        }
        m_WallTilemap.SetTile(ConvertToTilemapCoordinates(new Vector2Int(-1, m_NumRows)), m_WallTile);
        m_WallTilemap.SetTile(ConvertToTilemapCoordinates(new Vector2Int(-1, -1)), m_WallTile);
        m_WallTilemap.SetTile(ConvertToTilemapCoordinates(new Vector2Int(m_NumCols, m_NumRows)), m_WallTile);
        m_WallTilemap.SetTile(ConvertToTilemapCoordinates(new Vector2Int(m_NumCols, -1)), m_WallTile);
    }

    public void ClearObstacles()
    {
        m_ObstacleTilemap.ClearAllTiles();
    }
    #endregion
#endif

    #region Pathfinding
    /*
    public bool IsPositionValid(Vector3 worldPosition)
    {
        Vector2Int tileCoordinates = CalculateTileCoordinates(worldPosition);
        return IsCoordinatesValid(tileCoordinates) && !HasObstacleAtTile(tileCoordinates); 
    }
    */

    public List<Vector2> GetRandomTileLocations(Vector2 currWorldPosition)
    {
        Vector2Int currPosition = CalculateTileCoordinates(currWorldPosition);
        int randomCol = Random.Range(0, m_NumCols - 1);
        int randomRow = Random.Range(0, m_NumRows - 1);
        Vector2Int randomCoordinates = new Vector2Int(randomCol, randomRow);
        
        int i = 0;
        while ((randomCoordinates == currPosition || HasObstacleAtTile(randomCoordinates)) && i < 50)
        {
            randomCol = Random.Range(0, m_NumCols - 1);
            randomRow = Random.Range(0, m_NumRows - 1);
            randomCoordinates = new Vector2Int(randomCol, randomRow);
            ++i;
        }

        return Pathfind(currPosition, randomCoordinates, false);
    }

    private bool IsCoordinatesValid(Vector2Int tileCoordinates)
    {
        return tileCoordinates.x >= 0 && tileCoordinates.x < m_NumCols && tileCoordinates.y >= 0 && tileCoordinates.y < m_NumRows;
    }

    public List<Vector2> Pathfind(Vector2 initialWorldPosition, Vector2 finalWorldPosition, bool removeFinalPosition = true)
    {
        Vector2Int initialTilePosition = CalculateTileCoordinates(initialWorldPosition);
        Vector2Int finalTilePosition = CalculateTileCoordinates(finalWorldPosition);
        return Pathfind(initialTilePosition, finalTilePosition, removeFinalPosition);
    }

    private List<Vector2> Pathfind(Vector2Int intiialTileCoordinates, Vector2Int finalTileCoordinates, bool removeFinalPosition = true)
    {
        Stack<Vector2Int> tilesToGoTo = BFS(intiialTileCoordinates, finalTileCoordinates);
        List<Vector2> locationsToGoTo = new();
        bool first = true;
        while (tilesToGoTo.Count > 0)
        {
            Vector2Int tileCoordinates = tilesToGoTo.Pop();
            if (first)
            {
                first = false;
            }
            else if (!removeFinalPosition || tilesToGoTo.Count > 0)
            {
                locationsToGoTo.Add(GetWorldPositionOfTile(tileCoordinates));
            }
        }
        return locationsToGoTo;
    }

    private Stack<Vector2Int> BFS(Vector2Int initialTile, Vector2Int finalTile)
    {
        PathNode currTile = new(initialTile);
        Queue<PathNode> queue = new();
        HashSet<Vector2Int> visitedLocations = new();
        queue.Enqueue(currTile);

        while (queue.Count > 0)
        {
            PathNode curr = queue.Dequeue();

            if (visitedLocations.Contains(curr.m_Coordinates))
                continue;

            visitedLocations.Add(curr.m_Coordinates);

            if (curr.m_Coordinates == finalTile)
                return curr.GetTilesToMoveTo();

            if (!IsCoordinatesValid(curr.m_Coordinates))
                continue;

            if (HasObstacleAtTile(curr.m_Coordinates))
            {
                continue;
            }

            queue.Enqueue(new PathNode(curr.m_Coordinates + Vector2Int.up, curr));
            queue.Enqueue(new PathNode(curr.m_Coordinates + Vector2Int.down, curr));
            queue.Enqueue(new PathNode(curr.m_Coordinates + Vector2Int.left, curr));
            queue.Enqueue(new PathNode(curr.m_Coordinates + Vector2Int.right, curr));
        }

        return new();
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    GridManager m_GridManager;

    private void OnEnable()
    {
        m_GridManager = target.GetComponent<GridManager>();        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10f);

        if (GUILayout.Button("Adjust grid"))
        {
            m_GridManager.SetupGrids();
        }

        if (GUILayout.Button("Clear obstacles"))
        {
            m_GridManager.ClearObstacles();
        }
    }
}
#endif