using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SceneSetup : MonoBehaviour
{
    [MenuItem("Gearchemy/Setup Scene")]
    public static void SetupScene()
    {
        // Create main game object structure
        CreateGameManagers();
        CreateGridSystem();
        CreateUI();
        CreateCamera();
        
        Debug.Log("Gearchemy scene setup complete!");
    }
    
    [MenuItem("Gearchemy/Create Sample Elements")]
    public static void CreateSampleElements()
    {
        CreateElementDatabase();
        CreateGeneratorDatabase();
        CreateRecipeDatabase();
        
        Debug.Log("Sample elements created!");
    }
    
    private static void CreateGameManagers()
    {
        // Create main GameManager
        GameObject gameManagerObj = new GameObject("GameManager");
        gameManagerObj.AddComponent<GameManager>();
        
        // Add all manager components
        gameManagerObj.AddComponent<GridManager>();
        gameManagerObj.AddComponent<ElementSystem>();
        gameManagerObj.AddComponent<EconomyManager>();
        gameManagerObj.AddComponent<ProgressionManager>();
        gameManagerObj.AddComponent<NarrativeManager>();
        gameManagerObj.AddComponent<CraftingManager>();
        gameManagerObj.AddComponent<SaveSystem>();
        
        // Set up references
        var gameManager = gameManagerObj.GetComponent<GameManager>();
        gameManager.gridWidth = 6;
        gameManager.gridHeight = 6;
    }
    
    private static void CreateGridSystem()
    {
        // Create grid container
        GameObject gridContainer = new GameObject("GridContainer");
        gridContainer.transform.position = Vector3.zero;
        
        // Set up grid manager reference
        var gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            gridManager.gridContainer = gridContainer.transform;
            
            // Create cell prefab if it doesn't exist
            string cellPrefabPath = "Assets/Prefabs/GridCell.prefab";
            GameObject cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(cellPrefabPath);
            
            if (cellPrefab == null)
            {
                cellPrefab = CreateCellPrefab();
                AssetDatabase.CreateAsset(cellPrefab, cellPrefabPath);
            }
            
            gridManager.cellPrefab = cellPrefab;
        }
    }
    
    private static GameObject CreateCellPrefab()
    {
        GameObject cellObj = new GameObject("GridCell");
        
        // Add SpriteRenderer
        SpriteRenderer renderer = cellObj.AddComponent<SpriteRenderer>();
        renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/CellSprite.png");
        renderer.color = Color.gray;
        
        // Add GridCell component
        GridCell gridCell = cellObj.AddComponent<GridCell>();
        gridCell.cellRenderer = renderer;
        
        // Add collider for interaction
        cellObj.AddComponent<BoxCollider2D>();
        
        return cellObj;
    }
    
    private static void CreateUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("MainCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Add CanvasScaler for responsive design
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Create UI Manager
        GameObject uiManagerObj = new GameObject("UIManager");
        UIManager uiManager = uiManagerObj.AddComponent<UIManager>();
        
        // Create UI panels
        CreateMainGameUI(canvasObj.transform);
        CreateResourceDisplay(canvasObj.transform);
        CreateBottomNavigation(canvasObj.transform);
    }
    
    private static void CreateMainGameUI(Transform parent)
    {
        // Main game panel
        GameObject mainPanel = new GameObject("MainGamePanel");
        mainPanel.transform.SetParent(parent);
        
        RectTransform rectTransform = mainPanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        
        Image background = mainPanel.AddComponent<Image>();
        background.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    }
    
    private static void CreateResourceDisplay(Transform parent)
    {
        // Resource panel
        GameObject resourcePanel = new GameObject("ResourcePanel");
        resourcePanel.transform.SetParent(parent);
        
        RectTransform rectTransform = resourcePanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = new Vector2(0, 100);
        
        // Coins display
        CreateResourceItem(resourcePanel.transform, "Coins", "🪙", new Vector2(0, 0));
        CreateResourceItem(resourcePanel.transform, "EtherGems", "💎", new Vector2(200, 0));
        CreateResourceItem(resourcePanel.transform, "Essence", "✨", new Vector2(400, 0));
        CreateResourceItem(resourcePanel.transform, "Energy", "⚡", new Vector2(600, 0));
    }
    
    private static void CreateResourceItem(Transform parent, string resourceName, string icon, Vector2 position)
    {
        GameObject resourceItem = new GameObject(resourceName + "Display");
        resourceItem.transform.SetParent(parent);
        
        RectTransform rectTransform = resourceItem.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(180, 50);
        
        // Icon
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(resourceItem.transform);
        TextMeshProUGUI iconText = iconObj.AddComponent<TextMeshProUGUI>();
        iconText.text = icon;
        iconText.fontSize = 24;
        
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = new Vector2(0.3f, 1);
        iconRect.sizeDelta = Vector2.zero;
        
        // Value
        GameObject valueObj = new GameObject("Value");
        valueObj.transform.SetParent(resourceItem.transform);
        TextMeshProUGUI valueText = valueObj.AddComponent<TextMeshProUGUI>();
        valueText.text = "0";
        valueText.fontSize = 20;
        valueText.alignment = TextAlignmentOptions.Right;
        
        RectTransform valueRect = valueObj.GetComponent<RectTransform>();
        valueRect.anchorMin = new Vector2(0.3f, 0);
        valueRect.anchorMax = Vector2.one;
        valueRect.sizeDelta = Vector2.zero;
    }
    
    private static void CreateBottomNavigation(Transform parent)
    {
        // Navigation panel
        GameObject navPanel = new GameObject("NavigationPanel");
        navPanel.transform.SetParent(parent);
        
        RectTransform rectTransform = navPanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = new Vector2(1, 0.15f);
        rectTransform.sizeDelta = Vector2.zero;
        
        // Create navigation buttons
        string[] buttonNames = { "Inventory", "Quests", "Shop", "Achievements", "Settings" };
        string[] buttonIcons = { "📦", "📋", "🏪", "🏆", "⚙️" };
        
        for (int i = 0; i < buttonNames.Length; i++)
        {
            CreateNavButton(navPanel.transform, buttonNames[i], buttonIcons[i], i, buttonNames.Length);
        }
    }
    
    private static void CreateNavButton(Transform parent, string buttonName, string icon, int index, int total)
    {
        GameObject buttonObj = new GameObject(buttonName + "Button");
        buttonObj.transform.SetParent(parent);
        
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        float width = 1f / total;
        rectTransform.anchorMin = new Vector2(width * index, 0);
        rectTransform.anchorMax = new Vector2(width * (index + 1), 1);
        rectTransform.sizeDelta = Vector2.zero;
        
        Button button = buttonObj.AddComponent<Button>();
        
        // Icon
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(buttonObj.transform);
        TextMeshProUGUI iconText = iconObj.AddComponent<TextMeshProUGUI>();
        iconText.text = icon;
        iconText.fontSize = 32;
        iconText.alignment = TextAlignmentOptions.Center;
        
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.3f);
        iconRect.anchorMax = Vector2.one;
        iconRect.sizeDelta = Vector2.zero;
        
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(buttonObj.transform);
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = buttonName;
        labelText.fontSize = 14;
        labelText.alignment = TextAlignmentOptions.Center;
        
        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = new Vector2(1, 0.3f);
        labelRect.sizeDelta = Vector2.zero;
    }
    
    private static void CreateCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
        }
        
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 5;
        mainCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.backgroundColor = Color.black;
    }
    
    private static void CreateElementDatabase()
    {
        // Create sample elements
        CreateBerryElements();
        CreateMushroomElements();
        CreateCrystalElements();
        CreateSteamElements();
    }
    
    private static void CreateBerryElements()
    {
        ElementData berry1 = CreateElementData("Red Berry", ElementData.ElementType.Berry, 1, "🍓", Color.red);
        ElementData berry2 = CreateElementData("Blue Berry", ElementData.ElementType.Berry, 2, "🔵", Color.blue);
        ElementData berry3 = CreateElementData("Golden Berry", ElementData.ElementType.Berry, 3, "✨", Color.yellow);
        ElementData berry4 = CreateElementData("Crystal Berry", ElementData.ElementType.Berry, 4, "💎", Color.cyan);
        ElementData berry5 = CreateElementData("Alchemical Berry", ElementData.ElementType.Berry, 5, "⚗️", Color.magenta);
        
        // Set up merge chain
        berry1.nextLevelElement = berry2;
        berry2.nextLevelElement = berry3;
        berry3.nextLevelElement = berry4;
        berry4.nextLevelElement = berry5;
    }
    
    private static void CreateMushroomElements()
    {
        ElementData mushroom1 = CreateElementData("Common Mushroom", ElementData.ElementType.Mushroom, 1, "🍄", Color.gray);
        ElementData mushroom2 = CreateElementData("Glowing Mushroom", ElementData.ElementType.Mushroom, 2, "🟡", Color.yellow);
        ElementData mushroom3 = CreateElementData("Magic Mushroom", ElementData.ElementType.Mushroom, 3, "🔮", Color.magenta);
        ElementData mushroom4 = CreateElementData("Alchemical Mushroom", ElementData.ElementType.Mushroom, 4, "⚗️", Color.cyan);
        ElementData mushroom5 = CreateElementData("Crystal Mushroom", ElementData.ElementType.Mushroom, 5, "💎", Color.blue);
        
        // Set up merge chain
        mushroom1.nextLevelElement = mushroom2;
        mushroom2.nextLevelElement = mushroom3;
        mushroom3.nextLevelElement = mushroom4;
        mushroom4.nextLevelElement = mushroom5;
    }
    
    private static void CreateCrystalElements()
    {
        ElementData crystal1 = CreateElementData("Quartz Crystal", ElementData.ElementType.Crystal, 1, "🔶", Color.yellow);
        ElementData crystal2 = CreateElementData("Sapphire", ElementData.ElementType.Crystal, 2, "🔷", Color.blue);
        ElementData crystal3 = CreateElementData("Emerald", ElementData.ElementType.Crystal, 3, "💚", Color.green);
        ElementData crystal4 = CreateElementData("Ruby", ElementData.ElementType.Crystal, 4, "❤️", Color.red);
        ElementData crystal5 = CreateElementData("Almandine", ElementData.ElementType.Crystal, 5, "💎", Color.magenta);
        
        // Set up merge chain
        crystal1.nextLevelElement = crystal2;
        crystal2.nextLevelElement = crystal3;
        crystal3.nextLevelElement = crystal4;
        crystal4.nextLevelElement = crystal5;
    }
    
    private static void CreateSteamElements()
    {
        ElementData steam1 = CreateElementData("Steam", ElementData.ElementType.Steam, 1, "💨", Color.white);
        ElementData steam2 = CreateElementData("Steam Sphere", ElementData.ElementType.Steam, 2, "🔵", Color.blue);
        ElementData steam3 = CreateElementData("Steam Crystal", ElementData.ElementType.Steam, 3, "⚪", Color.cyan);
        ElementData steam4 = CreateElementData("Ethereal Steam", ElementData.ElementType.Steam, 4, "💎", Color.white);
        ElementData steam5 = CreateElementData("Aether Element", ElementData.ElementType.Steam, 5, "✨", Color.yellow);
        
        // Set up merge chain
        steam1.nextLevelElement = steam2;
        steam2.nextLevelElement = steam3;
        steam3.nextLevelElement = steam4;
        steam4.nextLevelElement = steam5;
    }
    
    private static ElementData CreateElementData(string name, ElementData.ElementType type, int level, string emoji, Color color)
    {
        ElementData element = ScriptableObject.CreateInstance<ElementData>();
        element.elementName = name;
        element.elementType = type;
        element.level = level;
        element.baseValue = level * 10;
        element.primaryColor = color;
        element.canMerge = true;
        
        // Create simple prefab
        GameObject prefab = new GameObject(name);
        prefab.AddComponent<SpriteRenderer>();
        prefab.AddComponent<GameElement>();
        prefab.AddComponent<BoxCollider2D>();
        
        element.prefab = prefab;
        
        // Save asset
        string path = $"Assets/ScriptableObjects/Elements/{name}.asset";
        AssetDatabase.CreateAsset(element, path);
        
        return element;
    }
    
    private static void CreateGeneratorDatabase()
    {
        /*
        // Create sample generators
        GeneratorData berryBush = CreateGeneratorData("Berry Bush", "🌳", berry1, 30, 5);
        GeneratorData mushroomStump = CreateGeneratorData("Mushroom Stump", "🪵", mushroom1, 35, 4);
        GeneratorData crystalVein = CreateGeneratorData("Crystal Vein", "⛰️", crystal1, 40, 6);
        GeneratorData steamBoiler = CreateGeneratorData("Steam Boiler", "⚗️", steam1, 25, 3);
        */
    }
    
    private static GeneratorData CreateGeneratorData(string name, string emoji, ElementData generatedElement, float genTime, int energyCost)
    {
        GeneratorData generator = ScriptableObject.CreateInstance<GeneratorData>();
        generator.generatorName = name;
        generator.generatedElement = generatedElement;
        generator.baseGenerationTime = genTime;
        generator.baseEnergyCost = energyCost;
        generator.maxLevel = 5;
        
        // Create simple prefab
        GameObject prefab = new GameObject(name);
        prefab.AddComponent<SpriteRenderer>();
        GeneratorElement generatorElement = prefab.AddComponent<GeneratorElement>();
        prefab.AddComponent<BoxCollider2D>();
        
        generator.prefab = prefab;
        
        // Save asset
        string path = $"Assets/ScriptableObjects/Generators/{name}.asset";
        AssetDatabase.CreateAsset(generator, path);
        
        return generator;
    }
    
    private static void CreateRecipeDatabase()
    {
        // Create sample recipes
        // This would be expanded with actual crafting recipes
    }
}