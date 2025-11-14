using UnityEngine;
using System.Collections.Generic;

public class CraftingManager : MonoBehaviour
{
    [Header("Crafting Settings")]
    public List<RecipeData> allRecipes = new List<RecipeData>();
    public List<CraftingStationData> craftingStations = new List<CraftingStationData>();
    
    [Header("Crafting UI")]
    public GameObject craftingUIPrefab;
    public Transform craftingUIParent;
    
    private CraftingUI currentCraftingUI;
    private Dictionary<string, RecipeData> recipeDictionary = new Dictionary<string, RecipeData>();
    
    public void Initialize()
    {
        // Initialize recipe dictionary
        foreach (var recipe in allRecipes)
        {
            recipeDictionary[recipe.recipeName] = recipe;
        }
        
        // Initialize crafting UI
        if (craftingUIPrefab != null && craftingUIParent != null)
        {
            GameObject uiObject = Instantiate(craftingUIPrefab, craftingUIParent);
            currentCraftingUI = uiObject.GetComponent<CraftingUI>();
            if (currentCraftingUI != null)
            {
                currentCraftingUI.Initialize(this);
            }
        }
    }
    
    public bool CanCraftRecipe(RecipeData recipe, PlayerData playerData)
    {
        // Check player level
        if (playerData.playerLevel < recipe.requiredPlayerLevel)
            return false;
        
        // Check ingredients
        foreach (var ingredient in recipe.ingredients)
        {
            if (!playerData.HasElement(ingredient.element, ingredient.quantity))
                return false;
        }
        
        // Check if recipe is unlocked
        if (!playerData.unlockedRecipes.Contains(recipe.recipeName))
            return false;
        
        return true;
    }
    
    public bool TryCraftRecipe(RecipeData recipe, RecipeData.Quality desiredQuality = RecipeData.Quality.Normal)
    {
        var playerData = GameManager.Instance.playerData;
        
        if (!CanCraftRecipe(recipe, playerData))
            return false;
        
        // Calculate success rate
        int successRate = recipe.CalculateSuccessRate(playerData);
        
        // Apply quality modifier
        switch (desiredQuality)
        {
            case RecipeData.Quality.Good:
                successRate = Mathf.RoundToInt(successRate * 0.9f);
                break;
            case RecipeData.Quality.Excellent:
                successRate = Mathf.RoundToInt(successRate * 0.8f);
                break;
            case RecipeData.Quality.Perfect:
                successRate = Mathf.RoundToInt(successRate * 0.7f);
                break;
        }
        
        // Check for success
        bool success = Random.Range(0, 100) < successRate;
        
        if (success)
        {
            // Consume ingredients
            foreach (var ingredient in recipe.ingredients)
            {
                playerData.RemoveElement(ingredient.element, ingredient.quantity);
            }
            
            // Create result
            CreateCraftingResult(recipe, desiredQuality);
            
            // Award XP
            GameManager.Instance.playerData.AddXP(recipe.craftingTime > 0 ? 
                Mathf.RoundToInt(recipe.craftingTime * 2) : 10);
            
            // Track crafting progress
            var progressionManager = FindObjectOfType<ProgressionManager>();
            progressionManager?.TrackProgress("crafts");
            
            Debug.Log($"Successfully crafted {recipe.recipeName} with {desiredQuality} quality!");
        }
        else
        {
            Debug.Log($"Failed to craft {recipe.recipeName}");
            // Could implement partial ingredient loss on failure
        }
        
        return success;
    }
    
    private void CreateCraftingResult(RecipeData recipe, RecipeData.Quality quality)
    {
        int quantity = recipe.resultQuantity;
        
        // Apply quality bonus
        switch (quality)
        {
            case RecipeData.Quality.Good:
                quantity = Mathf.RoundToInt(quantity * 1.1f);
                break;
            case RecipeData.Quality.Excellent:
                quantity = Mathf.RoundToInt(quantity * 1.2f);
                break;
            case RecipeData.Quality.Perfect:
                quantity = Mathf.RoundToInt(quantity * 1.5f);
                break;
        }
        
        // Add to inventory
        GameManager.Instance.playerData.AddElement(recipe.resultElement, quantity);
        
        // Create visual effect
        CreateCraftingEffect(recipe.resultElement);
    }
    
