using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Gearchemy/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Quest Info")]
    public string questId;
    public string questName;
    public string description;
    public Sprite icon;
    
    [Header("Quest Giver")]
    public string giverNPCId;
    public string questDialogue;
    public string completionDialogue;
    
    [Header("Quest Type")]
    public QuestType questType;
    public ElementData requiredElement;
    public int requiredQuantity = 1;
    public int requiredLevel = 1;
    
    [Header("Rewards")]
    public int coinReward;
    public int xpReward;
    public int etherGemReward;
    public int reputationReward;
    public ElementData itemReward;
    public string unlockContent;
    
    [Header("Quest Properties")]
    public QuestDifficulty difficulty;
    public bool isRepeatable;
    public int maxRepeats = 1;
    public bool isActive;
    public bool isCompleted;
    
    public enum QuestType
    {
        Collect,      // Collect specific elements
        Merge,        // Merge elements of specific type
        ReachLevel,   // Reach a certain player level
        Craft,        // Craft specific items
        Explore,      // Explore or discover something
        Help          // Help NPC with tasks
    }
    
    public enum QuestDifficulty
    {
        Easy,
        Medium,
        Hard,
        Legendary
    }
    
    public bool IsAvailable()
    {
        return !isActive && (!isCompleted || isRepeatable);
    }
    
    public bool CanAcceptQuest()
    {
        return GameManager.Instance.playerData.playerLevel >= requiredLevel;
    }
    
    public string GetProgressText()
    {
        switch (questType)
        {
            case QuestType.Collect:
                int currentCount = GameManager.Instance.playerData.HasElement(requiredElement, 0) ? 
                    GameManager.Instance.playerData.inventory.Find(i => i.elementId == requiredElement.name)?.quantity ?? 0 : 0;
                return $"{currentCount}/{requiredQuantity} {requiredElement.elementName}";
                
            case QuestType.Merge:
                return $"Merge {requiredQuantity} {requiredElement.elementName} elements";
                
            case QuestType.ReachLevel:
                return $"Reach level {requiredLevel} (Current: {GameManager.Instance.playerData.playerLevel})";
                
            case QuestType.Craft:
                return $"Craft {requiredQuantity} {requiredElement.elementName}";
                
            default:
                return description;
        }
    }
    
    public int GetDifficultyMultiplier()
    {
        switch (difficulty)
        {
            case QuestDifficulty.Easy: return 1;
            case QuestDifficulty.Medium: return 2;
            case QuestDifficulty.Hard: return 3;
            case QuestDifficulty.Legendary: return 5;
            default: return 1;
        }
    }
    
    public void MarkCompleted()
    {
        isCompleted = true;
        isActive = false;
        
        if (!isRepeatable)
        {
            // Mark as permanently completed
        }
    }
}