using UnityEngine;
using System.Collections;

public class GeneratorElement : GameElement
{
    [Header("Generator Components")]
    public ParticleSystem generationEffect;
    public AudioSource generatorAudioSource;
    
    private float lastGenerationTime;
    private int currentLevel = 1;
    private bool isGenerating = false;
    private GeneratorData generatorData; // Store original GeneratorData
    
    public System.Action<GeneratorElement> OnGenerated;
    
    private void Start()
    {
        if (generatorAudioSource == null)
            generatorAudioSource = GetComponent<AudioSource>();
    }
    
    public void Initialize(GeneratorData data, int level = 1)
    {
        generatorData = data; // Store the original GeneratorData
        // Create a temporary ElementData from GeneratorData for base initialization
        ElementData elementData = CreateElementDataFromGeneratorData(data);
        base.Initialize(elementData);
        currentLevel = level;
        lastGenerationTime = Time.time;
    }
    
    private ElementData CreateElementDataFromGeneratorData(GeneratorData generatorData)
    {
        // Create a new ElementData instance with generator properties
        ElementData elementData = ScriptableObject.CreateInstance<ElementData>();
        
        // Copy basic properties from GeneratorData
        elementData.elementName = generatorData.generatorName;
        elementData.description = generatorData.description;
        elementData.icon = generatorData.icon;
        elementData.prefab = generatorData.prefab;
        
        // Set generator-specific properties
        elementData.elementType = ElementData.ElementType.Generator;
        elementData.level = 1; // Base level for generators
        elementData.baseValue = 0; // Generators don't have sell value
        elementData.rarity = ElementData.Rarity.Rare; // Default rarity for generators
        
        // Set merge properties
        elementData.canMerge = false; // Generators typically can't be merged
        elementData.nextLevelElement = null;
        
        // Set visual properties
        elementData.primaryColor = generatorData.primaryColor;
        elementData.secondaryColor = Color.gray;
        
        // Set generator properties
        elementData.isGenerator = true;
        elementData.generationTime = generatorData.baseGenerationTime;
        elementData.energyCost = generatorData.baseEnergyCost;
        elementData.generatedElement = generatorData.generatedElement;
        
        return elementData;
    }
    
    private void Update()
    {
        // Auto-generation timer
        if (elementData.isGenerator && !isGenerating)
        {
            float generationTime = generatorData.GetGenerationTime(currentLevel);
            
            if (Time.time - lastGenerationTime >= generationTime)
            {
                TryGenerate();
            }
        }
    }
    
    public bool TryGenerate()
    {
        if (!elementData.isGenerator || isGenerating)
            return false;
        
        // Check energy cost
        int energyCost = generatorData.GetEnergyCost(currentLevel);
        if (GameManager.Instance.playerData.energy < energyCost)
        {
            // Not enough energy
            return false;
        }
        
        // Find empty cell
        GridManager gridManager = FindObjectOfType<GridManager>();
        GridCell emptyCell = gridManager?.GetRandomEmptyCell();
        
        if (emptyCell == null)
        {
            // No empty space
            return false;
        }
        
        // Consume energy
        GameManager.Instance.playerData.energy -= energyCost;
        
        // Start generation
        StartCoroutine(GenerateElement(emptyCell));
        
        return true;
    }
    
    private IEnumerator GenerateElement(GridCell targetCell)
    {
        isGenerating = true;
        
        ElementData elementToGenerate = generatorData.generatedElement;
        
        // Play generation effects
        if (generationEffect != null)
        {
            generationEffect.Play();
        }
        
        if (generatorAudioSource != null && generatorData.generationSound != null)
        {
            generatorAudioSource.PlayOneShot(generatorData.generationSound);
        }
        
        // Wait for generation time
        float generationTime = generatorData.GetGenerationTime(currentLevel);
        yield return new WaitForSeconds(generationTime);
        
        // Create new element
        if (elementToGenerate != null && elementToGenerate.prefab != null)
        {
            GameObject newElementObj = Instantiate(elementToGenerate.prefab, targetCell.transform.position, Quaternion.identity);
            GameElement newElement = newElementObj.GetComponent<GameElement>();
            
            if (newElement != null)
            {
                newElement.Initialize(elementToGenerate);
                targetCell.SetElement(newElement);
                
                // Notify systems
                OnGenerated?.Invoke(this);
                
                // Add XP for generation
                GameManager.Instance.playerData.AddXP(5);
            }
        }
        
        lastGenerationTime = Time.time;
        isGenerating = false;
    }
    
    public bool CanUpgrade()
    {
        int upgradeCost = generatorData.GetUpgradeCost(currentLevel);
        
        return currentLevel < generatorData.maxLevel && 
               GameManager.Instance.playerData.coins >= upgradeCost;
    }
    
    public bool TryUpgrade()
    {
        if (!CanUpgrade())
            return false;
        
        int upgradeCost = generatorData.GetUpgradeCost(currentLevel);
        
        // Deduct coins
        GameManager.Instance.playerData.coins -= upgradeCost;
        
        // Upgrade
        currentLevel++;
        
        // Play upgrade effects
        if (generatorAudioSource != null && generatorData.upgradeSound != null)
        {
            generatorAudioSource.PlayOneShot(generatorData.upgradeSound);
        }
        
        // Update visual representation
        UpdateVisuals();
        
        return true;
    }
    
    private void UpdateVisuals()
    {
        // Update generator appearance based on level
        // This could include changing colors, adding particles, etc.
        if (elementRenderer != null)
        {
            Color levelColor = elementData.primaryColor;
            levelColor *= (1f + currentLevel * 0.1f);
            elementRenderer.color = levelColor;
        }
    }
    
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    public float GetGenerationProgress()
    {
        if (!elementData.isGenerator)
            return 0f;
            
        float generationTime = generatorData.GetGenerationTime(currentLevel);
        float elapsed = Time.time - lastGenerationTime;
        
        return Mathf.Clamp01(elapsed / generationTime);
    }
}