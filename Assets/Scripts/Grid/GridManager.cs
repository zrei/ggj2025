using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    PAINTED,
    NEUTRAL,
    DIRTY
}

public class GridManager : Singleton<GridManager>
{
    [Header("Dimensions")]
    [SerializeField] private int m_NumRows = 10;
    [SerializeField] private int m_NumCols = 10;
    [Tooltip("Scale will be set to this")]
    [SerializeField] private int m_TileLength = 1;

    [Header("Visuals")]
    [SerializeField] private Grid m_Grid = null;
    [SerializeField] private Tilemap m_Tilemap = null;
    [SerializeField] private TileBase m_PaintedTile = null;
    [SerializeField] private TileBase m_DirtyTile = null;
    [SerializeField] private TileBase m_NeutralTile = null;

    public int NumRows => m_NumRows;
    public int NumCols => m_NumCols;
    public int TileLength => m_TileLength;
    public Grid Grid => m_Grid;

    private TileType[,] m_TileStates;

    // grid calculations
    private float m_LeftXPos;
    private float m_BottomYPos;

    protected override void HandleAwake()
    {
        m_TileStates = new TileType[m_NumRows, m_NumCols];

        for (int r = 0; r < m_NumRows; ++r)
        {
            for (int c = 0; c < m_NumCols; ++c)
            {
                m_TileStates[r, c] = TileType.NEUTRAL;
                SetTileVisuals(new Vector2Int(r, c), TileType.NEUTRAL);
            }
        }

        Vector2 centerPos = transform.position;
        m_LeftXPos = centerPos.x - (float) (m_NumCols / 2) * m_TileLength;
        m_BottomYPos = centerPos.y - (float) (m_NumRows / 2) * m_TileLength;
    }

    #region Tile Status
    public TileType GetTileTypeAtWorldCoordinates(Vector2 worldCoordinates)
    {
        Vector2Int tileCoordinates = CalculateTileCoordinates(worldCoordinates);
        return m_TileStates[tileCoordinates.x, tileCoordinates.y];
    }

    public void SetTileStatus(Vector2 worldCoordinates, TileType tileType)
    {
        Vector2Int tileCoordinates = CalculateTileCoordinates(worldCoordinates);

        if (m_TileStates[tileCoordinates.x, tileCoordinates.y] == tileType)
            return;
        
        // update state
        m_TileStates[tileCoordinates.x, tileCoordinates.y] = tileType;

        // update visuals
        SetTileVisuals(tileCoordinates, tileType);
    }
    #endregion

    #region Coordinate Helpers
    private Vector2Int CalculateTileCoordinates(Vector2 worldCoordinates)
    {
        int rowNum = Mathf.FloorToInt((worldCoordinates.y - m_BottomYPos) / m_TileLength);
        int colNum = Mathf.FloorToInt((worldCoordinates.x - m_LeftXPos) / m_TileLength);
        return new Vector2Int(rowNum, colNum);
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
            TileType.PAINTED => m_PaintedTile,
            _ => null
        };
    }

    private void SetTileVisuals(Vector2Int tileCoordinates, TileType tileType)
    {
        // update visuals
        m_Tilemap.SetTile(ConvertToTilemapCoordinates(tileCoordinates), GetTile(tileType));
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
            m_GridManager.transform.localScale = new Vector3(m_GridManager.NumCols * m_GridManager.TileLength, m_GridManager.NumRows * m_GridManager.TileLength, 1);
            m_GridManager.Grid.cellSize = new Vector3(m_GridManager.TileLength, m_GridManager.TileLength, 0);
        }
    }
}
#endif