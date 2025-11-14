# Gearchemy: Core Unity Architecture

## Core Game Systems

### 1. Merge System (Core Loop)
- **Grid Management**: 6x6 grid with expansion capability
- **Drag & Drop**: Touch-based element merging
- **Merge Rules**: 2 identical elements → 1 higher level element
- **Chain Reactions**: Auto-merging when new pairs are formed
- **Element Types**: 4 main series (Berries, Mushrooms, Crystals, Steam) with 10 levels each

### 2. Generation System
- **Generators**: 4 types (Berry Bush, Mushroom Stump, Crystal Vein, Steam Boiler)
- **Generation Logic**: Double-tap to generate elements on random free cells
- **Energy System**: Generators consume energy for instant generation
- **Upgrade Path**: Merge generators to improve efficiency

### 3. Economy System
- **Currencies**: Coins, Ether Gems, Essence
- **Income Sources**: Element sales, quest completion, level progression
- **Expenses**: Generator purchases, inventory expansion, upgrades
- **Pricing**: Scaled by element level and rarity

### 4. Progression System
- **Player Levels**: XP-based progression (10 XP per merge, 20-200 XP per quest)
- **Inventory Management**: Expandable storage system
- **Achievements**: Milestone-based rewards
- **Unlock System**: New generators and recipes by level

### 5. Narrative System
- **NPC Management**: 4 character types with unique personalities
- **Quest System**: Dynamic quest generation based on NPC relationships
- **Dialogue System**: Branching conversations with relationship impact
- **Relationship Tracking**: Reputation and trust metrics

### 6. Crafting System
- **Recipe Management**: Complex multi-ingredient recipes
- **Crafting Stations**: Cauldron, Transmutation Press, Enchanter, Etheric Matrix
- **Mathematical Puzzles**: Mini-games that enhance crafting results
- **Quality Tiers**: Normal, Good, Excellent, Perfect outcomes

## Technical Architecture

### Design Patterns Used:
1. **Observer Pattern**: For event-driven communication between systems
2. **Factory Pattern**: For element and generator creation
3. **State Pattern**: For game states and NPC behaviors
4. **Singleton Pattern**: For game managers and data persistence
5. **Command Pattern**: For user inputs and undo functionality

### Core Classes Structure:
- **GameManager**: Central game state controller
- **GridManager**: Handles grid logic and cell management
- **ElementSystem**: Manages all game elements and their behaviors
- **MergeHandler**: Processes merge logic and validations
- **GenerationController**: Manages generator behavior and output
- **EconomyManager**: Handles all currency and transaction logic
- **ProgressionManager**: Tracks player advancement and unlocks
- **NarrativeManager**: Controls NPCs, quests, and dialogue
- **CraftingManager**: Manages recipes and crafting processes
- **UIManager**: Handles all UI interactions and updates
- **SaveSystem**: Manages game data persistence

### Data Structures:
- **ElementData**: ScriptableObject for element properties
- **GeneratorData**: ScriptableObject for generator configurations
- **RecipeData**: ScriptableObject for crafting recipes
- **QuestData**: ScriptableObject for quest definitions
- **NPCData**: ScriptableObject for character information
- **PlayerData**: Serializable class for save game data

## Implementation Priority:
1. **Core Merge System** (Grid, Elements, Drag & Drop)
2. **Generation System** (Generators, Element creation)
3. **Basic Economy** (Coins, simple transactions)
4. **Progression System** (XP, levels, basic achievements)
5. **UI Framework** (Main game interface)
6. **Narrative System** (NPCs, basic quests)
7. **Crafting System** (Advanced recipes and puzzles)
8. **Save System** (Data persistence)
9. **Advanced Features** (Chain reactions, special combinations)