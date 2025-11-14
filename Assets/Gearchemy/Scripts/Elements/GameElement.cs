using UnityEngine;
using UnityEngine.EventSystems;

public class GameElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Element Components")]
    public SpriteRenderer elementRenderer;
    public ParticleSystem mergeEffect;
    public AudioSource audioSource;
    
    [Header("Drag Settings")]
    public float dragSpeed = 10f;
    public float returnSpeed = 15f;
    public AnimationCurve dragScaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 1.2f);
    
    private GridCell currentCell;
    private Vector3 originalPosition;
    private Transform originalParent;
    private bool isDragging = false;
    private bool isReturning = false;
    
    public ElementData elementData { get; private set; }
    
    public System.Action<GameElement> OnElementMerged;
    public System.Action<GameElement> OnElementClicked;
    
    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    
    public void Initialize(ElementData data)
    {
        elementData = data;
        
        // Set visual representation
        if (elementRenderer != null && data.icon != null)
        {
            elementRenderer.sprite = data.icon;
            elementRenderer.color = data.primaryColor;
        }
        
        // Set up audio
        if (audioSource != null && data.mergeSound != null)
        {
            audioSource.clip = data.mergeSound;
        }
        
        originalParent = transform.parent;
    }
    
    public void SetGridCell(GridCell cell)
    {
        currentCell = cell;
        
        if (cell != null)
        {
            transform.position = cell.GetWorldPosition();
        }
    }
    
    public GridCell GetGridCell()
    {
        return currentCell;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!elementData.canMerge) return;
        
        isDragging = true;
        isReturning = false;
        originalPosition = transform.position;
        
        // Bring to front
        transform.SetParent(originalParent);
        
        // Scale up during drag
        transform.localScale = Vector3.one * dragScaleCurve.Evaluate(0.5f);
        
        // Highlight possible merge targets
        HighlightMergeTargets();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPosition.z = transform.position.z;
        
        transform.position = Vector3.Lerp(transform.position, worldPosition, dragSpeed * Time.deltaTime);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        isDragging = false;
        
        // Remove highlights
        RemoveHighlights();
        
        // Check for valid drop target
        GridManager gridManager = FindObjectOfType<GridManager>();
        GridCell targetCell = gridManager?.GetCellAtWorldPosition(transform.position);
        
        if (targetCell != null && targetCell != currentCell)
        {
            if (targetCell.CanAcceptElement(this))
            {
                if (targetCell.HasElement())
                {
                    // Merge elements
                    TryMerge(targetCell);
                }
                else
                {
                    // Move to empty cell
                    MoveToCell(targetCell);
                }
                return;
            }
        }
        
        // Return to original position if drop was invalid
        ReturnToOriginalPosition();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // Single click handling
        if (!isDragging && eventData.clickCount == 1)
        {
            OnElementClicked?.Invoke(this);
            
            // Show element info
            ShowElementInfo();
        }
    }
    
    private void TryMerge(GridCell targetCell)
    {
        GameElement targetElement = targetCell.GetElement();
        if (targetElement != null && elementData.CanMergeWith(targetElement.elementData))
        {
            // Create merged element
            ElementData mergedData = elementData.nextLevelElement;
            if (mergedData != null)
            {
                // Remove old elements
                currentCell?.RemoveElement();
                targetCell.RemoveElement();
                
                // Create new merged element
                GameObject mergedObject = Instantiate(mergedData.prefab, targetCell.transform.position, Quaternion.identity);
                GameElement mergedElement = mergedObject.GetComponent<GameElement>();
                mergedElement.Initialize(mergedData);
                
                // Add to grid
                targetCell.SetElement(mergedElement);
                
                // Trigger merge effects
                TriggerMergeEffects();
                
                // Notify systems
                OnElementMerged?.Invoke(mergedElement);
                
                // Award XP
                GameManager.Instance?.playerData.AddXP(10);
                
                // Destroy old elements
                Destroy(gameObject);
                Destroy(targetElement.gameObject);
            }
        }
    }
    
    private void MoveToCell(GridCell targetCell)
    {
        if (currentCell != null)
        {
            currentCell.RemoveElement();
        }
        
        targetCell.SetElement(this);
        transform.position = targetCell.GetWorldPosition();
        transform.localScale = Vector3.one;
    }
    
    private void ReturnToOriginalPosition()
    {
        isReturning = true;
        StartCoroutine(ReturnToPosition(originalPosition));
    }
    
    private System.Collections.IEnumerator ReturnToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, returnSpeed * Time.deltaTime);
            yield return null;
        }
        
        transform.position = targetPosition;
        transform.localScale = Vector3.one;
        isReturning = false;
    }
    
    private void HighlightMergeTargets()
    {
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            var mergeableCells = gridManager.FindMergeablePairs(elementData);
            foreach (var cell in mergeableCells)
            {
                cell.Highlight(true);
            }
        }
    }
    
    private void RemoveHighlights()
    {
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            for (int x = 0; x < gridManager.GridWidth; x++)
            {
                for (int y = 0; y < gridManager.GridHeight; y++)
                {
                    gridManager.GetCellAt(x, y)?.RemoveHighlight();
                }
            }
        }
    }
    
    private void TriggerMergeEffects()
    {
        if (mergeEffect != null)
        {
            mergeEffect.Play();
        }
        
        if (audioSource != null && elementData.mergeSound != null)
        {
            audioSource.PlayOneShot(elementData.mergeSound);
        }
    }
    
    private void ShowElementInfo()
    {
        // This would show a tooltip or info panel
        Debug.Log($"Element: {elementData.elementName}, Level: {elementData.level}, Value: {elementData.GetSellValue()}");
    }
}