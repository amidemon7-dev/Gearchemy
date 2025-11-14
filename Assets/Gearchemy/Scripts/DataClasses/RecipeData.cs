using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRecipeData", menuName = "Gearchemy/Recipe Data")]
public class RecipeData : ScriptableObject
{
    [System.Serializable]
    public class Ingredient
    {
        public ElementData element;
        public int quantity;
    }
    
    [Header("Recipe Info")]
    public string recipeName;
    public string description;
    public Sprite resultIcon;
    
    [Header("Ingredients")]
    public List<Ingredient> ingredients = new List<Ingredient>();
    
    [Header("Result")]
    public ElementData resultElement;
    public int resultQuantity = 1;
    public int baseSuccessRate = 100; // Percentage
    
    [Header("Crafting Settings")]
    public float craftingTime = 5f;
    public CraftingStation requiredStation;
    public int requiredPlayerLevel = 1;
    
    [Header("Puzzle Settings")]
    public PuzzleType requiredPuzzle;
    public int puzzleDifficulty = 1;
    
    public enum CraftingStation
    {
        Cauldron,
        TransmutationPress,
        Enchanter,
        EthericMatrix
    }
    
    public enum PuzzleType
    {
        None,
        LogicSorting,
        ProportionalEquations,
        SpatialRouting,
        FibonacciSequences
    }
    
    public enum Quality
    {
        Normal,
        Good,
        Excellent,
        Perfect
    }
    
    public bool CanCraft(PlayerData playerData)
    {
        if (playerData.playerLevel < requiredPlayerLevel)
            return false;
            
        foreach (var ingredient in ingredients)
        {
            if (!playerData.HasElement(ingredient.element, ingredient.quantity))
                return false;
        }
        
        return true;
    }
    
    public int CalculateSuccessRate(PlayerData playerData)
    {
        int rate = baseSuccessRate;
        
        // Add bonuses based on player level and achievements
        rate += (playerData.playerLevel - requiredPlayerLevel) * 2;
        
        // Apply puzzle completion bonus
        if (requiredPuzzle != PuzzleType.None)
        {
            rate += 15; // Base puzzle bonus
        }
        
        return Mathf.Clamp(rate, 0, 100);
    }
}