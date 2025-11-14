using UnityEngine;
using System.Collections.Generic;

public class NarrativeManager : MonoBehaviour
{
    [Header("NPC Settings")]
    public List<NPCData> npcs = new List<NPCData>();
    public Transform npcSpawnArea;
    public float npcSpawnInterval = 60f; // seconds
    public int maxActiveNPCs = 3;
    
    [Header("Quest Settings")]
    public List<QuestData> availableQuests = new List<QuestData>();
    public int maxActiveQuests = 5;
    
    private List<GameObject> activeNPCs = new List<GameObject>();
    private List<QuestData> activeQuests = new List<QuestData>();
    private float lastSpawnTime;
    
    public void Initialize()
    {
        // Initialize NPC relationships
        foreach (var npc in npcs)
        {
            if (!GameManager.Instance.playerData.npcRelationships.ContainsKey(npc.npcId))
            {
                GameManager.Instance.playerData.npcRelationships[npc.npcId] = 0;
            }
        }
        
        // Start NPC spawning
        lastSpawnTime = Time.time;
    }
    
    private void Update()
    {
        // Check for NPC spawning
        if (Time.time - lastSpawnTime >= npcSpawnInterval && activeNPCs.Count < maxActiveNPCs)
        {
            SpawnRandomNPC();
            lastSpawnTime = Time.time;
        }
        
        // Update active quests
        UpdateActiveQuests();
    }
    
    private void SpawnRandomNPC()
    {
        if (npcs.Count == 0) return;
        
        // Filter NPCs based on player level and relationships
        var availableNPCs = npcs.FindAll(npc => 
            npc.requiredPlayerLevel <= GameManager.Instance.playerData.playerLevel &&
            !IsNPCActive(npc.npcId));
        
        if (availableNPCs.Count == 0) return;
        
        NPCData selectedNPC = availableNPCs[Random.Range(0, availableNPCs.Count)];
        SpawnNPC(selectedNPC);
    }
    
