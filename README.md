# Описание

Пакет содержит набор утилит для редактора Unity, упрощающих разработку собственных инспекторов. Основная мотивация - идея создать аналог известного и хорошо зарекомендовавшего себя, но во многом устаревшего пакета [Naughty Attributes](https://github.com/dbrizov/NaughtyAttributes). Несмотря на хороший функционал, он полностью написан на IMGUI, что усложняет его использование с UI Toolkit; помимо этого, некоторые архитектурные решения и API по субъективному мнению автора морально устарели и требуют более лаконичной реализации. Это ни в коем случае не попытка принизить изначального автора проекта, а также всех его контрибьюторов, а напротив, дань уважения их заслугам.

Лицензия MIT, проект с полностью открытым исходным кодом, вы можете делать с ним всё, что пожелаете.

# Атрибуты

Пакет предоставляет множество готовых к использованию атрибутов для настройки отображения полей в инспекторе. Они полностью реализованы с помощью UI Toolkit и совместимы со стандартными атрибутами Unity.

Все атрибуты взаимосочетаемы друг с другом, то есть, одно поле может содержать два и более различных атрибута и будет корректно отображаться.

Ниже подробно описаны все предоставляемые атрибуты, способы и особенности использования.

## LabelAttribute

Позволяет указать собственную подпись поля в инспекторе.

**Код:**

```csharp
[SerializeField, Label("Alpha")] private int _beta;
```

**Инспектор:**

![Демонстрация работы атрибута Label](https://github.com/user-attachments/assets/b33b5047-3e86-449d-a49e-3deb21d5910b)

**Ещё больше кастомизации**

Так как UI Toolkit из коробки поддерживает форматирование с помощью тегов, вы можете изменить начертание, цвет и многое что ещё. Ниже представлен простой пример, как сделать подпись красной и жирной.

**Код:**

```csharp
[SerializeField, Label("<b><color=\"red\">Alpha")] private int _beta;
```

**Инспектор:**

![Демонстрация кастомизации подписи атрибута Label](https://github.com/user-attachments/assets/a208073c-ace0-4f91-bd33-795e425b1bfb)

Подробнее о тегах форматирования текста можно узнать [здесь](https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-rich-text-tags.html).

## ReadOnlyAttribute

Делает поле неактивным для ввода.

**Код:**

```csharp
[SerializeField, ReadOnly] private string _text = "Hello World!";
```

**Инспектор:**

![Демонстрация работы атрибута ReadOnly](https://github.com/user-attachments/assets/4321d84a-7ae5-4b37-8de6-45ececc0e26a)

## AlternatingRowsAttribute

Включает чередование цвета элементов списка.

**Код:**

```csharp
[SerializeField, AlternatingRows] private List<int> _list = new List<int>() { 0, 1, 2, 3, 4 };
```

**Инспектор:**

![Демонстрация работы атрибута AlternatingRows](https://github.com/user-attachments/assets/5cba9e38-10f7-4c90-bcd8-1674fec3060f)

## FoldoutAttribute, EndFoldoutAttribute

Перемещает поле атрибута, а также все поля ниже в раскрывающийся список. Перемещение происходит до следующего `FoldoutAttribute` или `EndFoldoutAttribute`. На данный момент вложенные раскрывающиеся списки не поддерживаются.

**Код:**
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
**Инспектор:**

![](https://github.com/user-attachments/assets/b4b99ee2-a635-431d-b056-f5953a9a3806)
![](https://github.com/user-attachments/assets/5b6ac336-7ac7-46d1-87be-bd9497d9fe5b)

## ButtonAttribute

Добавляет кнопку над свойством. На данный момент атрибут применим только к полю, применение к методу не поддерживается.

**Код:**
```csharp
[Button("Randomize", nameof(Randomize))]
[SerializeField] private int _value;

[SerializeField] private int _min = 0;
[SerializeField] private int _max = 100;

private void Randomize() => _value = UnityEngine.Random.Range(_min, _max);
```

![](https://github.com/user-attachments/assets/d1ed7780-436d-4b3b-baa5-d43ac9345ca5)
