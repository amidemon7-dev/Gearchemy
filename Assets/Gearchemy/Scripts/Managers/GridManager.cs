using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject cellPrefab;
    public Transform gridContainer;
    public float cellSize = 1f;
    public Vector2 gridOffset = Vector2.zero;
    
    private GridCell[,] gridCells;
    private int gridWidth;
    private int gridHeight;
    
    public System.Action<GridCell> OnCellClicked;
    public System.Action<GridCell, GridCell> OnElementsMerged;

    public int GridWidth { get => gridWidth; }
    public int GridHeight { get => gridHeight; }
    public void InitializeGrid(int width, int height)
    {
        gridWidth = width;
        gridHeight = height;
        gridCells = new GridCell[width, height];
        
        CreateGrid();
    }
    
    private void CreateGrid()
    {
        // Clear existing cells
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create new grid
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject cellObject = Instantiate(cellPrefab, gridContainer);
                cellObject.name = $"Cell_{x}_{y}";
                
                Vector3 position = new Vector3(
                    (x - GridWidth / 2f + 0.5f) * cellSize + gridOffset.x,
                    (y - gridHeight / 2f + 0.5f) * cellSize + gridOffset.y,
                    0
                );
                
                cellObject.transform.localPosition = position;
                
                GridCell cell = cellObject.GetComponent<GridCell>();
                cell.Initialize(x, y);
                cell.OnCellClicked += HandleCellClick;
                
                gridCells[x, y] = cell;
            }
        }
    }
    
    private void HandleCellClick(GridCell cell)
    {
        OnCellClicked?.Invoke(cell);
    }
    
    public GridCell GetCellAt(int x, int y)
    {
        if (x >= 0 && x < GridWidth && y >= 0 && y < gridHeight)
        {
            return gridCells[x, y];
        }
        return null;
    }
    
    public GridCell GetCellAtWorldPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = gridContainer.InverseTransformPoint(worldPosition);
        
        int x = Mathf.RoundToInt((localPosition.x - gridOffset.x) / cellSize + GridWidth / 2f - 0.5f);
        int y = Mathf.RoundToInt((localPosition.y - gridOffset.y) / cellSize + gridHeight / 2f - 0.5f);
        
        return GetCellAt(x, y);
    }
    
    public List<GridCell> GetEmptyCells()
    {
        List<GridCell> emptyCells = new List<GridCell>();
        
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridCells[x, y].IsEmpty())
                {
                    emptyCells.Add(gridCells[x, y]);
                }
            }
        }
        
        return emptyCells;
    }
    
    public GridCell GetRandomEmptyCell()
    {
        List<GridCell> emptyCells = GetEmptyCells();
        if (emptyCells.Count > 0)
        {
            return emptyCells[UnityEngine.Random.Range(0, emptyCells.Count)];
        }
        return null;
    }
    
    public List<GridCell> GetAdjacentCells(GridCell cell)
    {
        List<GridCell> adjacentCells = new List<GridCell>();
        int x = cell.gridX;
        int y = cell.gridY;
        
        // Check all 4 directions
        GridCell[] directions = {
            GetCellAt(x - 1, y),   // Left
            GetCellAt(x + 1, y),   // Right
            GetCellAt(x, y - 1),   // Down
            GetCellAt(x, y + 1)    // Up
        };
        
        foreach (GridCell adjacent in directions)
        {
            if (adjacent != null)
            {
                adjacentCells.Add(adjacent);
            }
        }
        
        return adjacentCells;
    }
    
    public List<GridCell> FindMergeablePairs(ElementData elementType)
    {
        List<GridCell> mergeablePairs = new List<GridCell>();
        
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GridCell cell = gridCells[x, y];
                if (cell.HasElement() && cell.GetElement().elementData.elementType == elementType.elementType)
                {
                    // Check adjacent cells for same element
                    List<GridCell> adjacent = GetAdjacentCells(cell);
                    foreach (GridCell adjacentCell in adjacent)
                    {
                        if (adjacentCell.HasElement() && 
                            adjacentCell.GetElement().elementData.CanMergeWith(cell.GetElement().elementData))
                        {
                            mergeablePairs.Add(cell);
                            mergeablePairs.Add(adjacentCell);
                        }
                    }
                }
            }
        }
        
        return mergeablePairs;
    }
    
    public bool IsGridFull()
    {
        return GetEmptyCells().Count == 0;
    }
    
    public int GetElementCount(ElementData elementType)
    {
        int count = 0;
        
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GridCell cell = gridCells[x, y];
                if (cell.HasElement() && cell.GetElement().elementData == elementType)
                {
                    count++;
                }
            }
        }
        
        return count;
    }
}