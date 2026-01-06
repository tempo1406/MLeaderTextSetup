namespace MLeaderTextSetup.Models
{
    public class MLeaderTextSettingModel
    {
        public string TextStyleName { get; set; } = "Standard";
        public double TextHeight { get; set; } = 2.5;

        public bool ColorByLayer { get; set; } = true;
        public short ColorIndex { get; set; } = 256;

        public string FormatTemplate { get; set; } = "{A}-{B},{N},{E}-{T} / d{D}";
    }
}
