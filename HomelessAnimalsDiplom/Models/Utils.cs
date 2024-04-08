using System.Text.Encodings.Web;

namespace HomelessAnimalsDiplom.Models
{
    public static class Utils
    {
        public static HtmlEncoder HtmlEncoder;
        public static string? StrIf(this bool x, string v)
        {
            return x ? v : null;
        }
    }
}
