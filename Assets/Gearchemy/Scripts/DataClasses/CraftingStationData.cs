using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCraftingStation", menuName = "Gearchemy/Crafting Station Data")]
public class CraftingStationData : ScriptableObject
{
    [Header("Station Info")]
    public string stationName;
    public string description;
    public Sprite stationIcon;
    public GameObject stationPrefab;
    
    [Header("Station Properties")]
    public RecipeData.CraftingStation stationType;
    public int requiredPlayerLevel = 1;
    public int unlockCost = 100;
    public bool isUnlocked = false;
    
    [Header("Station Bonuses")]
    public float successRateBonus = 0f; // Percentage bonus to success rate
    public float craftingTimeReduction = 0f; // Percentage reduction in crafting time
    public float qualityImprovement = 0f; // Percentage improvement to quality outcomes
    
    [Header("Visual Properties")]
    public Color stationColor = Color.white;
    public ParticleSystem craftingEffect;
    public AudioClip craftingSound;
    public AudioClip completionSound;
    
    [Header("Upgrade Properties")]
    public List<StationUpgrade> availableUpgrades = new List<StationUpgrade>();
    public int currentUpgradeLevel = 0;
    
    [System.Serializable]
    public class StationUpgrade
    {
        public string upgradeName;
        public string description;
        public int cost;
        public float successRateBonus;
        public float craftingTimeReduction;
        public float qualityImprovement;
        public Sprite upgradeIcon;
    }
    
    public bool IsAvailable(PlayerData playerData)
    {
        return playerData.playerLevel >= requiredPlayerLevel && playerData.coins >= unlockCost;
    }
    
    public bool CanUnlock(PlayerData playerData)
    {
        return !isUnlocked && IsAvailable(playerData);
    }
    
    public void UnlockStation()
    {
        isUnlocked = true;
    }
    
    public float GetTotalSuccessRateBonus()
    {
        float bonus = successRateBonus;
        
        // Add upgrade bonuses
        for (int i = 0; i < currentUpgradeLevel && i < availableUpgrades.Count; i++)
        {
            bonus += availableUpgrades[i].successRateBonus;
        }
        
        return bonus;
    }
    
    public float GetTotalCraftingTimeReduction()
    {
        float reduction = craftingTimeReduction;
        
        // Add upgrade reductions
        for (int i = 0; i < currentUpgradeLevel && i < availableUpgrades.Count; i++)
        {
            reduction += availableUpgrades[i].craftingTimeReduction;
        }
        
        return reduction;
    }
    
    public float GetTotalQualityImprovement()
    {
        float improvement = qualityImprovement;
        
        // Add upgrade improvements
        for (int i = 0; i < currentUpgradeLevel && i < availableUpgrades.Count; i++)
        {
            improvement += availableUpgrades[i].qualityImprovement;
        }
        
        return improvement;
    }
    
    public bool CanUpgrade(PlayerData playerData)
    {
        if (currentUpgradeLevel >= availableUpgrades.Count) return false;
        
        var nextUpgrade = availableUpgrades[currentUpgradeLevel];
        return playerData.coins >= nextUpgrade.cost;
    }
    
    public void ApplyUpgrade(PlayerData playerData)
    {
        if (CanUpgrade(playerData))
        {
            var upgrade = availableUpgrades[currentUpgradeLevel];
            playerData.coins -= upgrade.cost;
            currentUpgradeLevel++;
            
            Debug.Log($"Crafting station {stationName} upgraded to level {currentUpgradeLevel}");
        }
    }
}