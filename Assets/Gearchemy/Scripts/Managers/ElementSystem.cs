using UnityEngine;
using System.Collections.Generic;

public class ElementSystem : MonoBehaviour
{
    [Header("Element Database")]
    public List<ElementData> allElements = new List<ElementData>();
    public List<GeneratorData> allGenerators = new List<GeneratorData>();
    
    [Header("Element Pools")]
    public Transform elementPoolContainer;
    public int poolSize = 50;
    
    private Dictionary<string, Queue<GameElement>> elementPools = new Dictionary<string, Queue<GameElement>>();
    
    public void Initialize()
    {
        CreateElementPools();
    }
    
    private void CreateElementPools()
    {
        // Create pools for each element type
        foreach (var elementData in allElements)
        {
            if (elementData.prefab != null)
            {
                CreateElementPool(elementData);
            }
        }
    }
    
    private void CreateElementPool(ElementData elementData)
    {
        Queue<GameElement> pool = new Queue<GameElement>();
        
        for (int i = 0; i < poolSize; i++)
        {
            GameObject elementObj = Instantiate(elementData.prefab, elementPoolContainer);
            GameElement element = elementObj.GetComponent<GameElement>();
            
            if (element != null)
            {
                element.Initialize(elementData);
                element.gameObject.SetActive(false);
                pool.Enqueue(element);
            }
        }
        
        elementPools[elementData.name] = pool;
    }
    
    public GameElement GetElement(ElementData elementData)
    {
        if (elementPools.ContainsKey(elementData.name) && elementPools[elementData.name].Count > 0)
        {
            GameElement element = elementPools[elementData.name].Dequeue();
            element.gameObject.SetActive(true);
            element.Initialize(elementData);
            return element;
        }
        else
        {
            // Create new element if pool is empty
            GameObject elementObj = Instantiate(elementData.prefab);
            GameElement element = elementObj.GetComponent<GameElement>();
            element.Initialize(elementData);
            return element;
        }
    }
    
    public void ReturnElement(GameElement element)
    {
        element.gameObject.SetActive(false);
        element.transform.SetParent(elementPoolContainer);
        
        if (elementPools.ContainsKey(element.elementData.name))
        {
            elementPools[element.elementData.name].Enqueue(element);
        }
    }
    
    public ElementData GetElementData(string elementName)
    {
        return allElements.Find(e => e.elementName == elementName);
    }
    
    public GeneratorData GetGeneratorData(string generatorName)
    {
        return allGenerators.Find(g => g.generatorName == generatorName);
    }
    
    public List<ElementData> GetElementsByType(ElementData.ElementType type)
    {
        return allElements.FindAll(e => e.elementType == type);
    }
    
    public List<ElementData> GetElementsByLevel(int level)
    {
        return allElements.FindAll(e => e.level == level);
    }
    
    public List<ElementData> GetMergeableElements(ElementData element)
    {
        return allElements.FindAll(e => e != element && 
                                       e.elementType == element.elementType && 
                                       e.level == element.level);
    }
    
    public void RegisterElement(ElementData elementData)
    {
        if (!allElements.Contains(elementData))
        {
            allElements.Add(elementData);
        }
    }
    
    public void RegisterGenerator(GeneratorData generatorData)
    {
        if (!allGenerators.Contains(generatorData))
        {
            allGenerators.Add(generatorData);
        }
    }
    
    public bool CanCreateElement(ElementData elementData)
    {
        // Check if we have the required ingredients or generators
        if (elementData.isGenerator)
        {
            return true; // Generators can always be created
        }
        
        // For regular elements, check if we can generate them
        var generators = allGenerators.FindAll(g => g.generatedElement == elementData);
        return generators.Count > 0;
    }
    
    public int GetElementRarity(ElementData elementData)
    {
        // Calculate rarity based on level and type
        int baseRarity = (int)elementData.rarity;
        int levelBonus = elementData.level / 2;
        
        return Mathf.Min(baseRarity + levelBonus, 4); // Max rarity is 4 (Legendary)
    }
    
    public void ClearAllElements()
    {
        // Return all active elements to pool
        GameElement[] activeElements = FindObjectsOfType<GameElement>();
        foreach (var element in activeElements)
        {
            if (element.gameObject.activeInHierarchy)
            {
                ReturnElement(element);
            }
        }
    }
    
    public void OnElementMerged(GameElement mergedElement)
    {
        // Handle element merging logic
        // This could trigger achievements, update statistics, etc.
        
        // Check for chain reactions
        if (GameManager.Instance.enableChainReactions)
        {
            CheckForChainReactions(mergedElement);
        }
    }
    
    private void CheckForChainReactions(GameElement mergedElement)
    {
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null) return;
        
        // Find adjacent cells with same element type
        GridCell mergedCell = mergedElement.GetGridCell();
        if (mergedCell == null) return;
        
        var adjacentCells = gridManager.GetAdjacentCells(mergedCell);
        
        foreach (var adjacentCell in adjacentCells)
        {
            if (adjacentCell.HasElement() && 
                adjacentCell.GetElement().elementData.CanMergeWith(mergedElement.elementData))
            {
                // Trigger automatic merge
                // This would be handled by the merge system
                break;
            }
        }
    }
}