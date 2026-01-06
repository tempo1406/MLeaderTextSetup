using MLeaderTextSetup.Models;

namespace MLeaderTextSetup.Actions
{
    public class PreviewAction
    {
        public static string BuildText(MLeaderTextSettingModel s, PreviewDataModel d)
        {
            string text = s.FormatTemplate
                .Replace("{A}", d.A)
                .Replace("{B}", d.B)
                .Replace("{N}", d.N)
                .Replace("{E}", d.E)
                .Replace("{T}", d.T)
                .Replace("{D}", d.D)
                .Replace("{CUSTOM}", d.Custom);

            return text;
        }
    }
}
