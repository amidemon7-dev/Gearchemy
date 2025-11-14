using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CraftingUI : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject craftingPanel;
    public GameObject recipeListContainer;
    public GameObject recipeItemPrefab;
    public GameObject recipeDetailPanel;
    public GameObject craftingProgressPanel;
    
    [Header("Recipe Display")]
    public TextMeshProUGUI recipeNameText;
    public TextMeshProUGUI recipeDescriptionText;
    public Image recipeIcon;
    public Transform ingredientsContainer;
    public GameObject ingredientPrefab;
    public TextMeshProUGUI successRateText;
    public Slider successRateSlider;
    
    [Header("Crafting Controls")]
    public Button craftButton;
    public Button closeButton;
    public TMP_Dropdown qualityDropdown;
    
    [Header("Progress Display")]
    public Slider craftingProgressSlider;
    public TextMeshProUGUI craftingProgressText;
    public Button cancelCraftingButton;
    
    private CraftingManager craftingManager;
    private RecipeData selectedRecipe;
    private List<GameObject> recipeItems = new List<GameObject>();
    private List<GameObject> ingredientItems = new List<GameObject>();
    
    public void Initialize(CraftingManager manager)
    {
        craftingManager = manager;
        
        // Set up button listeners
        craftButton?.onClick.AddListener(OnCraftButtonClicked);
        closeButton?.onClick.AddListener(OnCloseButtonClicked);
        cancelCraftingButton?.onClick.AddListener(OnCancelCraftingClicked);
        
        // Set up quality dropdown
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string> { "Normal", "Good", "Excellent", "Perfect" });
        }
        
        // Hide panels initially
        craftingPanel.SetActive(false);
        recipeDetailPanel.SetActive(false);
        craftingProgressPanel.SetActive(false);
        
        // Load available recipes
        LoadAvailableRecipes();
    }
    
    public void Show()
    {
        craftingPanel.SetActive(true);
        LoadAvailableRecipes();
        
        // Refresh UI
        if (selectedRecipe != null)
        {
            ShowRecipeDetails(selectedRecipe);
        }
    }
    
    public void Hide()
    {
        craftingPanel.SetActive(false);
        recipeDetailPanel.SetActive(false);
        craftingProgressPanel.SetActive(false);
    }
    
    private void LoadAvailableRecipes()
    {
        // Clear existing recipe items
        foreach (var item in recipeItems)
        {
            Destroy(item);
        }
        recipeItems.Clear();
        
        // Get available recipes
        var availableRecipes = craftingManager.GetAvailableRecipes(GameManager.Instance.playerData);
        
        // Create recipe list items
        foreach (var recipe in availableRecipes)
        {
            GameObject recipeItem = Instantiate(recipeItemPrefab, recipeListContainer.transform);
            RecipeListItem listItem = recipeItem.GetComponent<RecipeListItem>();
            
            if (listItem != null)
            {
                listItem.Initialize(recipe, OnRecipeSelected);
            }
            
            recipeItems.Add(recipeItem);
        }
    }
    
    private void OnRecipeSelected(RecipeData recipe)
    {
        selectedRecipe = recipe;
        ShowRecipeDetails(recipe);
    }
    
    private void ShowRecipeDetails(RecipeData recipe)
    {
        recipeDetailPanel.SetActive(true);
        
        // Update recipe info
        if (recipeNameText != null)
            recipeNameText.text = recipe.recipeName;
        
        if (recipeDescriptionText != null)
            recipeDescriptionText.text = recipe.description;
        
        if (recipeIcon != null && recipe.resultIcon != null)
            recipeIcon.sprite = recipe.resultIcon;
        
        // Show ingredients
        ShowIngredients(recipe);
        
        // Update success rate
        UpdateSuccessRate(recipe);
        
        // Update craft button
        UpdateCraftButton(recipe);
    }
    
    private void ShowIngredients(RecipeData recipe)
    {
        // Clear existing ingredient items
        foreach (var item in ingredientItems)
        {
            Destroy(item);
        }
        ingredientItems.Clear();
        
        // Create ingredient items
        foreach (var ingredient in recipe.ingredients)
        {
            GameObject ingredientItem = Instantiate(ingredientPrefab, ingredientsContainer);
            IngredientDisplay ingredientDisplay = ingredientItem.GetComponent<IngredientDisplay>();
            
            if (ingredientDisplay != null)
            {
                ingredientDisplay.Initialize(ingredient);
            }
            
            ingredientItems.Add(ingredientItem);
        }
    }
    
    private void UpdateSuccessRate(RecipeData recipe)
    {
        int successRate = recipe.CalculateSuccessRate(GameManager.Instance.playerData);
        
        if (successRateText != null)
            successRateText.text = $"Success Rate: {successRate}%";
        
        if (successRateSlider != null)
        {
            successRateSlider.value = successRate;
            successRateSlider.maxValue = 100;
        }
    }
    
    private void UpdateCraftButton(RecipeData recipe)
    {
        bool canCraft = craftingManager.CanCraftRecipe(recipe, GameManager.Instance.playerData);
        
        if (craftButton != null)
        {
            craftButton.interactable = canCraft;
            
            var buttonText = craftButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = canCraft ? "Craft" : "Insufficient Materials";
            }
        }
    }
    
    private void OnCraftButtonClicked()
    {
        if (selectedRecipe != null)
        {
            RecipeData.Quality selectedQuality = (RecipeData.Quality)qualityDropdown.value;
            craftingManager.OnCraftingStarted(selectedRecipe, selectedQuality);
        }
    }
    
    private void OnCloseButtonClicked()
    {
        Hide();
        GameManager.Instance.ChangeState(GameManager.GameState.Playing);
    }
    
    private void OnCancelCraftingClicked()
    {
        // Cancel crafting process
        craftingProgressPanel.SetActive(false);
        recipeDetailPanel.SetActive(true);
    }
    
    public void ShowCraftingProgress(RecipeData recipe, float duration)
    {
        recipeDetailPanel.SetActive(false);
        craftingProgressPanel.SetActive(true);
        
        if (craftingProgressSlider != null)
        {
            craftingProgressSlider.maxValue = duration;
            craftingProgressSlider.value = 0;
        }
        
        if (craftingProgressText != null)
        {
            craftingProgressText.text = $"Crafting {recipe.recipeName}...";
        }
        
        StartCoroutine(UpdateCraftingProgress(duration));
    }
    
    public void HideCraftingProgress()
    {
        craftingProgressPanel.SetActive(false);
        recipeDetailPanel.SetActive(true);
        
        StopAllCoroutines();
    }
    
    private System.Collections.IEnumerator UpdateCraftingProgress(float duration)
    {
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            if (craftingProgressSlider != null)
            {
                craftingProgressSlider.value = elapsed;
            }
            
            if (craftingProgressText != null)
            {
                float progress = elapsed / duration;
                craftingProgressText.text = $"Crafting... {Mathf.RoundToInt(progress * 100)}%";
            }
            
            yield return null;
        }
        
        // Crafting complete
        craftingProgressText.text = "Crafting Complete!";
        
        yield return new WaitForSeconds(1f);
        
        HideCraftingProgress();
    }
    
    public void RefreshUI()
    {
        if (selectedRecipe != null)
        {
            ShowRecipeDetails(selectedRecipe);
        }
        
        LoadAvailableRecipes();
    }
}