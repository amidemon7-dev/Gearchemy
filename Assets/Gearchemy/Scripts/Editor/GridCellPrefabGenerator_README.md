# GridCell Prefab Generator

## Описание
Генератор префабов GridCell для Unity проекта Gearchemy. Создает полнофункциональные ячейки сетки с необходимыми компонентами и настройками.

## Возможности

### Базовый генератор (`GridCellPrefabGenerator.cs`)
- Создание префаба GridCell с минимальными настройками
- Автоматическое добавление необходимых компонентов
- Создание placeholder спрайтов при отсутствии кастомных
- Меню в Unity Editor: `Gearchemy/Generate GridCell Prefab`

### Расширенный генератор (`GridCellPrefabGeneratorAdvanced.cs`)
- Полноценный редакторский окно с настройками
- Кастомизация визуальных параметров
- Добавление дополнительных компонентов (частицы, аудио)
- Гибкая настройка цветов и спрайтов
- Меню в Unity Editor: `Gearchemy/Advanced GridCell Generator`

## Использование

### Быстрый старт (Базовый генератор)
1. Откройте Unity Editor
2. Перейдите в меню: `Gearchemy/Generate GridCell Prefab`
3. Префаб будет создан по пути: `Assets/Prefabs/GridCell.prefab`

### Расширенная настройка
1. Откройте окно генератора: `Gearchemy/Advanced GridCell Generator`
2. Настройте параметры:
   - **Basic Settings**: имя и путь префаба
   - **Visual Settings**: спрайты и цвета
   - **Component Settings**: размер, слой, дополнительные компоненты
3. Нажмите "Generate GridCell Prefab"

## Компоненты префаба

### Обязательные компоненты
- **GameObject**: основной объект
- **SpriteRenderer**: визуальное отображение
- **BoxCollider2D**: коллайдер для взаимодействия
- **GridCell**: основной скрипт ячейки
- **EventTrigger**: обработка UI событий

### Дополнительные компоненты (опционально)
- **ParticleSystem**: визуальные эффекты
- **AudioSource**: звуковые эффекты

## Настройки GridCell

### Визуальные параметры
```csharp
// Цвета состояний
emptyColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);     // Пустая ячейка
occupiedColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);   // Занятая ячейка
highlightColor = new Color(1f, 1f, 0f, 0.7f);        // Подсветка
mergeHighlightColor = new Color(0f, 1f, 0f, 0.7f);   // Подсветка слияния
```

### Спрайты
- **emptySprite**: спрайт для пустой ячейки
- **occupiedSprite**: спрайт для занятой ячейки
- **cellRenderer**: ссылка на SpriteRenderer

## Структура файлов

```
Assets/
├── Prefabs/
│   └── GridCell.prefab              # Сгенерированный префаб
├── Sprites/
│   ├── CellEmpty.png                # Спрайт пустой ячейки
│   ├── CellOccupied.png             # Спрайт занятой ячейки
│   ├── Generated/                   # Сгенерированные спрайты
│   └── Placeholder/                 # Placeholder спрайты
└── Scripts/
    └── Editor/
        ├── GridCellPrefabGenerator.cs
        ├── GridCellPrefabGeneratorAdvanced.cs
        └── GridCellPrefabGenerator_README.md
```

## Интеграция с GridManager

После генерации префаба необходимо назначить его в GridManager:

```csharp
// В Inspector окне GridManager
public GameObject cellPrefab;  // Назначить сгенерированный префаб
```

## API GridCell

### Основные методы
```csharp
// Инициализация ячейки
public void Initialize(int x, int y)

// Работа с элементами
public bool IsEmpty()
public bool HasElement()
public GameElement GetElement()
public void SetElement(GameElement element)
public void RemoveElement()

// Подсветка
public void Highlight(bool isMergeable = false)
public void RemoveHighlight()

// Проверка возможности размещения
public bool CanAcceptElement(GameElement element)
```

### События
```csharp
public System.Action<GridCell> OnCellClicked;
public System.Action<GameElement> OnElementDropped;
```

## Примеры использования

### Создание сетки
```csharp
GridManager gridManager = FindObjectOfType<GridManager>();
gridManager.cellPrefab = gridCellPrefab;  // Назначить префаб
gridManager.InitializeGrid(6, 6);         // Создать сетку 6x6
```

### Обработка кликов по ячейкам
```csharp
gridManager.OnCellClicked += (cell) => {
    Debug.Log($"Cell clicked: {cell.gridX}, {cell.gridY}");
};
```

## Решение проблем

### Префаб не создается
- Проверьте права доступа к папке Assets
- Убедитесь, что путь существует
- Проверьте консоль на наличие ошибок

### Спрайты не отображаются
- Проверьте настройки SpriteRenderer
- Убедитесь, что спрайты импортированы как Sprite
- Проверьте sorting order и слои

### Взаимодействие не работает
- Проверьте наличие BoxCollider2D
- Убедитесь, что слой настроен корректно
- Проверьте EventTrigger компонент

## Расширение функциональности

### Добавление кастомных эффектов
```csharp
// Добавить компонент к префабу
GameObject gridCellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GridCell.prefab");
gridCellPrefab.AddComponent<YourCustomComponent>();
```

### Изменение визуального стиля
1. Откройте расширенный генератор
2. Измените цвета и спрайты
3. Сгенерируйте новый префаб

## Поддержка
При возникновении проблем или вопросов обращайтесь к документации Unity или создайте issue в репозитории проекта.