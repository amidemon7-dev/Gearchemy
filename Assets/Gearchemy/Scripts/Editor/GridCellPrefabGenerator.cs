using UnityEngine;
using UnityEditor;
using System.IO;

public class GridCellPrefabGenerator : MonoBehaviour
{
    [MenuItem("Gearchemy/Generate GridCell Prefab")]
    public static void GenerateGridCellPrefab()
    {
        string prefabPath = "Assets/Prefabs/GridCell.prefab";
        
        // Create or load existing prefab
        GameObject prefab = CreateGridCellPrefab();
        
        // Ensure directory exists
        string directory = Path.GetDirectoryName(prefabPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // Check if prefab already exists
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existingPrefab != null)
        {
            // Update existing prefab
            PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            Debug.Log("GridCell prefab updated at: " + prefabPath);
        }
        else
        {
            // Create new prefab
            PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            Debug.Log("GridCell prefab created at: " + prefabPath);
        }
        
        // Clean up temporary game object
        DestroyImmediate(prefab);
        
        // Refresh asset database
        AssetDatabase.Refresh();
    }
    
    private static GameObject CreateGridCellPrefab()
    {
        // Create base game object
        GameObject cellObject = new GameObject("GridCell");
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = cellObject.AddComponent<SpriteRenderer>();
        
        // Try to load default sprites
        Sprite emptySprite = LoadSprite("Assets/Sprites/CellEmpty.png");
        Sprite occupiedSprite = LoadSprite("Assets/Sprites/CellOccupied.png");
        
        // If sprites don't exist, create placeholder sprites
        if (emptySprite == null)
        {
            emptySprite = CreatePlaceholderSprite(Color.gray, "Empty Cell");
        }
        if (occupiedSprite == null)
        {
            occupiedSprite = CreatePlaceholderSprite(Color.white, "Occupied Cell");
        }
        
        // Configure SpriteRenderer
        spriteRenderer.sprite = emptySprite;
        spriteRenderer.color = Color.gray;
        spriteRenderer.sortingOrder = 0;
        
        // Add BoxCollider2D for interaction
        BoxCollider2D collider = cellObject.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        
        // Add GridCell component
        GridCell gridCell = cellObject.AddComponent<GridCell>();
        gridCell.cellRenderer = spriteRenderer;
        gridCell.emptySprite = emptySprite;
        gridCell.occupiedSprite = occupiedSprite;
        gridCell.emptyColor = new Color(0.3f, 0.3f, 0.3f, 0.5f); // Semi-transparent gray
        gridCell.occupiedColor = new Color(0.8f, 0.8f, 0.8f, 0.8f); // Light gray
        gridCell.highlightColor = new Color(1f, 1f, 0f, 0.7f); // Yellow highlight
        gridCell.mergeHighlightColor = new Color(0f, 1f, 0f, 0.7f); // Green merge highlight
        
        // Add EventTrigger for better UI interaction
        UnityEngine.EventSystems.EventTrigger eventTrigger = cellObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        
        // Add entry for pointer click
        UnityEngine.EventSystems.EventTrigger.Entry clickEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
        clickEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick;
        eventTrigger.triggers.Add(clickEntry);
        
        // Add entry for drop
        UnityEngine.EventSystems.EventTrigger.Entry dropEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
        dropEntry.eventID = UnityEngine.EventSystems.EventTriggerType.Drop;
        eventTrigger.triggers.Add(dropEntry);
        
        // Set up layer for UI interaction
        cellObject.layer = LayerMask.NameToLayer("UI");
        if (cellObject.layer == -1)
        {
            // If UI layer doesn't exist, use Default layer
            cellObject.layer = 0;
        }
        
        return cellObject;
    }
    
    private static Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
    
    private static Sprite CreatePlaceholderSprite(Color color, string name)
    {
        // Create a simple 64x64 texture
        Texture2D texture = new Texture2D(64, 64);
        
        // Fill with color
        Color[] pixels = new Color[64 * 64];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Create sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        
        // Save as asset
        string spritePath = $"Assets/Sprites/Placeholder/{name.Replace(" ", "")}.png";
        string directory = Path.GetDirectoryName(spritePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // Save texture
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(spritePath, bytes);
        
        // Import asset
        AssetDatabase.ImportAsset(spritePath);
        
        // Load and return the sprite
        return AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
    }
    
    [MenuItem("Gearchemy/Generate GridCell Prefab", true)]
    private static bool ValidateGenerateGridCellPrefab()
    {
        return !Application.isPlaying;
    }
    
    [MenuItem("Gearchemy/Setup GridCell Components")]
    public static void SetupGridCellComponents()
    {
        GameObject selectedObject = Selection.activeGameObject;
        
        if (selectedObject == null)
        {
            Debug.LogWarning("Please select a GameObject to set up as GridCell");
            return;
        }
        
        // Add missing components
        if (selectedObject.GetComponent<SpriteRenderer>() == null)
        {
            selectedObject.AddComponent<SpriteRenderer>();
        }
        
        if (selectedObject.GetComponent<BoxCollider2D>() == null)
        {
            selectedObject.AddComponent<BoxCollider2D>();
        }
        
        if (selectedObject.GetComponent<GridCell>() == null)
        {
            selectedObject.AddComponent<GridCell>();
        }
        
        if (selectedObject.GetComponent<UnityEngine.EventSystems.EventTrigger>() == null)
        {
            selectedObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }
        
        // Configure components
        GridCell gridCell = selectedObject.GetComponent<GridCell>();
        SpriteRenderer spriteRenderer = selectedObject.GetComponent<SpriteRenderer>();
        
        if (gridCell.cellRenderer == null)
        {
            gridCell.cellRenderer = spriteRenderer;
        }
        
        // Try to load default sprites
        Sprite emptySprite = LoadSprite("Assets/Sprites/CellEmpty.png");
        Sprite occupiedSprite = LoadSprite("Assets/Sprites/CellOccupied.png");
        
        if (emptySprite != null)
        {
            gridCell.emptySprite = emptySprite;
        }
        if (occupiedSprite != null)
        {
            gridCell.occupiedSprite = occupiedSprite;
        }
        
        Debug.Log($"GridCell components set up for {selectedObject.name}");
    }
}