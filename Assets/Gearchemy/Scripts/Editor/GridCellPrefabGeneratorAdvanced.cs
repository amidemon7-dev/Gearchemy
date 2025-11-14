using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class GridCellPrefabGeneratorAdvanced : EditorWindow
{
    private string prefabName = "GridCell";
    private string prefabPath = "Assets/Prefabs/";
    private bool useCustomSprites = false;
    private Sprite customEmptySprite;
    private Sprite customOccupiedSprite;
    private bool createPlaceholderSprites = true;
    private bool addParticleEffects = false;
    private bool addAudioSource = false;
    private Color emptyColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    private Color occupiedColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
    private Color highlightColor = new Color(1f, 1f, 0f, 0.7f);
    private Color mergeHighlightColor = new Color(0f, 1f, 0f, 0.7f);
    private Vector2 cellSize = Vector2.one;
    private int sortingOrder = 0;
    
    [MenuItem("Gearchemy/Advanced GridCell Generator")]
    public static void ShowWindow()
    {
        GetWindow<GridCellPrefabGeneratorAdvanced>("GridCell Generator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("GridCell Prefab Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Basic Settings
        GUILayout.Label("Basic Settings", EditorStyles.boldLabel);
        prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);
        prefabPath = EditorGUILayout.TextField("Prefab Path", prefabPath);
        
        EditorGUILayout.Space();
        
        // Visual Settings
        GUILayout.Label("Visual Settings", EditorStyles.boldLabel);
        useCustomSprites = EditorGUILayout.Toggle("Use Custom Sprites", useCustomSprites);
        
        if (useCustomSprites)
        {
            customEmptySprite = EditorGUILayout.ObjectField("Empty Sprite", customEmptySprite, typeof(Sprite), false) as Sprite;
            customOccupiedSprite = EditorGUILayout.ObjectField("Occupied Sprite", customOccupiedSprite, typeof(Sprite), false) as Sprite;
        }
        else
        {
            createPlaceholderSprites = EditorGUILayout.Toggle("Create Placeholder Sprites", createPlaceholderSprites);
        }
        
        // Color Settings
        GUILayout.Label("Color Settings", EditorStyles.boldLabel);
        emptyColor = EditorGUILayout.ColorField("Empty Color", emptyColor);
        occupiedColor = EditorGUILayout.ColorField("Occupied Color", occupiedColor);
        highlightColor = EditorGUILayout.ColorField("Highlight Color", highlightColor);
        mergeHighlightColor = EditorGUILayout.ColorField("Merge Highlight Color", mergeHighlightColor);
        
        EditorGUILayout.Space();
        
        // Component Settings
        GUILayout.Label("Component Settings", EditorStyles.boldLabel);
        cellSize = EditorGUILayout.Vector2Field("Cell Size", cellSize);
        sortingOrder = EditorGUILayout.IntField("Sorting Order", sortingOrder);
        addParticleEffects = EditorGUILayout.Toggle("Add Particle Effects", addParticleEffects);
        addAudioSource = EditorGUILayout.Toggle("Add Audio Source", addAudioSource);
        
        EditorGUILayout.Space();
        
        // Generate Button
        if (GUILayout.Button("Generate GridCell Prefab", GUILayout.Height(30)))
        {
            GenerateAdvancedGridCellPrefab();
        }
        
        EditorGUILayout.Space();
        
        // Quick Actions
        GUILayout.Label("Quick Actions", EditorStyles.boldLabel);
        if (GUILayout.Button("Create Default GridCell"))
        {
            CreateDefaultGridCell();
        }
        
        if (GUILayout.Button("Setup Selected Object as GridCell"))
        {
            SetupSelectedAsGridCell();
        }
    }
    
    private void GenerateAdvancedGridCellPrefab()
    {
        string fullPath = Path.Combine(prefabPath, prefabName + ".prefab");
        
        // Ensure directory exists
        string directory = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // Create the prefab
        GameObject prefab = CreateAdvancedGridCell();
        
        // Save prefab
        PrefabUtility.SaveAsPrefabAsset(prefab, fullPath);
        
        // Clean up
        DestroyImmediate(prefab);
        
        // Refresh asset database
        AssetDatabase.Refresh();
        
        Debug.Log($"Advanced GridCell prefab created at: {fullPath}");
        
        // Close window if generation was successful
        this.Close();
    }
    
    private GameObject CreateAdvancedGridCell()
    {
        // Create base game object
        GameObject cellObject = new GameObject(prefabName);
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = cellObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = sortingOrder;
        
        // Handle sprites
        Sprite emptySprite = null;
        Sprite occupiedSprite = null;
        
        if (useCustomSprites && customEmptySprite != null && customOccupiedSprite != null)
        {
            emptySprite = customEmptySprite;
            occupiedSprite = customOccupiedSprite;
        }
        else if (createPlaceholderSprites)
        {
            emptySprite = CreateAdvancedPlaceholderSprite(emptyColor, $"{prefabName}_Empty");
            occupiedSprite = CreateAdvancedPlaceholderSprite(occupiedColor, $"{prefabName}_Occupied");
        }
        else
        {
            // Try to load default sprites
            emptySprite = LoadSprite("Assets/Sprites/CellEmpty.png");
            occupiedSprite = LoadSprite("Assets/Sprites/CellOccupied.png");
        }
        
        // Configure SpriteRenderer
        spriteRenderer.sprite = emptySprite;
        spriteRenderer.color = emptyColor;
        
        // Add BoxCollider2D
        BoxCollider2D collider = cellObject.AddComponent<BoxCollider2D>();
        collider.size = cellSize;
        
        // Add GridCell component
        GridCell gridCell = cellObject.AddComponent<GridCell>();
        gridCell.cellRenderer = spriteRenderer;
        gridCell.emptySprite = emptySprite;
        gridCell.occupiedSprite = occupiedSprite;
        gridCell.emptyColor = emptyColor;
        gridCell.occupiedColor = occupiedColor;
        gridCell.highlightColor = highlightColor;
        gridCell.mergeHighlightColor = mergeHighlightColor;
        
        // Add EventTrigger
        UnityEngine.EventSystems.EventTrigger eventTrigger = cellObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        SetupEventTrigger(eventTrigger);
        
        // Add additional components if requested
        if (addParticleEffects)
        {
            AddParticleSystem(cellObject);
        }
        
        if (addAudioSource)
        {
            AddAudioSource(cellObject);
        }
        
        // Set up layer
        cellObject.layer = LayerMask.NameToLayer("UI");
        if (cellObject.layer == -1)
        {
            cellObject.layer = 0; // Default layer
        }
        
        // Add tag
        cellObject.tag = "GridCell";
        
        return cellObject;
    }
    
    private void SetupEventTrigger(UnityEngine.EventSystems.EventTrigger eventTrigger)
    {
        // Clear existing entries
        eventTrigger.triggers.Clear();
        
        // Add pointer click entry
        UnityEngine.EventSystems.EventTrigger.Entry clickEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
        clickEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick;
        eventTrigger.triggers.Add(clickEntry);
        
        // Add drop entry
        UnityEngine.EventSystems.EventTrigger.Entry dropEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
        dropEntry.eventID = UnityEngine.EventSystems.EventTriggerType.Drop;
        eventTrigger.triggers.Add(dropEntry);
        
        // Add pointer enter entry
        UnityEngine.EventSystems.EventTrigger.Entry enterEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
        enterEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        eventTrigger.triggers.Add(enterEntry);
        
        // Add pointer exit entry
        UnityEngine.EventSystems.EventTrigger.Entry exitEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
        exitEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        eventTrigger.triggers.Add(exitEntry);
    }
    
    private void AddParticleSystem(GameObject parent)
    {
        GameObject particleObject = new GameObject("ParticleEffects");
        particleObject.transform.SetParent(parent.transform);
        particleObject.transform.localPosition = Vector3.zero;
        
        ParticleSystem particleSystem = particleObject.AddComponent<ParticleSystem>();
        
        // Configure basic particle settings
        var main = particleSystem.main;
        main.startLifetime = 1f;
        main.startSpeed = 2f;
        main.startSize = 0.1f;
        main.startColor = highlightColor;
        main.maxParticles = 10;
        
        var emission = particleSystem.emission;
        emission.rateOverTime = 5;
        
        var shape = particleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.5f;
    }
    
    private void AddAudioSource(GameObject parent)
    {
        AudioSource audioSource = parent.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;
        audioSource.pitch = 1f;
    }
    
    private Sprite CreateAdvancedPlaceholderSprite(Color color, string name)
    {
        // Create a high-quality 128x128 texture
        Texture2D texture = new Texture2D(128, 128);
        
        // Create a gradient effect
        for (int x = 0; x < 128; x++)
        {
            for (int y = 0; y < 128; y++)
            {
                // Create a circular gradient
                float centerX = 64;
                float centerY = 64;
                float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                float maxDistance = 64;
                float alpha = 1f - (distance / maxDistance) * 0.3f;
                
                Color pixelColor = color;
                pixelColor.a *= alpha;
                texture.SetPixel(x, y, pixelColor);
            }
        }
        
        texture.Apply();
        
        // Create sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f), 64);
        
        // Save as asset
        string spritePath = $"Assets/Sprites/Generated/{name}.png";
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
        
        return AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
    }
    
    private void CreateDefaultGridCell()
    {
        prefabName = "GridCell";
        prefabPath = "Assets/Prefabs/";
        useCustomSprites = false;
        createPlaceholderSprites = true;
        addParticleEffects = false;
        addAudioSource = false;
        
        GenerateAdvancedGridCellPrefab();
    }
    
    private void SetupSelectedAsGridCell()
    {
        GameObject selectedObject = Selection.activeGameObject;
        
        if (selectedObject == null)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select a GameObject to set up as GridCell.", "OK");
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
            UnityEngine.EventSystems.EventTrigger trigger = selectedObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            SetupEventTrigger(trigger);
        }
        
        // Configure GridCell component
        GridCell gridCell = selectedObject.GetComponent<GridCell>();
        SpriteRenderer spriteRenderer = selectedObject.GetComponent<SpriteRenderer>();
        
        if (gridCell.cellRenderer == null)
        {
            gridCell.cellRenderer = spriteRenderer;
        }
        
        // Set default colors
        gridCell.emptyColor = emptyColor;
        gridCell.occupiedColor = occupiedColor;
        gridCell.highlightColor = highlightColor;
        gridCell.mergeHighlightColor = mergeHighlightColor;
        
        // Try to load sprites
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
        
        // Set tag
        selectedObject.tag = "GridCell";
        
        EditorUtility.DisplayDialog("Success", $"GridCell components set up for {selectedObject.name}", "OK");
    }
    
    private Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}