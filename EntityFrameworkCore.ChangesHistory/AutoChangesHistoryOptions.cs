using System.Text.Encodings.Web;
using System.Text.Json;

namespace Microsoft.EntityFrameworkCore.Internal
{
    public sealed class AutoChangesHistoryOptions
    {
        internal static AutoChangesHistoryOptions Instance { get; } = new AutoChangesHistoryOptions();

        private AutoChangesHistoryOptions()
        {

        }

        public string TableName { get; set; }

        public JsonSerializerOptions JsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}
