using UnityEngine;
using UnityEngine.EventSystems;

public class GridCell : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    [Header("Cell Settings")]
    public SpriteRenderer cellRenderer;
    public Sprite emptySprite;
    public Sprite occupiedSprite;
    public Color emptyColor = Color.gray;
    public Color occupiedColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color mergeHighlightColor = Color.green;
    
    private GameElement currentElement;
    private bool isHighlighted = false;
    
    public int gridX { get; private set; }
    public int gridY { get; private set; }
    
    public System.Action<GridCell> OnCellClicked;
    public System.Action<GameElement> OnElementDropped;
    
    public void Initialize(int x, int y)
    {
        gridX = x;
        gridY = y;
        SetEmpty();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // Handle single click
        if (eventData.clickCount == 1)
        {
            OnCellClicked?.Invoke(this);
        }
        // Handle double click for generator activation
        else if (eventData.clickCount == 2 && HasElement() && currentElement.elementData.isGenerator)
        {
            ActivateGenerator();
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        GameElement draggedElement = eventData.pointerDrag?.GetComponent<GameElement>();
        if (draggedElement != null && CanAcceptElement(draggedElement))
        {
            OnElementDropped?.Invoke(draggedElement);
        }
    }
    
    public bool IsEmpty()
    {
        return currentElement == null;
    }
    
    public bool HasElement()
    {
        return currentElement != null;
    }
    
    public GameElement GetElement()
    {
        return currentElement;
    }
    
    public bool CanAcceptElement(GameElement element)
    {
        if (IsEmpty())
        {
            return true;
        }
        
        // Check if elements can merge
        if (HasElement() && currentElement.elementData.CanMergeWith(element.elementData))
        {
            return true;
        }
        
        return false;
    }
    
    public void SetElement(GameElement element)
    {
        currentElement = element;
        
        if (element != null)
        {
            element.SetGridCell(this);
            SetOccupied();
        }
        else
        {
            SetEmpty();
        }
    }
    
    public void RemoveElement()
    {
        if (currentElement != null)
        {
            currentElement.SetGridCell(null);
            currentElement = null;
            SetEmpty();
        }
    }
    
    public void Highlight(bool isMergeable = false)
    {
        isHighlighted = true;
        cellRenderer.color = isMergeable ? mergeHighlightColor : highlightColor;
    }
    
    public void RemoveHighlight()
    {
        isHighlighted = false;
        if (HasElement())
        {
            SetOccupied();
        }
        else
        {
            SetEmpty();
        }
    }
    
    private void SetEmpty()
    {
        cellRenderer.sprite = emptySprite;
        cellRenderer.color = emptyColor;
    }
    
    private void SetOccupied()
    {
        cellRenderer.sprite = occupiedSprite;
        cellRenderer.color = occupiedColor;
    }
    
    private void ActivateGenerator()
    {
        if (currentElement != null && currentElement.elementData.isGenerator)
        {
            var generator = currentElement as GeneratorElement;
            if (generator != null)
            {
                generator.TryGenerate();
            }
        }
    }
    
    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }
    
    public Vector2Int GetGridPosition()
    {
        return new Vector2Int(gridX, gridY);
    }
}