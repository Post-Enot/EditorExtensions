namespace PostEnot.Toolkits
{
    public sealed class VectorLabelsAttribute : ModifyPropertyAttribute
    {
        public VectorLabelsAttribute() {}

        public VectorLabelsAttribute(string xLabel, string yLabel)
        {
            XLabel = xLabel;
            YLabel = yLabel;
        }

        public VectorLabelsAttribute(string xLabel, string yLabel, int xWidth, int yWidth)
        {
            XLabel = xLabel;
            YLabel = yLabel;
            XWidth = xWidth;
            YWidth = yWidth;
        }

        public VectorLabelsAttribute(string xLabel, string yLabel, string zLabel)
        {
            XLabel = xLabel;
            YLabel = yLabel;
            ZLabel = zLabel;
        }

        public VectorLabelsAttribute(string xLabel, string yLabel, string zLabel, int xWidth, int yWidth, int zWidth)
        {
            XLabel = xLabel;
            YLabel = yLabel;
            ZLabel = zLabel;
            XWidth = xWidth;
            YWidth = yWidth;
            ZWidth = zWidth;
        }

        public string XLabel { get; }
        public string YLabel { get; }
        public string ZLabel { get; }
        public int XWidth { get; }
        public int YWidth { get; }
        public int ZWidth { get; }
    }
}
