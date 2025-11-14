using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCController : MonoBehaviour
{
    [Header("NPC References")]
    public NPCData npcData;
    public GameObject speechBubblePrefab;
    public Transform speechBubblePoint;
    
    [Header("UI Components")]
    public Image portraitImage;
    public TextMeshProUGUI nameText;
    public Button interactButton;
    
    // Events
    public System.Action<QuestData> OnQuestOffered;
    public System.Action<NPCController> OnDialogueStarted;
    public System.Action<NPCController> OnDialogueEnded;
    
    private GameObject currentSpeechBubble;
    private bool isInDialogue = false;
    
    private void Awake()
    {
        // Validate required components
        ValidateComponents();
    }
    
    private void ValidateComponents()
    {
        if (speechBubblePrefab == null)
        {
            Debug.LogWarning($"NPCController on {gameObject.name}: Speech bubble prefab is not assigned");
        }
        
        if (speechBubblePoint == null)
        {
            Debug.LogWarning($"NPCController on {gameObject.name}: Speech bubble spawn point is not assigned");
        }
        
        if (portraitImage == null)
        {
            Debug.LogWarning($"NPCController on {gameObject.name}: Portrait image component is not assigned");
        }
        
        if (nameText == null)
        {
            Debug.LogWarning($"NPCController on {gameObject.name}: Name text component is not assigned");
        }
        
        if (interactButton == null)
        {
            Debug.LogWarning($"NPCController on {gameObject.name}: Interact button component is not assigned");
        }
    }
    
    public void Initialize(NPCData data)
    {
        if (data == null)
        {
            Debug.LogError("NPCController: Cannot initialize with null NPCData");
            return;
        }
        
        npcData = data;
        
        // Set up UI
        if (portraitImage != null && npcData.portrait != null)
        {
            portraitImage.sprite = npcData.portrait;
        }
        
        if (nameText != null && npcData != null)
        {
            nameText.text = npcData.npcName;
        }
        
        // Set up interact button
        if (interactButton != null)
        {
            interactButton.onClick.AddListener(OnInteract);
        }
        
        // Show initial speech bubble
        if (npcData != null)
        {
            ShowSpeechBubble(npcData.GetRandomPhrase());
        }
    }
    
    private void OnInteract()
    {
        if (npcData == null)
        {
            Debug.LogError("NPCController: Cannot interact - NPC data is null");
            return;
        }
        
        if (!isInDialogue)
        {
            StartDialogue();
        }
    }
    
    private void StartDialogue()
    {
        if (npcData == null)
        {
            Debug.LogError("NPCController: Cannot start dialogue - NPC data is null");
            return;
        }
        
        isInDialogue = true;
        OnDialogueStarted?.Invoke(this);
        
        // Hide speech bubble during dialogue
        HideSpeechBubble();
        
        // This would typically show a dialogue UI
        Debug.Log($"Starting dialogue with {npcData.npcName}: {npcData.defaultDialogue}");
        
        // For now, just end dialogue after a delay
        Invoke("EndDialogue", 2f);
    }
    
    private void EndDialogue()
    {
        isInDialogue = false;
        OnDialogueEnded?.Invoke(this);
        
        // Show random phrase after dialogue
        if (npcData != null)
        {
            ShowSpeechBubble(npcData.GetRandomPhrase());
        }
    }
    
    public void ShowSpeechBubble(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("NPCController: Attempting to show speech bubble with empty text");
            return;
        }
        
        if (speechBubblePrefab != null && speechBubblePoint != null)
        {
            // Remove existing speech bubble
            HideSpeechBubble();
            
            // Create new speech bubble
            currentSpeechBubble = Instantiate(speechBubblePrefab, speechBubblePoint);
            
            if (currentSpeechBubble == null)
            {
                Debug.LogError("NPCController: Failed to instantiate speech bubble prefab");
                return;
            }
            
            // Set text
            TextMeshProUGUI bubbleText = currentSpeechBubble.GetComponentInChildren<TextMeshProUGUI>();
            if (bubbleText != null)
            {
                bubbleText.text = text;
            }
            else
            {
                Debug.LogWarning("NPCController: No TextMeshProUGUI component found in speech bubble prefab");
            }
            
            // Set color based on NPC personality
            Image bubbleImage = currentSpeechBubble.GetComponentInChildren<Image>();
            if (bubbleImage != null && npcData != null)
            {
                bubbleImage.color = npcData.speechBubbleColor;
            }
        }
        else
        {
            Debug.LogWarning("NPCController: Cannot show speech bubble - missing prefab or spawn point");
        }
    }
    
    public void HideSpeechBubble()
    {
        if (currentSpeechBubble != null)
        {
            Destroy(currentSpeechBubble);
            currentSpeechBubble = null;
        }
    }
    
    public void OfferQuest(QuestData quest)
    {
        if (npcData == null)
        {
            Debug.LogError("NPCController: Cannot offer quest - NPC data is null");
            return;
        }
        
        if (quest == null)
        {
            Debug.LogError("NPCController: Cannot offer null quest");
            return;
        }
        
        if (npcData.CanOfferQuests())
        {
            OnQuestOffered?.Invoke(quest);
            Debug.Log($"Quest offered by {npcData.npcName}: {quest.questName}");
        }
        else
        {
            Debug.LogWarning($"NPCController: {npcData.npcName} cannot offer quests");
        }
    }
    
    public bool CanTrade()
    {
        return npcData != null && npcData.CanTrade();
    }
    
    private void OnDestroy()
    {
        // Clean up event listeners
        if (interactButton != null)
        {
            interactButton.onClick.RemoveListener(OnInteract);
        }
        
        // Cancel any pending invoke calls
        CancelInvoke();
        
        HideSpeechBubble();
    }
    
    private void OnDisable()
    {
        // Clean up when disabled
        CancelInvoke();
        HideSpeechBubble();
    }
    
    public bool IsValid()
    {
        return npcData != null &&
               speechBubblePrefab != null &&
               speechBubblePoint != null &&
               portraitImage != null &&
               nameText != null &&
               interactButton != null;
}
}