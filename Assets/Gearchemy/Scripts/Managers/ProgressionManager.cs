using UnityEngine;
using System.Collections.Generic;

public class ProgressionManager : MonoBehaviour
{
    [Header("Level Settings")]
    public int baseXPRequired = 100;
    public float xpGrowthFactor = 1.5f;
    public int maxLevel = 50;
    
    [Header("Rewards")]
    public int coinsPerLevel = 50;
    public int energyIncreasePerLevel = 10;
    public int inventorySlotIncreasePerLevel = 2;
    
    [Header("Achievements")]
    public List<AchievementData> achievements = new List<AchievementData>();
    
    private Dictionary<string, AchievementData> achievementDictionary = new Dictionary<string, AchievementData>();
    private Dictionary<string, int> achievementProgress = new Dictionary<string, int>();
    
    public void Initialize()
    {
        // Initialize achievement dictionary
        foreach (var achievement in achievements)
        {
            achievementDictionary[achievement.achievementId] = achievement;
            
            // Load progress from player data
            if (GameManager.Instance.playerData.completedAchievements.Contains(achievement.achievementId))
            {
                achievement.isCompleted = true;
            }
        }
        
        // Initialize achievement progress tracking
        InitializeProgressTracking();
    }
    
    private void InitializeProgressTracking()
    {
        achievementProgress["merges"] = 0;
        achievementProgress["generators_built"] = 0;
        achievementProgress["elements_sold"] = 0;
        achievementProgress["quests_completed"] = 0;
        achievementProgress["coins_earned"] = 0;
        achievementProgress["high_level_elements"] = 0;
    }
    
    public int GetXPRequiredForLevel(int level)
    {
        if (level <= 1) return baseXPRequired;
        
        return Mathf.RoundToInt(baseXPRequired * Mathf.Pow(xpGrowthFactor, level - 1));
    }
    
    public void AddXP(int amount)
    {
        var playerData = GameManager.Instance.playerData;
        playerData.currentXP += amount;
        
        // Check for level up
        while (playerData.currentXP >= GetXPRequiredForLevel(playerData.playerLevel + 1) && 
               playerData.playerLevel < maxLevel)
        {
            LevelUp();
        }
        
        // Update UI
        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager?.UpdateLevelDisplay();
    }
    
    private void LevelUp()
    {
        var playerData = GameManager.Instance.playerData;
        int oldLevel = playerData.playerLevel;
        playerData.playerLevel++;
        
        // Reset XP for new level
        playerData.currentXP -= GetXPRequiredForLevel(oldLevel + 1);
        
        // Award level up rewards
        AwardLevelUpRewards(playerData.playerLevel);
        
        // Check for level-based achievements
        CheckLevelAchievements(playerData.playerLevel);
        
        // Unlock new content
        UnlockContentForLevel(playerData.playerLevel);
        
        // Show level up notification
        ShowLevelUpNotification(oldLevel, playerData.playerLevel);
    }
    
    private void AwardLevelUpRewards(int newLevel)
    {
        var playerData = GameManager.Instance.playerData;
        
        // Coins
        playerData.coins += coinsPerLevel * newLevel;
        
        // Energy increase
        playerData.maxEnergy += energyIncreasePerLevel;
        playerData.energy = playerData.maxEnergy; // Refill energy
        
        // Inventory expansion
        // This would be handled by inventory system
        
        // Unlock new recipes
        UnlockRecipesForLevel(newLevel);
    }
    
    private void UnlockContentForLevel(int level)
    {
        // Unlock new generators
        var elementSystem = FindObjectOfType<ElementSystem>();
        if (elementSystem != null)
        {
            // Unlock generators based on level
            foreach (var generator in elementSystem.allGenerators)
            {
                // Example: Unlock generators at specific levels
                if (level == 5 && generator.generatorName == "Steam Boiler")
                {
                    // Unlock logic would go here
                }
            }
        }
    }
    
    private void UnlockRecipesForLevel(int level)
    {
        var craftingManager = FindObjectOfType<CraftingManager>();
        if (craftingManager != null)
        {
            // Unlock recipes based on player level
            foreach (var recipe in craftingManager.allRecipes)
            {
                if (recipe.requiredPlayerLevel <= level && 
                    !GameManager.Instance.playerData.unlockedRecipes.Contains(recipe.recipeName))
                {
                    GameManager.Instance.playerData.unlockedRecipes.Add(recipe.recipeName);
                }
            }
        }
    }
    
    public void TrackProgress(string statName, int value = 1)
    {
        if (achievementProgress.ContainsKey(statName))
        {
            achievementProgress[statName] += value;
            CheckStatAchievements(statName);
        }
    }
    
    private void CheckStatAchievements(string statName)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.statRequirement == statName && !achievement.isCompleted)
            {
                int currentProgress = achievementProgress[statName];
                if (currentProgress >= achievement.requiredValue)
                {
                    CompleteAchievement(achievement.achievementId);
                }
            }
        }
    }
    
    private void CheckLevelAchievements(int level)
    {
        /*
        foreach (var achievement in achievements)
        {
            if (achievement.type == AchievementType.Level && !achievement.isCompleted)
            {
                if (level >= achievement.requiredValue)
                {
                    CompleteAchievement(achievement.achievementId);
                }
            }
        }
        */
    }
    
    public void CompleteAchievement(string achievementId)
    {
        if (achievementDictionary.ContainsKey(achievementId) && 
            !achievementDictionary[achievementId].isCompleted)
        {
            var achievement = achievementDictionary[achievementId];
            achievement.isCompleted = true;
            
            // Add to completed list
            GameManager.Instance.playerData.completedAchievements.Add(achievementId);
            
            // Award achievement rewards
            AwardAchievementRewards(achievement);
            
            // Show achievement notification
            ShowAchievementNotification(achievement);
            
            // Save progress
            GameManager.Instance.SaveGame();
        }
    }
    
    private void AwardAchievementRewards(AchievementData achievement)
    {
        var playerData = GameManager.Instance.playerData;
        
        playerData.coins += achievement.coinReward;
        playerData.etherGems += achievement.etherGemReward;
        playerData.essence += achievement.essenceReward;
        
        // Add XP
        AddXP(achievement.xpReward);
    }
    
    private void ShowLevelUpNotification(int oldLevel, int newLevel)
    {
        // This would show a UI notification
        Debug.Log($"Level Up! {oldLevel} → {newLevel}");
        
        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager?.ShowLevelUpNotification(oldLevel, newLevel);
    }
    
    private void ShowAchievementNotification(AchievementData achievement)
    {
        // This would show a UI notification
        Debug.Log($"Achievement Unlocked: {achievement.achievementName}");
        
        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager?.ShowAchievementNotification(achievement);
    }
    
    public int GetAchievementProgress(string achievementId)
    {
        if (achievementDictionary.ContainsKey(achievementId))
        {
            var achievement = achievementDictionary[achievementId];
            if (achievementProgress.ContainsKey(achievement.statRequirement))
            {
                return achievementProgress[achievement.statRequirement];
            }
        }
        return 0;
    }
    
    public List<AchievementData> GetCompletedAchievements()
    {
        return achievements.FindAll(a => a.isCompleted);
    }
    
    public List<AchievementData> GetAvailableAchievements()
    {
        return achievements.FindAll(a => !a.isCompleted);
    }
    
    public float GetProgressPercentage()
    {
        if (maxLevel <= 1) return 0f;
        
        var playerData = GameManager.Instance.playerData;
        return (float)(playerData.playerLevel - 1) / (maxLevel - 1);
    }
}