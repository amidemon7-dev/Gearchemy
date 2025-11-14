using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public int gridWidth = 6;
    public int gridHeight = 6;
    public bool enableChainReactions = true;
    public bool enableSoundEffects = true;
    
    [Header("Game State")]
    public GameState currentState = GameState.MainMenu;
    public PlayerData playerData;
    
    // Managers
    private GridManager gridManager;
    private ElementSystem elementSystem;
    private EconomyManager economyManager;
    private ProgressionManager progressionManager;
    private NarrativeManager narrativeManager;
    private CraftingManager craftingManager;
    private SaveSystem saveSystem;
    private UIManager uiManager;
    
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Crafting,
        Dialogue,
        Shop,
        Settings
    }
    
    public static event Action<GameState> OnGameStateChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeGame()
    {
        // Initialize player data
        playerData = new PlayerData();
        
        // Get manager references
        gridManager = GetComponent<GridManager>();
        elementSystem = GetComponent<ElementSystem>();
        economyManager = GetComponent<EconomyManager>();
        progressionManager = GetComponent<ProgressionManager>();
        narrativeManager = GetComponent<NarrativeManager>();
        craftingManager = GetComponent<CraftingManager>();
        saveSystem = GetComponent<SaveSystem>();
        uiManager = GetComponent<UIManager>();
        
        // Load saved game data
        saveSystem?.LoadGame();
        
        // Initialize all systems
        gridManager?.InitializeGrid(gridWidth, gridHeight);
        elementSystem?.Initialize();
        economyManager?.Initialize();
        progressionManager?.Initialize();
        narrativeManager?.Initialize();
        craftingManager?.Initialize();
        uiManager?.Initialize();
        
        ChangeState(GameState.Playing);
    }
    
    public void ChangeState(GameState newState)
    {
        if (currentState != newState)
        {
            GameState oldState = currentState;
            currentState = newState;
            OnGameStateChanged?.Invoke(newState);
            
            HandleStateChange(oldState, newState);
        }
    }
    
    private void HandleStateChange(GameState oldState, GameState newState)
    {
        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
                
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
                
            case GameState.Crafting:
                uiManager?.ShowCraftingUI();
                break;
                
            case GameState.Dialogue:
                uiManager?.ShowDialogueUI();
                break;
                
            case GameState.Shop:
                uiManager?.ShowShopUI();
                break;
        }
    }
    
    public void SaveGame()
    {
        saveSystem?.SaveGame();
    }
    
    public void LoadGame()
    {
        saveSystem?.LoadGame();
    }
    
    public void ResetGame()
    {
        playerData = new PlayerData();
        saveSystem?.DeleteSave();
        InitializeGame();
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }
    
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}