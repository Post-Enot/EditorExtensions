namespace PostEnot.Toolkits
{
    public sealed class VectorLabelsAttribute : ModifyPropertyAttribute
    {
        public VectorLabelsAttribute(string xLabel = default, string yLabel = default, string zLabel = default)
        {
            XLabel = xLabel ?? string.Empty;
            YLabel = yLabel ?? string.Empty;
            ZLabel = zLabel ?? string.Empty;
        }

        public VectorLabelsAttribute(string xLabel, string yLabel, float xWidth, float yWidth)
        {
            XLabel = xLabel ?? string.Empty;
            YLabel = yLabel ?? string.Empty;
            XWidth = HandleWidth(xWidth);
            YWidth = HandleWidth(yWidth);
        }

        public VectorLabelsAttribute(string xLabel, string yLabel, string zLabel, float xWidth, float yWidth, float zWidth)
        {
            XLabel = xLabel ?? string.Empty;
            YLabel = yLabel ?? string.Empty;
            ZLabel = zLabel ?? string.Empty;
            XWidth = HandleWidth(xWidth);
            YWidth = HandleWidth(yWidth);
            ZWidth = HandleWidth(zWidth);
        }

        public string XLabel { get; }
        public string YLabel { get; }
        public string ZLabel { get; }
        public float XWidth { get; }
        public float YWidth { get; }
        public float ZWidth { get; }

        private static float HandleWidth(float width) => width < 0.0f ? 0.0f : width;
    }
}
