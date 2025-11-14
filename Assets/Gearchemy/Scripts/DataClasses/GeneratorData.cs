using UnityEngine;

[CreateAssetMenu(fileName = "NewGeneratorData", menuName = "Gearchemy/Generator Data")]
public class GeneratorData : ScriptableObject
{
    [Header("Generator Info")]
    public string generatorName;
    public string description;
    public Sprite icon;
    public GameObject prefab;
    
    [Header("Generation Settings")]
    public ElementData generatedElement;
    public float baseGenerationTime = 30f;
    public int baseEnergyCost = 5;
    public int maxLevel = 5;
    
    [Header("Upgrade Settings")]
    public AnimationCurve generationTimeCurve = AnimationCurve.Linear(0, 1, 1, 0.5f);
    public AnimationCurve energyCostCurve = AnimationCurve.Linear(0, 1, 1, 0.7f);
    public int upgradeCostMultiplier = 100;
    
    [Header("Visual Settings")]
    public Color primaryColor = Color.cyan;
    public ParticleSystem generationEffect;
    public AudioClip generationSound;
    public AudioClip upgradeSound;
    
    public float GetGenerationTime(int level)
    {
        return baseGenerationTime * generationTimeCurve.Evaluate((level - 1) / (float)(maxLevel - 1));
    }
    
    public int GetEnergyCost(int level)
    {
        return Mathf.RoundToInt(baseEnergyCost * energyCostCurve.Evaluate((level - 1) / (float)(maxLevel - 1)));
    }
    
    public int GetUpgradeCost(int currentLevel)
    {
        return upgradeCostMultiplier * (currentLevel + 1);
    }
}