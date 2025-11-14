using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeListItem : MonoBehaviour
{
    [Header("UI Components")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public Image backgroundImage;
    public Button selectButton;
    
    [Header("Colors")]
    public Color availableColor = Color.white;
    public Color unavailableColor = Color.gray;
    public Color selectedColor = Color.yellow;
    
    private RecipeData recipeData;
    private System.Action<RecipeData> onSelectedCallback;
    private bool isSelected = false;
    
    public void Initialize(RecipeData recipe, System.Action<RecipeData> onSelected)
    {
        if (recipe == null)
        {
            Debug.LogWarning("Invalid recipe data");
            return;
        }
        
        recipeData = recipe;
        onSelectedCallback = onSelected;
        
        // Set up UI
        UpdateDisplay();
        
        // Set up button
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnSelected);
        }
    }
    
    public void UpdateDisplay()
    {
        if (recipeData == null) return;
        
        // Set icon
        if (iconImage != null && recipeData.resultIcon != null)
        {
            iconImage.sprite = recipeData.resultIcon;
        }
        
        // Set name
        if (nameText != null)
        {
            nameText.text = recipeData.recipeName;
        }
        
        // Set level requirement
        if (levelText != null)
        {
            levelText.text = $"Lv.{recipeData.requiredPlayerLevel}";
        }
        
        // Update availability based on player level and ingredients
        UpdateAvailability();
    }
    
    private void UpdateAvailability()
    {
        if (recipeData == null) return;
        
        bool canCraft = recipeData.CanCraft(GameManager.Instance.playerData);
        
        if (backgroundImage != null)
        {
            backgroundImage.color = canCraft ? availableColor : unavailableColor;
        }
        
        // Update button interactability
        if (selectButton != null)
        {
            selectButton.interactable = canCraft;
        }
    }
    
    private void OnSelected()
    {
        if (recipeData != null && onSelectedCallback != null)
        {
            onSelectedCallback(recipeData);
            SetSelected(true);
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (backgroundImage != null)
        {
            backgroundImage.color = selected ? selectedColor : 
                (recipeData.CanCraft(GameManager.Instance.playerData) ? availableColor : unavailableColor);
        }
    }
    
    public void Refresh()
    {
        UpdateDisplay();
    }
}