using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientDisplay : MonoBehaviour
{
    [Header("UI Components")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI quantityText;
    public Image backgroundImage;
    
    [Header("Colors")]
    public Color hasIngredientColor = Color.green;
    public Color missingIngredientColor = Color.red;
    
    public void Initialize(RecipeData.Ingredient ingredient)
    {
        if (ingredient == null || ingredient.element == null)
        {
            Debug.LogWarning("Invalid ingredient data");
            return;
        }
        
        // Set icon
        if (iconImage != null && ingredient.element.icon != null)
        {
            iconImage.sprite = ingredient.element.icon;
        }
        
        // Set name
        if (nameText != null)
        {
            nameText.text = ingredient.element.elementName;
        }
        
        // Set quantity and check if player has enough
        UpdateQuantityDisplay(ingredient);
    }
    
    public void UpdateQuantityDisplay(RecipeData.Ingredient ingredient)
    {
        if (ingredient == null || ingredient.element == null) return;
        
        // Check if player has enough of this ingredient
        bool hasEnough = GameManager.Instance.playerData.HasElement(ingredient.element, ingredient.quantity);
        int playerQuantity = GetPlayerQuantity(ingredient.element);
        
        // Update quantity text
        if (quantityText != null)
        {
            quantityText.text = $"{playerQuantity}/{ingredient.quantity}";
        }
        
        // Update background color based on availability
        if (backgroundImage != null)
        {
            backgroundImage.color = hasEnough ? hasIngredientColor : missingIngredientColor;
        }
    }
    
    private int GetPlayerQuantity(ElementData element)
    {
        if (element == null) return 0;
        
        var item = GameManager.Instance.playerData.inventory.Find(i => i.elementId == element.name && i.level == element.level);
        return item != null ? item.quantity : 0;
    }
    
    public void RefreshDisplay()
    {
        // This can be called when inventory changes to update the display
        // The specific ingredient should be stored and refreshed
    }
}