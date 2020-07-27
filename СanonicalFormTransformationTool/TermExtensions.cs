using System.Globalization;
using System.Linq;

namespace СanonicalFormTransformationTool
{
    public static class TermExtensions
    {
        public static string AsString(this TermSum value)
        {
            var str = string.Concat(value
                .Where(x => x.Value != 0)
                .Select((kvp, i) =>
                    string.Concat(
                        i > 0 && kvp.Value > 0 ? "+" : "",
                        kvp.Value switch
                        {
                            1 => kvp.Key.IsEmpty ? "1" : "",
                            -1 => kvp.Key.IsEmpty ? "-1" : "-",
                            _ => kvp.Value.ToString(CultureInfo.InvariantCulture)
                        },
                        kvp.Key.ToString())));
            return str == "" ? "0" : str;
        }
    }
}