    private void SpawnNPC(NPCData npcData)
    {
        if (npcData == null)
        {
            Debug.LogError("NarrativeManager: Cannot spawn NPC with null data");
            return;
        }
        
        if (npcData.prefab == null)
        {
            Debug.LogError($"NarrativeManager: NPC {npcData.npcName} has no prefab assigned");
            return;
        }
        
        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject npcObject = Instantiate(npcData.prefab, spawnPosition, Quaternion.identity, npcSpawnArea);
        
        if (npcObject == null)
        {
            Debug.LogError("NarrativeManager: Failed to instantiate NPC prefab");
            return;
        }
        
        var controller = npcObject.GetComponent<NPCController>();
        if (controller != null)
        {
            controller.Initialize(npcData);
            controller.OnQuestOffered += OnQuestOffered;
            controller.OnDialogueStarted += OnDialogueStarted;
            controller.OnDialogueEnded += OnDialogueEnded;
            
            // Validate the controller setup
            if (!controller.IsValid())
            {
                Debug.LogWarning($"NarrativeManager: NPC {npcData.npcName} controller has missing components");
            }
        }
        else
        {
            Debug.LogError($"NarrativeManager: NPC prefab for {npcData.npcName} has no NPCController component");
        }
        
        activeNPCs.Add(npcObject);
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        // Return a random position within the spawn area
        if (npcSpawnArea != null)
        {
            Bounds bounds = npcSpawnArea.GetComponent<Collider>()?.bounds ?? new Bounds(npcSpawnArea.position, Vector3.one * 5);
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                npcSpawnArea.position.z
            );
        }
        return Vector3.zero;
    }
    
    private bool IsNPCActive(string npcId)
    {
        foreach (var npcObject in activeNPCs)
        {
            var controller = npcObject.GetComponent<NPCController>();
            if (controller != null && controller.npcData.npcId == npcId)
            {
                return true;
            }
        }
        return false;
    }
    
    private void OnQuestOffered(QuestData quest)
    {
        if (activeQuests.Count < maxActiveQuests && !activeQuests.Contains(quest))
        {
            activeQuests.Add(quest);
            Debug.Log($"New quest offered: {quest.questName}");
        }
    }
    
    private void OnDialogueStarted(NPCController npc)
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Dialogue);
        // Pause game or show dialogue UI
    }
    
    private void OnDialogueEnded(NPCController npc)
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Playing);
        // Resume game
    }
    
    private void UpdateActiveQuests()
    {
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            var quest = activeQuests[i];
            if (IsQuestComplete(quest))
            {
                CompleteQuest(quest);
                activeQuests.RemoveAt(i);
            }
        }
    }
    
    private bool IsQuestComplete(QuestData quest)
    {
        switch (quest.questType)
        {
            case QuestData.QuestType.Collect:
                return GameManager.Instance.playerData.HasElement(quest.requiredElement, quest.requiredQuantity);
                
            case QuestData.QuestType.Merge:
                // Check if player has merged enough elements
                return GetMergeCount(quest.requiredElement) >= quest.requiredQuantity;
                
            case QuestData.QuestType.ReachLevel:
                return GameManager.Instance.playerData.playerLevel >= quest.requiredLevel;
                
            case QuestData.QuestType.Craft:
                // Check if player has crafted the item
                return HasCraftedItem(quest.requiredElement);
                
            default:
                return false;
        }
    }
    
    private void CompleteQuest(QuestData quest)
    {
        // Remove required items from inventory
        if (quest.questType == QuestData.QuestType.Collect)
        {
            GameManager.Instance.playerData.RemoveElement(quest.requiredElement, quest.requiredQuantity);
        }
        
        // Award rewards
        var economyManager = FindObjectOfType<EconomyManager>();
        if (economyManager != null)
        {
            economyManager.AwardQuestReward(quest.coinReward, quest.xpReward, quest.etherGemReward);
        }
        
        // Update NPC relationship
        UpdateNPCRelationship(quest.giverNPCId, quest.reputationReward);
        
        // Show completion notification
        ShowQuestCompletion(quest);
        
        // Track quest completion
        var progressionManager = FindObjectOfType<ProgressionManager>();
        progressionManager?.TrackProgress("quests_completed");
    }
    
    private void UpdateNPCRelationship(string npcId, int reputationChange)
    {
        GameManager.Instance.playerData.UpdateRelationship(npcId, reputationChange);
        
        // Check for relationship-based achievements
        int currentRelationship = GameManager.Instance.playerData.GetRelationshipLevel(npcId);
        if (currentRelationship >= 4) // Legendary level
        {
            var progressionManager = FindObjectOfType<ProgressionManager>();
            progressionManager?.CompleteAchievement($"legendary_{npcId}");
        }
    }
    
    private void ShowQuestCompletion(QuestData quest)
    {
        Debug.Log($"Quest completed: {quest.questName}");
        // This would show a UI notification
    }
    
    private int GetMergeCount(ElementData elementType)
    {
        // This would track merges from the element system
        // For now, return a placeholder
        return 0;
    }
    
    private bool HasCraftedItem(ElementData item)
    {
        // This would check the crafting system
        // For now, return false
        return false;
    }
    
    public void RemoveNPC(GameObject npcObject)
    {
        if (npcObject == null)
        {
            Debug.LogWarning("NarrativeManager: Attempting to remove null NPC object");
            return;
        }
        
        if (activeNPCs.Contains(npcObject))
        {
            // Clean up event listeners before destroying
            var controller = npcObject.GetComponent<NPCController>();
            if (controller != null)
            {
                controller.OnQuestOffered -= OnQuestOffered;
                controller.OnDialogueStarted -= OnDialogueStarted;
                controller.OnDialogueEnded -= OnDialogueEnded;
            }
            
            activeNPCs.Remove(npcObject);
            Destroy(npcObject);
        }
        else
        {
            Debug.LogWarning($"NarrativeManager: NPC object {npcObject.name} is not in active NPCs list");
        }
    }
    
    public List<QuestData> GetActiveQuests()
    {
        return new List<QuestData>(activeQuests);
    }
    
    public List<NPCData> GetAvailableNPCs()
    {
        return npcs;
    }
    
    public NPCData GetNPCData(string npcId)
    {
        return npcs.Find(npc => npc.npcId == npcId);
    }
}