using UnityEngine;
using System.Collections.Generic;

public class EconomyManager : MonoBehaviour
{
    [Header("Currency Settings")]
    public int startingCoins = 100;
    public int startingEtherGems = 0;
    public int startingEssence = 0;
    public int startingEnergy = 100;
    public int maxEnergy = 100;
    public float energyRegenRate = 1f; // Energy per minute
    
    [Header("Pricing")]
    public AnimationCurve elementValueCurve = AnimationCurve.EaseInOut(0, 1, 10, 1000);
    public int generatorBaseCost = 100;
    public int inventorySlotCost = 50;
    public int energyRefillCost = 10; // Cost per energy point
    
    private float lastEnergyRegenTime;
    
    public void Initialize()
    {
        if (GameManager.Instance.playerData == null)
        {
            GameManager.Instance.playerData = new PlayerData();
        }
        
        // Set starting values if first time
        if (GameManager.Instance.playerData.coins == 0)
        {
            GameManager.Instance.playerData.coins = startingCoins;
            GameManager.Instance.playerData.etherGems = startingEtherGems;
            GameManager.Instance.playerData.essence = startingEssence;
            GameManager.Instance.playerData.energy = startingEnergy;
            GameManager.Instance.playerData.maxEnergy = maxEnergy;
        }
        
        lastEnergyRegenTime = Time.time;
    }
    
    private void Update()
    {
        // Regenerate energy over time
        RegenerateEnergy();
    }
    
    private void RegenerateEnergy()
    {
        float timeSinceLastRegen = Time.time - lastEnergyRegenTime;
        float regenInterval = 60f / energyRegenRate; // Convert per minute to per second
        
        if (timeSinceLastRegen >= regenInterval)
        {
            int energyToAdd = Mathf.FloorToInt(timeSinceLastRegen / regenInterval);
            AddEnergy(energyToAdd);
            lastEnergyRegenTime = Time.time;
        }
    }
    
    public bool CanAfford(int coinCost, int etherGemCost = 0, int essenceCost = 0)
    {
        var playerData = GameManager.Instance.playerData;
        return playerData.coins >= coinCost && 
               playerData.etherGems >= etherGemCost && 
               playerData.essence >= essenceCost;
    }
    
    public bool SpendCurrency(int coinCost, int etherGemCost = 0, int essenceCost = 0)
    {
        if (!CanAfford(coinCost, etherGemCost, essenceCost))
            return false;
        
        var playerData = GameManager.Instance.playerData;
        playerData.coins -= coinCost;
        playerData.etherGems -= etherGemCost;
        playerData.essence -= essenceCost;
        
        TriggerCurrencyUpdate();
        return true;
    }
    
    public void AddCoins(int amount)
    {
        GameManager.Instance.playerData.coins += amount;
        TriggerCurrencyUpdate();
    }
    
    public void AddEtherGems(int amount)
    {
        GameManager.Instance.playerData.etherGems += amount;
        TriggerCurrencyUpdate();
    }
    
    public void AddEssence(int amount)
    {
        GameManager.Instance.playerData.essence += amount;
        TriggerCurrencyUpdate();
    }
    
    public void AddEnergy(int amount)
    {
        var playerData = GameManager.Instance.playerData;
        playerData.energy = Mathf.Min(playerData.energy + amount, playerData.maxEnergy);
        TriggerCurrencyUpdate();
    }
    
    public bool UseEnergy(int amount)
    {
        var playerData = GameManager.Instance.playerData;
        if (playerData.energy >= amount)
        {
            playerData.energy -= amount;
            TriggerCurrencyUpdate();
            return true;
        }
        return false;
    }
    
    public int GetElementSellValue(ElementData element)
    {
        int baseValue = element.baseValue;
        int levelMultiplier = Mathf.RoundToInt(elementValueCurve.Evaluate(element.level));
        int rarityMultiplier = GetRarityMultiplier(element.rarity);
        
        return baseValue * levelMultiplier * rarityMultiplier;
    }
    
    public bool SellElement(ElementData element)
    {
        if (GameManager.Instance.playerData.HasElement(element, 1))
        {
            int sellValue = GetElementSellValue(element);
            GameManager.Instance.playerData.RemoveElement(element, 1);
            AddCoins(sellValue);
            
            // Add XP for selling
            GameManager.Instance.playerData.AddXP(5);
            
            return true;
        }
        return false;
    }
    
    public int GetGeneratorCost(GeneratorData generator, int level = 1)
    {
        return generatorBaseCost * level;
    }
    
    public bool BuyGenerator(GeneratorData generator)
    {
        int cost = GetGeneratorCost(generator);
        
        if (SpendCurrency(cost))
        {
            // Add generator to inventory or place on grid
            // This would be handled by the grid system
            return true;
        }
        return false;
    }
    
    public bool BuyEnergy(int amount)
    {
        int cost = amount * energyRefillCost;
        
        if (SpendCurrency(cost))
        {
            AddEnergy(amount);
            return true;
        }
        return false;
    }
    
    public bool BuyInventorySlot()
    {
        if (SpendCurrency(inventorySlotCost))
        {
            // Increase inventory size
            // This would be handled by the inventory system
            return true;
        }
        return false;
    }
    
    public int GetRarityMultiplier(ElementData.Rarity rarity)
    {
        switch (rarity)
        {
            case ElementData.Rarity.Common: return 1;
            case ElementData.Rarity.Uncommon: return 2;
            case ElementData.Rarity.Rare: return 4;
            case ElementData.Rarity.Epic: return 8;
            case ElementData.Rarity.Legendary: return 16;
            default: return 1;
        }
    }
    
    public void AwardMergeBonus(ElementData mergedElement)
    {
        // Award bonus currency for merging high-level elements
        int level = mergedElement.level;
        int coinBonus = level * 2;
        int xpBonus = level * 5;
        
        AddCoins(coinBonus);
        GameManager.Instance.playerData.AddXP(xpBonus);
        
        // Chance for bonus premium currency
        if (level >= 5 && Random.Range(0, 100) < 10) // 10% chance for level 5+ elements
        {
            AddEtherGems(1);
        }
    }
    
    public void AwardQuestReward(int coinReward, int xpReward, int etherGemReward = 0)
    {
        AddCoins(coinReward);
        AddEtherGems(etherGemReward);
        GameManager.Instance.playerData.AddXP(xpReward);
    }
    
    private void TriggerCurrencyUpdate()
    {
        // Notify UI and other systems about currency change
        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager?.UpdateCurrencyDisplay();
    }
    
    public PlayerData.CurrencyData GetCurrencyData()
    {
        var playerData = GameManager.Instance.playerData;
        return new PlayerData.CurrencyData
        {
            coins = playerData.coins,
            etherGems = playerData.etherGems,
            essence = playerData.essence,
            energy = playerData.energy,
            maxEnergy = playerData.maxEnergy
        };
    }
}