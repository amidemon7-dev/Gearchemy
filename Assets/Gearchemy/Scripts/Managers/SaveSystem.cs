using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem : MonoBehaviour
{
    [Header("Save Settings")]
    public string saveFileName = "Gearchemy_save.dat";
    public bool enableAutoSave = true;
    public float autoSaveInterval = 300f; // 5 minutes
    
    private float lastAutoSaveTime;
    
    private void Start()
    {
        lastAutoSaveTime = Time.time;
    }
    
    private void Update()
    {
        if (enableAutoSave && Time.time - lastAutoSaveTime >= autoSaveInterval)
        {
            SaveGame();
            lastAutoSaveTime = Time.time;
        }
    }
    
    public void SaveGame()
    {
        if (GameManager.Instance?.playerData == null) return;
        
        try
        {
            SaveData saveData = CreateSaveData();
            string savePath = GetSavePath();
            
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(savePath, FileMode.Create);
            
            formatter.Serialize(fileStream, saveData);
            fileStream.Close();
            
            Debug.Log("Game saved successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }
    
    public void LoadGame()
    {
        string savePath = GetSavePath();
        
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file found. Starting new game.");
            return;
        }
        
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(savePath, FileMode.Open);
            
            SaveData saveData = formatter.Deserialize(fileStream) as SaveData;
            fileStream.Close();
            
            ApplySaveData(saveData);
            Debug.Log("Game loaded successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
        }
    }
    
    public void DeleteSave()
    {
        string savePath = GetSavePath();
        
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted.");
        }
    }
    
    private SaveData CreateSaveData()
    {
        SaveData saveData = new SaveData();
        
        // Player data
        saveData.playerLevel = GameManager.Instance.playerData.playerLevel;
        saveData.currentXP = GameManager.Instance.playerData.currentXP;
        saveData.coins = GameManager.Instance.playerData.coins;
        saveData.etherGems = GameManager.Instance.playerData.etherGems;
        saveData.essence = GameManager.Instance.playerData.essence;
        saveData.energy = GameManager.Instance.playerData.energy;
        saveData.maxEnergy = GameManager.Instance.playerData.maxEnergy;
        
        // Inventory
        saveData.inventory = new System.Collections.Generic.List<SaveData.InventoryItem>();
        foreach (var item in GameManager.Instance.playerData.inventory)
        {
            saveData.inventory.Add(new SaveData.InventoryItem
            {
                elementId = item.elementId,
                quantity = item.quantity,
                level = item.level
            });
        }
        
        // Achievements
        saveData.completedAchievements = new System.Collections.Generic.List<string>();
        foreach (var achievement in GameManager.Instance.playerData.completedAchievements)
        {
            saveData.completedAchievements.Add(achievement);
        }
        
        // NPC Relationships
        saveData.npcRelationships = new System.Collections.Generic.Dictionary<string, int>();
        foreach (var kvp in GameManager.Instance.playerData.npcRelationships)
        {
            saveData.npcRelationships[kvp.Key] = kvp.Value;
        }
        
        // Unlocked Recipes
        saveData.unlockedRecipes = new System.Collections.Generic.List<string>();
        foreach (var recipe in GameManager.Instance.playerData.unlockedRecipes)
        {
            saveData.unlockedRecipes.Add(recipe);
        }
        
        // Grid state
        saveData.gridElements = SaveGridState();
        
        // Game settings
        saveData.gameSettings = new SaveData.SettingsData
        {
            enableSoundEffects = GameManager.Instance.enableSoundEffects,
            enableChainReactions = GameManager.Instance.enableChainReactions
        };
        
        return saveData;
    }
    
    private System.Collections.Generic.List<SaveData.GridElementData> SaveGridState()
    {
        var gridElements = new System.Collections.Generic.List<SaveData.GridElementData>();
        var gridManager = FindObjectOfType<GridManager>();
        
        if (gridManager != null)
        {
            for (int x = 0; x < gridManager.GridWidth; x++)
            {
                for (int y = 0; y < gridManager.GridHeight; y++)
                {
                    var cell = gridManager.GetCellAt(x, y);
                    if (cell != null && cell.HasElement())
                    {
                        var element = cell.GetElement();
                        gridElements.Add(new SaveData.GridElementData
                        {
                            x = x,
                            y = y,
                            elementId = element.elementData.name,
                            level = element.elementData.level
                        });
                    }
                }
            }
        }
        
        return gridElements;
    }
    
    private void ApplySaveData(SaveData saveData)
    {
        // Player data
        GameManager.Instance.playerData.playerLevel = saveData.playerLevel;
        GameManager.Instance.playerData.currentXP = saveData.currentXP;
        GameManager.Instance.playerData.coins = saveData.coins;
        GameManager.Instance.playerData.etherGems = saveData.etherGems;
        GameManager.Instance.playerData.essence = saveData.essence;
        GameManager.Instance.playerData.energy = saveData.energy;
        GameManager.Instance.playerData.maxEnergy = saveData.maxEnergy;
        
        // Inventory
        GameManager.Instance.playerData.inventory.Clear();
        foreach (var item in saveData.inventory)
        {
            GameManager.Instance.playerData.inventory.Add(new PlayerData.ElementInventoryItem(
                item.elementId, item.quantity, item.level));
        }
        
        // Achievements
        GameManager.Instance.playerData.completedAchievements.Clear();
        foreach (var achievement in saveData.completedAchievements)
        {
            GameManager.Instance.playerData.completedAchievements.Add(achievement);
        }
        
        // NPC Relationships
        GameManager.Instance.playerData.npcRelationships.Clear();
        foreach (var kvp in saveData.npcRelationships)
        {
            GameManager.Instance.playerData.npcRelationships[kvp.Key] = kvp.Value;
        }
        
        // Unlocked Recipes
        GameManager.Instance.playerData.unlockedRecipes.Clear();
        foreach (var recipe in saveData.unlockedRecipes)
        {
            GameManager.Instance.playerData.unlockedRecipes.Add(recipe);
        }
        
        // Game settings
        GameManager.Instance.enableSoundEffects = saveData.gameSettings.enableSoundEffects;
        GameManager.Instance.enableChainReactions = saveData.gameSettings.enableChainReactions;
        
        // Load grid state
        LoadGridState(saveData.gridElements);
    }
    
    private void LoadGridState(System.Collections.Generic.List<SaveData.GridElementData> gridElements)
    {
        var gridManager = FindObjectOfType<GridManager>();
        var elementSystem = FindObjectOfType<ElementSystem>();
        
        if (gridManager != null && elementSystem != null)
        {
            // Clear existing elements
            for (int x = 0; x < gridManager.GridWidth; x++)
            {
                for (int y = 0; y < gridManager.GridHeight; y++)
                {
                    var cell = gridManager.GetCellAt(x, y);
                    if (cell != null)
                    {
                        cell.RemoveElement();
                    }
                }
            }
            
            // Place saved elements
            foreach (var elementData in gridElements)
            {
                var cell = gridManager.GetCellAt(elementData.x, elementData.y);
                if (cell != null)
                {
                    var element = elementSystem.GetElementData(elementData.elementId);
                    if (element != null)
                    {
                        var gameElement = elementSystem.GetElement(element);
                        cell.SetElement(gameElement);
                    }
                }
            }
        }
    }
    
    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, saveFileName);
    }
}

[System.Serializable]
public class SaveData
{
    // Player Progress
    public int playerLevel;
    public int currentXP;
    public int coins;
    public int etherGems;
    public int essence;
    public int energy;
    public int maxEnergy;
    
    // Inventory
    public System.Collections.Generic.List<InventoryItem> inventory;
    
    // Achievements
    public System.Collections.Generic.List<string> completedAchievements;
    
    // NPC Relationships
    public System.Collections.Generic.Dictionary<string, int> npcRelationships;
    
    // Unlocked Content
    public System.Collections.Generic.List<string> unlockedRecipes;
    
    // Grid State
    public System.Collections.Generic.List<GridElementData> gridElements;
    
    // Settings
    public SettingsData gameSettings;
    
    [System.Serializable]
    public class InventoryItem
    {
        public string elementId;
        public int quantity;
        public int level;
    }
    
    [System.Serializable]
    public class GridElementData
    {
        public int x;
        public int y;
        public string elementId;
        public int level;
    }
    
    [System.Serializable]
    public class SettingsData
    {
        public bool enableSoundEffects;
        public bool enableChainReactions;
    }
}