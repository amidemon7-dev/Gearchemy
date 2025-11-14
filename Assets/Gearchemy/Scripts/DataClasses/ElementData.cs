using UnityEngine;

[CreateAssetMenu(fileName = "NewElementData", menuName = "Gearchemy/Element Data")]
public class ElementData : ScriptableObject
{
    [Header("Basic Info")]
    public string elementName;
    public string description;
    public Sprite icon;
    public GameObject prefab;
    
    [Header("Element Properties")]
    public ElementType elementType;
    public int level;
    public int baseValue;
    public Rarity rarity;
    
    [Header("Merge Properties")]
    public ElementData nextLevelElement;
    public int mergeRequirement = 2; // Usually 2 elements for merge
    public bool canMerge = true;
    
    [Header("Visual Properties")]
    public Color primaryColor = Color.white;
    public Color secondaryColor = Color.gray;
    public ParticleSystem mergeEffect;
    public AudioClip mergeSound;
    
    [Header("Generator Properties")]
    public bool isGenerator = false;
    public float generationTime = 30f;
    public int energyCost = 5;
    public ElementData generatedElement;
    
    public enum ElementType
    {
        Berry,
        Mushroom, 
        Crystal,
        Steam,
        Tool,
        Potion,
        Artifact,
        Generator
    }
    
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
    
    public int GetSellValue()
    {
        return baseValue * (level + 1);
    }
    
    public bool CanMergeWith(ElementData other)
    {
        return canMerge && 
               elementType == other.elementType && 
               level == other.level &&
               nextLevelElement != null;
    }
}