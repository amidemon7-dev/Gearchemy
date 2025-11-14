using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Main UI Panels")]
    public GameObject mainGamePanel;
    public GameObject craftingPanel;
    public GameObject dialoguePanel;
    public GameObject shopPanel;
    public GameObject settingsPanel;
    public GameObject pausePanel;
    
    [Header("Resource Display")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI etherGemsText;
    public TextMeshProUGUI essenceText;
    public TextMeshProUGUI energyText;
    public Slider energySlider;
    
    [Header("Level Display")]
    public TextMeshProUGUI levelText;
    public Slider xpSlider;
    public TextMeshProUGUI xpText;
    
    [Header("Bottom Navigation")]
    public Button inventoryButton;
    public Button questsButton;
    public Button shopButton;
    public Button achievementsButton;
    public Button settingsButton;
    
    private void Start()
    {
        InitializeUI();
    }
    
    public void Initialize()
    {
        // Set up button listeners
        inventoryButton?.onClick.AddListener(() => ShowInventoryUI());
        questsButton?.onClick.AddListener(() => ShowQuestsUI());
        shopButton?.onClick.AddListener(() => ShowShopUI());
        achievementsButton?.onClick.AddListener(() => ShowAchievementsUI());
        settingsButton?.onClick.AddListener(() => ShowSettingsUI());
        
        // Hide all panels initially
        HideAllPanels();
        ShowMainGameUI();
        
        // Update displays
        UpdateCurrencyDisplay();
        UpdateLevelDisplay();
    }
    
    private void InitializeUI()
    {
        // Additional UI initialization
    }
    
    public void ShowMainGameUI()
    {
        HideAllPanels();
        mainGamePanel.SetActive(true);
        GameManager.Instance?.ChangeState(GameManager.GameState.Playing);
    }
    
    public void ShowCraftingUI()
    {
        HideAllPanels();
        craftingPanel.SetActive(true);
        GameManager.Instance?.ChangeState(GameManager.GameState.Crafting);
    }
    
    public void ShowDialogueUI()
    {
        HideAllPanels();
        dialoguePanel.SetActive(true);
        GameManager.Instance?.ChangeState(GameManager.GameState.Dialogue);
    }
    
    public void ShowShopUI()
    {
        HideAllPanels();
        shopPanel.SetActive(true);
        GameManager.Instance?.ChangeState(GameManager.GameState.Shop);
    }
    
    public void ShowSettingsUI()
    {
        HideAllPanels();
        settingsPanel.SetActive(true);
        GameManager.Instance?.ChangeState(GameManager.GameState.Settings);
    }
    
    public void ShowInventoryUI()
    {
        // This would show the inventory panel
        Debug.Log("Showing Inventory UI");
    }
    
    public void ShowQuestsUI()
    {
        // This would show the quests panel
        Debug.Log("Showing Quests UI");
    }
    
    public void ShowAchievementsUI()
    {
        // This would show the achievements panel
        Debug.Log("Showing Achievements UI");
    }
    
    public void ShowPauseUI()
    {
        HideAllPanels();
        pausePanel.SetActive(true);
        GameManager.Instance?.ChangeState(GameManager.GameState.Paused);
    }
    
    private void HideAllPanels()
    {
        mainGamePanel?.SetActive(false);
        craftingPanel?.SetActive(false);
        dialoguePanel?.SetActive(false);
        shopPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        pausePanel?.SetActive(false);
    }
    
    public void UpdateCurrencyDisplay()
    {
        if (GameManager.Instance?.playerData != null)
        {
            var playerData = GameManager.Instance.playerData;
            
            if (coinsText != null)
                coinsText.text = playerData.coins.ToString();
            
            if (etherGemsText != null)
                etherGemsText.text = playerData.etherGems.ToString();
            
            if (essenceText != null)
                essenceText.text = playerData.essence.ToString();
            
            if (energyText != null)
                energyText.text = $"{playerData.energy}/{playerData.maxEnergy}";
            
            if (energySlider != null)
            {
                energySlider.maxValue = playerData.maxEnergy;
                energySlider.value = playerData.energy;
            }
        }
    }
    
    public void UpdateLevelDisplay()
    {
        if (GameManager.Instance?.playerData != null)
        {
            var playerData = GameManager.Instance.playerData;
            
            if (levelText != null)
                levelText.text = $"Level {playerData.playerLevel}";
            
            if (xpSlider != null)
            {
                int xpRequired = GetXPRequiredForLevel(playerData.playerLevel + 1);
                xpSlider.maxValue = xpRequired;
                xpSlider.value = playerData.currentXP;
            }
            
            if (xpText != null)
            {
                int xpRequired = GetXPRequiredForLevel(playerData.playerLevel + 1);
                xpText.text = $"{playerData.currentXP}/{xpRequired} XP";
            }
        }
    }
    
    private int GetXPRequiredForLevel(int level)
    {
        var progressionManager = FindObjectOfType<ProgressionManager>();
        return progressionManager != null ? progressionManager.GetXPRequiredForLevel(level) : 100;
    }
    
    public void ShowLevelUpNotification(int oldLevel, int newLevel)
    {
        // This would show a fancy level up animation
        Debug.Log($"Level Up! {oldLevel} → {newLevel}");
        
        // Show notification popup
        // This could be a floating text or a modal popup
    }
    
    public void ShowAchievementNotification(AchievementData achievement)
    {
        // This would show an achievement unlock notification
        Debug.Log($"Achievement Unlocked: {achievement.achievementName}");
        
        // Show notification popup
        // This could be a toast notification or achievement popup
    }
    
    public void TogglePause()
    {
        if (GameManager.Instance.currentState == GameManager.GameState.Paused)
        {
            ShowMainGameUI();
        }
        else
        {
            ShowPauseUI();
        }
    }
    
    public void OnPauseButtonClicked()
    {
        TogglePause();
    }
    
    public void OnResumeButtonClicked()
    {
        ShowMainGameUI();
    }
    
    public void OnMainMenuButtonClicked()
    {
        // Return to main menu
        // This would load the main menu scene
    }
    
    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}