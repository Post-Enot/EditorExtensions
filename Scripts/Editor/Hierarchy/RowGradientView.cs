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

            Color leftColor = _color;
            Color rightColor = new(_color.r, _color.g, _color.b, 0f);
            Gradient gradient = new()
            {
                colorKeys = new GradientColorKey[2]
                {
                    new(leftColor, 0.0f),
                    new(rightColor, 1.0f)
                },
                alphaKeys = new GradientAlphaKey[2]
                {
                    new(1.0f, 0.0f),
                    new(0.0f, 1.0f)
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
