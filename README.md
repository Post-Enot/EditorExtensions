# Описание

Пакет содержит набор утилит для редактора Unity, упрощающих разработку собственных инспекторов. Основная мотивация - идея создать аналог известного и хорошо зарекомендовавшего себя, но во многом устаревшего пакета [Naughty Attributes](https://github.com/dbrizov/NaughtyAttributes). Несмотря на хороший функционал, он полностью написан на IMGUI, что усложняет его использование с UI Toolkit; помимо этого, некоторые архитектурные решения и API по субъективному мнению автора морально устарели и требуют более лаконичной реализации. Это ни в коем случае не попытка принизить изначального автора проекта, а также всех его контрибьюторов, а напротив, дань уважения их заслугам.

Лицензия MIT, проект с полностью открытым исходным кодом, вы можете делать с ним всё, что пожелаете.

Минимальная поддерживаемая версия Unity: 6.3 LTS. Потенциально, пакет должен без проблем работать и на более ранних версиях, однако тестирование не проводилось.

# Атрибуты

Пакет предоставляет множество готовых к использованию атрибутов для настройки отображения полей в инспекторе. Они полностью реализованы с помощью UI Toolkit и совместимы со стандартными атрибутами Unity.

Все атрибуты взаимосочетаемы друг с другом, то есть, одно поле может содержать два и более различных атрибута и будет корректно отображаться.

Ниже подробно описаны все предоставляемые атрибуты, способы и особенности использования.

## FoldoutAttribute, EndFoldoutAttribute

Перемещает поле атрибута, а также все поля ниже в раскрывающийся список. Перемещение происходит до следующего `FoldoutAttribute` или `EndFoldoutAttribute`. На данный момент вложенные раскрывающиеся списки не поддерживаются.

```csharp
[Foldout("<b>Foldout 1")]
[SerializeField] private int var0;
[SerializeField] private int var1;
[SerializeField] private int var2;
[EndFoldout]

[SerializeField] private int var3;

[Foldout("Foldout 2")]
[SerializeField] private int var4;
[SerializeField] private int var5;
[SerializeField] private int var6;

[Foldout("Foldout 2")]
[SerializeField] private int var7;
[SerializeField] private int var8;
[SerializeField] private int var9;
```
![](https://github.com/user-attachments/assets/b4b99ee2-a635-431d-b056-f5953a9a3806)
![](https://github.com/user-attachments/assets/5b6ac336-7ac7-46d1-87be-bd9497d9fe5b)

## WithoutFoldoutAttribute

Убирает раскрывающийся список у представлений классов и структур, помеченных атрибутом `Serializable`.

```csharp
[Serializable]
public struct ItemCostData
{
    [SerializeField] private string itemID;
    [SerializeField] private int cost;

    public ItemCostData(string itemID, int cost)
    {
        this.itemID = itemID;
        this.cost = cost;
    }

    public readonly string ItemID => itemID;
    public readonly int Cost => cost;
}

[SerializeField, WithoutFoldout] private ItemCostData swordCost;
```

![](https://github.com/user-attachments/assets/aacd7b78-b81b-431a-97b7-6bbe7b620614)

При применении к коллекциям, атрибут будет применён ко всем элементам коллекции, что особенно полезно для списков и массивов из значений, являющихся сериализуемыми классами и структурами.

```csharp
[SerializeField, WithoutFoldout, AlternatingRows] private List<ItemCostData> itemCosts = new()
{
    new("bad sword", 10),
    new("default sword", 100),
    new("Excalibur", 999)
};
```

![](https://github.com/user-attachments/assets/c424c07c-42f6-4191-9b75-4b7bbc29cef0)

**Замена Foldout на Header:**

Так как атрибуты сочетаются между собой, вы можете одновременно применить `WithoutFoldout` и `Header` к полю, чтобы заменить раскрывающийся список на заголовок.

```csharp
[SerializeField, WithoutFoldout, Header("Sword Cost")] private ItemCostData swordCost;
```

![](https://github.com/user-attachments/assets/3a34a068-5224-418f-a143-3bb1769992ed)

## SceneAttribute

Превращает стандартное `int` поле в поле ввода индекса сцены: выпадающий список содержит все сцены, включённые в билд, в том числе выключенные. Кнопка рядом с выпадающим списком открывает меню Build Profiles для быстрого и удобного перехода к окну изменения сцен, включённых в билд. На данный момент поддерживается применение атрибута только к полям типа `int`: применение к полям типа `string` на мотив `Naughty Attributes` не поддерживается.

```csharp
[SerializeField, Scene] private int mainMenuSceneIndex;
```

![](https://github.com/user-attachments/assets/018a58be-5782-4323-8c52-29d8995babec)
![](https://github.com/user-attachments/assets/707c68bc-ca3b-4ffc-ad91-1af442ce7f8c)

Поддерживается автоматическое отслеживание изменений списка сцен, включённых в билд: даже при открытом редакторе изменения отразятся на внешнем виде поля.

Если значение поля не будет соответствовать какому-либо индексу сцены, будет выведено предупреждение; при этом значение поля не будет перезаписано, во избежание потери данных при активной работе со сценами.

![](https://github.com/user-attachments/assets/10199bb4-fd2e-4f77-aada-b09927360db4)

## CurveAttribute

Позволяет задать область определения анимационной кривой, а также настроить цвет.
Цвет передаётся в HEX-представлении ввиде строки: вы можете использовать одну из 157 констант, заданных в `PostEnot.Toolkits.HexColors`, идентичных пресетам цветов `UnityEngine.Color` или же задать собственный цвет. в формате `#RRGGBB` или `#RRGGBBAA`.

```csharp
[SerializeField, Curve(HexColors.Blue)] private AnimationCurve curve0;
[SerializeField, Curve(0, 0, 1, 1)] private AnimationCurve curve1;
[SerializeField, Curve(0, 0, 1, 1, HexColors.Red)] private AnimationCurve curve2;
```

![](https://github.com/user-attachments/assets/9feb4c8c-c1db-4162-97b3-e67d46d50e38)
![](https://github.com/user-attachments/assets/6c7ef0eb-6259-4b89-90fa-7c004979bd25)

## PreviewAttribute

Превращает стандартное поле ввода `ObjectField` в поле ввода с превью, демонстрация присвоенный полю ассет. Вы можете задать размер превью с помощью перечисления `PreviewSize`, поддерживающего четыре размера (`Small`, `Medium`, `Big`, `Large`), либо указав размер превью в пикселях самостоятельно. Стандартный размер превью - `Medium` (90px).

```csharp
[SerializeField, Preview] private GameObject prefabWithDefaultPreview;
[SerializeField, Preview(PreviewSize.Small)] private GameObject prefabWithSmallPreview;
[SerializeField, Preview(PreviewSize.Medium)] private GameObject prefabWithMediumPreview;
[SerializeField, Preview(PreviewSize.Big)] private GameObject prefabWithBigPreview;
[SerializeField, Preview(PreviewSize.Large)] private GameObject prefabWithLargePreview;
```
![](https://github.com/user-attachments/assets/ec5c1bf6-a829-4d16-8238-5fd621fd4dc8)

## LayerAttribute

Превращает стандартное `int` поле в поле ввода индекса слоя. На данный момент поддерживается применение атрибута только к полям типа `int`: применение к полям типа `string` на мотив `Naughty Attributes` не поддерживается.

```csharp
[SerializeField, Layer] private int geometryLayer;
```

![](https://github.com/user-attachments/assets/28d3b483-685f-4d7f-988c-5f75d45868a1)

## TagAttribute

Превращает стаднартное `string` поле в поле ввода тега. Тег применим только к полю типа `string`.

```csharp
[SerializeField, Tag] private string coinTag;
```

![](https://github.com/user-attachments/assets/5cecae41-bbb4-4146-bbcf-38f42921d01a)

### SliderAttribute

Является аналогом `UnityEngine.RangeAttribute`, но с большими возможностями кастомизации; позволяет добавлять подписи для минимальных и максимальных значений на мотив полей в инспекторе `AudioSource`.

```csharp
[SerializeField, Slider(0, 1)] private float precipitationChance;
[SerializeField, Slider(-90, 90, "Left", "Right")] private float courseDeviation;
```

![](https://github.com/user-attachments/assets/9238fc00-9395-4b80-9061-95539a44df5b)

## Модифицирующие атрибуты

### LabelAttribute

Позволяет указать собственную подпись поля в инспекторе.

```csharp
[SerializeField, Label("Alpha")] private int beta;
```

![Демонстрация работы атрибута Label](https://github.com/user-attachments/assets/b33b5047-3e86-449d-a49e-3deb21d5910b)

**Ещё больше кастомизации**

Так как UI Toolkit из коробки поддерживает форматирование с помощью тегов, вы можете изменить начертание, цвет и многое что ещё. Ниже представлен простой пример, как сделать подпись красной и жирной.

```csharp
[SerializeField, Label("<b><color=\"red\">Alpha")] private int beta;
```

![Демонстрация кастомизации подписи атрибута Label](https://github.com/user-attachments/assets/a208073c-ace0-4f91-bd33-795e425b1bfb)

Подробнее о тегах форматирования текста можно узнать [здесь](https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-rich-text-tags.html).

### VectorLabelsAttribute

Изменяет подписи к полям ввода осей вектора. Аттрибут применим к полям типа `Vector2`, `Vector2Int`, `Vector3`, `Vector3Int`.

Стандартные подписи к полям ввода имеют фиксированную ширину 15 пикселей; атрибут меняет логику отображения полей, выставляя ширину, достаточную, чтобы уместить весь текст самой длинной подписи, а также убирая отступ справа для `Vector2` и `Vector2Int` полей.

При необходимости вы можете самостоятельно установить ширину для каждого из полей; также, вы можете убрать подпись, передав пустую строку для конкретной оси.

Одно из наиболее интересных применений атрибута - превращение простого `Vector2`-поля в Min-Max поле для задачи границ.

```csharp
        [Header("Default:")]
        [SerializeField] private Vector2    default2;
        [SerializeField] private Vector2Int default2int;
        [SerializeField] private Vector3    default3;
        [SerializeField] private Vector3Int default3Int;

        [Header("String:")]
        [SerializeField, VectorLabels("Min", "Max")] private Vector2 minMaxField;
        [SerializeField, VectorLabels("Min", "Max")] private Vector2Int minMaxIntField;
        [SerializeField, VectorLabels("Min", "Middle", "Max")] private Vector3    minMiddleMaxField;
        [SerializeField, VectorLabels("Min", "Middle", "Max")] private Vector3Int minMiddleMaxIntField;

        [Header("None:")]
        [SerializeField, VectorLabels] private Vector2 none2;
        [SerializeField, VectorLabels] private Vector3 none3;
        [SerializeField, VectorLabels("", "Middle", "Max")] private Vector3 partialNone3;
```

![](https://github.com/user-attachments/assets/03855fcd-4887-49cd-ad98-014bbdcf10ef)

### WithoutLabelAttribute

Скрывает подпись к полю, растягивая по всей ширине инспектора.

```csharp
[SerializeField, WithoutLabel] private string myName = "PostEnot";
```

![Демонстрация работы атрибута WithoutLabel](https://github.com/user-attachments/assets/7afc6ba8-4a7e-4092-b1e4-5587fb0e9245)

### ReadOnlyAttribute

Делает поле неактивным для ввода.

```csharp
[SerializeField, ReadOnly] private string text = "Hello World!";
```

![Демонстрация работы атрибута ReadOnly](https://github.com/user-attachments/assets/4321d84a-7ae5-4b37-8de6-45ececc0e26a)

### AlternatingRowsAttribute

Включает чередование цвета элементов списка.

```csharp
[SerializeField, AlternatingRows] private List<int> list = new List<int>() { 0, 1, 2, 3, 4 };
```

![Демонстрация работы атрибута AlternatingRows](https://github.com/user-attachments/assets/5cba9e38-10f7-4c90-bcd8-1674fec3060f)

## Декоративные атрибуты

Декоративные атрибуты не изменяют само отображение поля ввода, но добавляют в инспектор дополнительные элементы. Все декоративные атрибуты поддерживают множественное применение к одному и тому же полю.

По умолчанию все декораторы размещаются над полем ввода. В некоторых случаях вам может потребоваться разместить декораторы под полем ввода; для этого необходимо установить значение `DrawMode` атрибута на `AttributeDrawMode.After`.

```csharp
[Button("Log Value", nameof(LogMessage), AttributeDrawMode.After)]
[SerializeField] private int value;

private void LogMessage() => Debug.Log(value);
```

![](https://github.com/user-attachments/assets/ac52602f-6a0f-4b3a-91f9-a44ccebf5770)

### ButtonAttribute

Добавляет кнопку. На данный момент атрибут применим только к полю, применение к методу не поддерживается.

```csharp
[Button("Randomize", nameof(Randomize))]
[SerializeField] private int value;

[SerializeField] private int min = 0;
[SerializeField] private int max = 100;

private void Randomize() => value = UnityEngine.Random.Range(min, max);
```

![](https://github.com/user-attachments/assets/d1ed7780-436d-4b3b-baa5-d43ac9345ca5)

### LineAttribute

Добавляет горизонтальную линию. С помощью значений атрибута вы можете настроить её толщину, а также цвет.

```csharp
[Line(HexColors.Blue)]
[Line(5)]
[Line(HexColors.Green, 5)]
[SerializeField] private int field0;
```

![](https://github.com/user-attachments/assets/87cbd2da-b463-4d5e-9587-8d5d082d295c)

Вы можете получить достаточно интересные результаты, совмещая `LineAttribute` с другими: например, `HeaderAttribute`.

```csharp
[Header("Portrets:")]
[Line]
[SerializeField, Preview, Label("Hero One")] private Sprite heroOnePortret;
[SerializeField, Preview, Label("Hero Two")] private Sprite heroTwoPortret;
```

![](https://github.com/user-attachments/assets/e28dd647-1cc8-4095-96f2-e3259c92977f)

### HelpBoxAttribute

Добавляет поле с сообщением. С помощью значений атрибута можно настроить как само сообщение, так и его тип. Значение `messageType` по умолчанию - `HelpBoxMessageType.None` (без иконки).

```csharp
[HelpBox("Default")]
[HelpBox("None: Hello World!", HelpBoxMessageType.None)]
[HelpBox("Info: Hello World!", HelpBoxMessageType.Info)]
[HelpBox("Warning: Hello World!", HelpBoxMessageType.Warning)]
[HelpBox("Error: Hello World!", HelpBoxMessageType.Error)]
[SerializeField] private int var;
```

![](https://github.com/user-attachments/assets/a353393e-df3f-4d46-b4ae-ca18702a019a)
