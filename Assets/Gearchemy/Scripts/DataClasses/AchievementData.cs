using UnityEngine;

[CreateAssetMenu(fileName = "NewAchievement", menuName = "Gearchemy/Achievement Data")]
public class AchievementData : ScriptableObject
{
    [Header("Achievement Info")]
    public string achievementId;
    public string achievementName;
    public string description;
    public Sprite icon;
    
    [Header("Requirements")]
    public AchievementType type;
    public string statRequirement; // For stat-based achievements
    public int requiredValue;
    public int requiredPlayerLevel = 1;
    
    [Header("Rewards")]
    public int coinReward;
    public int etherGemReward;
    public int essenceReward;
    public int xpReward;
    public string unlockContent; // ID of content to unlock
    
    [Header("Visual")]
    public bool isCompleted;
    public bool isHidden;
    
    public enum AchievementType
    {
        Merge,
        Generator,
        Craft,
        Quest,
        Level,
        Currency,
        Special
    }
    
    public string GetProgressText()
    {
        if (type == AchievementType.Level)
        {
            return $"Reach level {requiredValue}";
        }
        else if (!string.IsNullOrEmpty(statRequirement))
        {
            return $"{description} ({requiredValue} required)";
        }
        
        return description;
    }
}