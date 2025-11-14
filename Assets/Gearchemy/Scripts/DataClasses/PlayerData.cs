using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int playerLevel = 1;
    public int currentXP = 0;
    public int coins = 100;
    public int etherGems = 0;
    public int essence = 0;
    public int energy = 100;
    public int maxEnergy = 100;
    
    public List<ElementInventoryItem> inventory = new List<ElementInventoryItem>();
    public List<string> completedAchievements = new List<string>();
    public Dictionary<string, int> npcRelationships = new Dictionary<string, int>();
    public List<string> unlockedRecipes = new List<string>();
    
    [Serializable]
    public class ElementInventoryItem
    {
        public string elementId;
        public int quantity;
        public int level;
        
        public ElementInventoryItem(string id, int qty, int lvl)
        {
            elementId = id;
            quantity = qty;
            level = lvl;
        }
    }
    
    public bool HasElement(ElementData element, int quantity = 1)
    {
        var item = inventory.Find(i => i.elementId == element.name && i.level == element.level);
        return item != null && item.quantity >= quantity;
    }
    
    public void AddElement(ElementData element, int quantity = 1)
    {
        var item = inventory.Find(i => i.elementId == element.name && i.level == element.level);
        if (item != null)
        {
            item.quantity += quantity;
        }
        else
        {
            inventory.Add(new ElementInventoryItem(element.name, quantity, element.level));
        }
    }
    
    public void RemoveElement(ElementData element, int quantity = 1)
    {
        var item = inventory.Find(i => i.elementId == element.name && i.level == element.level);
        if (item != null)
        {
            item.quantity = Mathf.Max(0, item.quantity - quantity);
            if (item.quantity == 0)
            {
                inventory.Remove(item);
            }
        }
    }
    
    public void AddXP(int amount)
    {
        currentXP += amount;
        int xpForNextLevel = GetXPRequiredForLevel(playerLevel + 1);
        
        while (currentXP >= xpForNextLevel)
        {
            currentXP -= xpForNextLevel;
            playerLevel++;
            xpForNextLevel = GetXPRequiredForLevel(playerLevel + 1);
            
            // Level up rewards
            coins += playerLevel * 50;
            maxEnergy += 10;
            energy = maxEnergy;
        }
    }
    
    private int GetXPRequiredForLevel(int level)
    {
        return 100 * level + (level * level * 10);
    }
    
    public int GetRelationshipLevel(string npcId)
    {
        if (npcRelationships.ContainsKey(npcId))
            return npcRelationships[npcId] / 20; // 0-100 scale to 0-5 levels
        return 0;
    }
    
    public void UpdateRelationship(string npcId, int change)
    {
        if (!npcRelationships.ContainsKey(npcId))
            npcRelationships[npcId] = 0;
            
        npcRelationships[npcId] = Mathf.Clamp(npcRelationships[npcId] + change, 0, 100);
    }
    
    [Serializable]
    public class CurrencyData
    {
        public int coins;
        public int etherGems;
        public int essence;
        public int energy;
        public int maxEnergy;
    }
}