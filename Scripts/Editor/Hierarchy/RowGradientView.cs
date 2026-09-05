using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [UxmlElement]
    public partial class RowGradientView : VisualElement
    {
        private Color _color = Color.white;

        /// <summary>
        /// Цвет левой (непрозрачной) части градиента.
        /// </summary>
        public Color Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    MarkDirtyRepaint();
                }
            }
        }

        public RowGradientView()
        {
            generateVisualContent = OnGenerateVisualContent;

            // Растягиваемся на весь родительский контейнер
            style.position = Position.Absolute;
            style.left = 0;
            style.right = 0;
            style.top = 0;
            style.bottom = 0;
        }

        public RowGradientView(Color color) : this()
        {
            _color = color;
        }

        private void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            var painter = ctx.painter2D;
            if (painter == null)
                return;

            Rect rect = contentRect;
            if (rect.width <= 0f || rect.height <= 0f)
                return;

            Gradient gradient = new()
            {
                colorKeys = new GradientColorKey[2]
                {
                    new(_color, 0.0f),
                    new(_color, 1.0f)
                },
                alphaKeys = new GradientAlphaKey[3]
                {
                    new(1.0f, 0.0f),
                    new(0.7f, 0.5f), // почти непрозрачно до 70% времени
                    new(0.0f, 1.0f)  // резкое исчезновение в конце
                }
            };

            painter.fillGradient = new FillGradient()
            {
                start = new Vector2(rect.xMin, rect.yMin),
                end = new Vector2(rect.xMax, rect.yMin),
                gradient = gradient
            };

            // Создаём контур прямоугольника
            painter.BeginPath();
            painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMax));
            painter.LineTo(new Vector2(rect.xMin, rect.yMax));
            painter.ClosePath();
            painter.Fill();
        }
    }
}
