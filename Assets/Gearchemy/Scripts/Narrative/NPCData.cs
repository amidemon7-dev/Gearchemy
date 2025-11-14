using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "Gearchemy/NPC Data")]
public class NPCData : ScriptableObject
{
    [Header("NPC Info")]
    public string npcId;
    public string npcName;
    public string description;
    public Sprite portrait;
    public GameObject prefab;
    
    [Header("NPC Properties")]
    public NPCType npcType;
    public int requiredPlayerLevel = 1;
    public string defaultDialogue;
    public Color speechBubbleColor = Color.white;
    
    [Header("Personality")]
    public PersonalityTrait[] personalityTraits;
    public string[] randomPhrases;
    
    [Header("Quest Preferences")]
    public ElementData.ElementType preferredElementType;
    public int minQuestLevel = 1;
    public int maxQuestLevel = 10;
    
    public enum NPCType
    {
        Elder,
        Merchant,
        Guard,
        Child,
        Scholar,
        Artisan
    }
    
    [System.Serializable]
    public class PersonalityTrait
    {
        public string traitName;
        public float intensity; // 0-1
        public string description;
    }
    
    public string GetRandomPhrase()
    {
        if (randomPhrases.Length > 0)
        {
            return randomPhrases[Random.Range(0, randomPhrases.Length)];
        }
        return defaultDialogue;
    }
    
    public bool CanOfferQuests()
    {
        return npcType == NPCType.Elder || npcType == NPCType.Merchant || 
               npcType == NPCType.Guard || npcType == NPCType.Artisan;
    }
    
    public bool CanTrade()
    {
        return npcType == NPCType.Merchant;
    }
}