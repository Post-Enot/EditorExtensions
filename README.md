# Описание

Пакет содержит набор утилит для редактора Unity, упрощающих разработку собственных инспекторов. Основная мотивация - идея создать аналог известного и хорошо зарекомендовавшего себя, но во многом устаревшего пакета [Naughty Attributes](https://github.com/dbrizov/NaughtyAttributes). Несмотря на хороший функционал, он полностью написан на IMGUI, что усложняет его использование с UI Toolkit; помимо этого, некоторые архитектурные решения и API по субъективному мнению автора морально устарели и требуют более лаконичной реализации. Это ни в коем случае не попытка принизить изначального автора проекта, а также всех его контрибьюторов, а напротив, дань уважения их заслугам.

Лицензия MIT, проект с полностью открытым исходным кодом, вы можете делать с ним всё, что пожелаете.

Минимальная поддерживаемая версия Unity: 6.3 LTS. Потенциально, пакет должен без проблем работать и на более ранних версиях, однако тестирование не проводилось.

# Атрибуты

Пакет предоставляет множество готовых к использованию атрибутов для настройки отображения полей в инспекторе. Они полностью реализованы с помощью UI Toolkit и совместимы со стандартными атрибутами Unity.

Все атрибуты взаимосочетаемы друг с другом, то есть, одно поле может содержать два и более различных атрибута и будет корректно отображаться.

Ниже подробно описаны все предоставляемые атрибуты, способы и особенности использования.

## LabelAttribute

Позволяет указать собственную подпись поля в инспекторе.

```csharp
[SerializeField, Label("Alpha")] private int _beta;
```

![Демонстрация работы атрибута Label](https://github.com/user-attachments/assets/b33b5047-3e86-449d-a49e-3deb21d5910b)

**Ещё больше кастомизации**

Так как UI Toolkit из коробки поддерживает форматирование с помощью тегов, вы можете изменить начертание, цвет и многое что ещё. Ниже представлен простой пример, как сделать подпись красной и жирной.

```csharp
[SerializeField, Label("<b><color=\"red\">Alpha")] private int _beta;
```

![Демонстрация кастомизации подписи атрибута Label](https://github.com/user-attachments/assets/a208073c-ace0-4f91-bd33-795e425b1bfb)

Подробнее о тегах форматирования текста можно узнать [здесь](https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-rich-text-tags.html).

## WithoutLabelAttribute

Скрывает подпись к полю, растягивая по всей ширине инспектора.

```csharp
[SerializeField, WithoutLabel] private string _myName = "PostEnot";
```

![Демонстрация работы атрибута WithoutLabel](https://github.com/user-attachments/assets/7afc6ba8-4a7e-4092-b1e4-5587fb0e9245)

## ReadOnlyAttribute

Делает поле неактивным для ввода.

```csharp
[SerializeField, ReadOnly] private string _text = "Hello World!";
```

![Демонстрация работы атрибута ReadOnly](https://github.com/user-attachments/assets/4321d84a-7ae5-4b37-8de6-45ececc0e26a)

## AlternatingRowsAttribute

Включает чередование цвета элементов списка.

```csharp
[SerializeField, AlternatingRows] private List<int> _list = new List<int>() { 0, 1, 2, 3, 4 };
```

![Демонстрация работы атрибута AlternatingRows](https://github.com/user-attachments/assets/5cba9e38-10f7-4c90-bcd8-1674fec3060f)

## FoldoutAttribute, EndFoldoutAttribute

Перемещает поле атрибута, а также все поля ниже в раскрывающийся список. Перемещение происходит до следующего `FoldoutAttribute` или `EndFoldoutAttribute`. На данный момент вложенные раскрывающиеся списки не поддерживаются.

```csharp
[Foldout("<b>Foldout 1")]
[SerializeField] private int _var0;
[SerializeField] private int _var1;
[SerializeField] private int _var2;
[EndFoldout]

[SerializeField] private int _var3;

[Foldout("Foldout 2")]
[SerializeField] private int _var4;
[SerializeField] private int _var5;
[SerializeField] private int _var6;

[Foldout("Foldout 2")]
[SerializeField] private int _var7;
[SerializeField] private int _var8;
[SerializeField] private int _var9;
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

## ButtonAttribute

Добавляет кнопку над полем. На данный момент атрибут применим только к полю, применение к методу не поддерживается.

```csharp
[Button("Randomize", nameof(Randomize))]
[SerializeField] private int _value;

[SerializeField] private int _min = 0;
[SerializeField] private int _max = 100;

private void Randomize() => _value = UnityEngine.Random.Range(_min, _max);
```

![](https://github.com/user-attachments/assets/d1ed7780-436d-4b3b-baa5-d43ac9345ca5)

По умолчанию кнопка располагается над полем; для того, чтобы разместить кнопку под полем, необходимо изменить значение параметра атрибута `position` на `ButtonPosition.Down`.

```csharp
[Button("Log Value", nameof(LogMessage), ButtonPosition.Down)]
[SerializeField] private int _value;

private void LogMessage() => Debug.Log(_value);
```

![](https://github.com/user-attachments/assets/ac52602f-6a0f-4b3a-91f9-a44ccebf5770)