    private void CreateCraftingEffect(ElementData resultElement)
    {
        // This would create a visual effect when crafting is successful
        // Could include particles, sound, etc.
        Debug.Log($"Crafting effect for {resultElement.elementName}");
    }
    
    public List<RecipeData> GetAvailableRecipes(PlayerData playerData)
    {
        List<RecipeData> availableRecipes = new List<RecipeData>();
        
        foreach (var recipe in allRecipes)
        {
            if (playerData.unlockedRecipes.Contains(recipe.recipeName) &&
                playerData.playerLevel >= recipe.requiredPlayerLevel)
            {
                availableRecipes.Add(recipe);
            }
        }
        
        return availableRecipes;
    }
    
    public List<RecipeData> GetCraftableRecipes(PlayerData playerData)
    {
        List<RecipeData> craftableRecipes = new List<RecipeData>();
        
        foreach (var recipe in allRecipes)
        {
            if (CanCraftRecipe(recipe, playerData))
            {
                craftableRecipes.Add(recipe);
            }
        }
        
        return craftableRecipes;
    }
    
    public RecipeData GetRecipe(string recipeName)
    {
        return recipeDictionary.ContainsKey(recipeName) ? recipeDictionary[recipeName] : null;
    }
    
    public void UnlockRecipe(string recipeName)
    {
        if (!GameManager.Instance.playerData.unlockedRecipes.Contains(recipeName))
        {
            GameManager.Instance.playerData.unlockedRecipes.Add(recipeName);
            Debug.Log($"Recipe unlocked: {recipeName}");
        }
    }
    
    public void UnlockRecipe(RecipeData recipe)
    {
        UnlockRecipe(recipe.recipeName);
    }
    
    public void ShowCraftingUI()
    {
        if (currentCraftingUI != null)
        {
            currentCraftingUI.Show();
        }
    }
    
    public void HideCraftingUI()
    {
        if (currentCraftingUI != null)
        {
            currentCraftingUI.Hide();
        }
    }
    
    public void OnRecipeSelected(RecipeData recipe)
    {
        // Handle recipe selection in UI
        Debug.Log($"Selected recipe: {recipe.recipeName}");
    }
    
    public void OnCraftingStarted(RecipeData recipe, RecipeData.Quality desiredQuality = RecipeData.Quality.Normal)
    {
        // Start crafting process
        StartCoroutine(CraftingCoroutine(recipe, desiredQuality));
    }
    
    private System.Collections.IEnumerator CraftingCoroutine(RecipeData recipe, RecipeData.Quality desiredQuality = RecipeData.Quality.Normal)
    {
        float craftingTime = recipe.craftingTime;
        
        // Show crafting progress
        if (currentCraftingUI != null)
        {
            currentCraftingUI.ShowCraftingProgress(recipe, craftingTime);
        }
        
        // Wait for crafting time
        yield return new WaitForSeconds(craftingTime);
        
        // Complete crafting
        TryCraftRecipe(recipe, desiredQuality);
        
        // Hide progress
        if (currentCraftingUI != null)
        {
            currentCraftingUI.HideCraftingProgress();
        }
    }
    
    public void SolvePuzzle(RecipeData recipe)
    {
        // This would handle the mathematical puzzle mini-game
        // For now, just add a bonus to success rate
        int bonus = 15; // 15% bonus for solving puzzle
        
        // This bonus would be applied in the CanCraftRecipe method
        Debug.Log($"Puzzle solved for {recipe.recipeName}! +{bonus}% success rate");
    }
}