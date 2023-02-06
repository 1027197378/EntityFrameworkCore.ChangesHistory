using EntityFrameworkCore.ChangesHistory.Entity;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.EntityFrameworkCore
{
    public static class ChangesHistoryExtensions
    {
        /// <summary>
        /// 获取更改的字段信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="history"></param>
        /// <param name="mappings">字段映射（key:字段名 value:映射名称）</param>
        /// <returns></returns>
        public static List<Difference> GetDifferences<T>(this T history, Dictionary<string, string> mappings) where T : AutoChangesHistory, new()
        {
            List<Difference> differences = new();
            foreach (var mapping in mappings)
            {
                var beforeElement = new JsonElement();
                var afterElement = new JsonElement();
                history?.Before?.RootElement.TryGetProperty(mapping.Key, out beforeElement);
                history?.After?.RootElement.TryGetProperty(mapping.Key, out afterElement);

                object before = null;
                object after = null;
                if (beforeElement.ValueKind != JsonValueKind.Undefined && beforeElement.ValueKind != JsonValueKind.Null)
                {
                    before = beforeElement.GetValue();
                }

                if (afterElement.ValueKind != JsonValueKind.Undefined && afterElement.ValueKind != JsonValueKind.Null)
                {
                    after = afterElement.GetValue();
                }

                if (!object.Equals(before, after))
                {
                    differences.Add(new Difference()
                    {
                        FieldName = mapping.Key,
                        CustomName = mapping.Value,
                        BeforeValue = before,
                        AfterValue = after
                    });
                }
            }

            return differences;
        }

        public static object GetValue(this JsonElement json)
        {
            object value;
            try
            {
                switch (json.ValueKind)
                {
                    case JsonValueKind.Undefined:
                        value = null;
                        break;
                    case JsonValueKind.Object:
                    case JsonValueKind.Array:
                        value = json.ToString();
                        break;
                    case JsonValueKind.String:
                        value = json.GetString();
                        break;
                    case JsonValueKind.Number:
                        value = json.GetDecimal();
                        break;
                    case JsonValueKind.True:
                        value = json.GetBoolean();
                        break;
                    case JsonValueKind.False:
                        value = json.GetBoolean();
                        break;
                    case JsonValueKind.Null:
                        value = null;
                        break;
                    default:
                        value = json.ToString();
                        break;
                }
            }
            catch
            {
                value = JsonSerializer.Serialize(json, new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
                });
            }

            return value;
        }
    }
}
