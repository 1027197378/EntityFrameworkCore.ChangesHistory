using System.Dynamic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextExtensions
    {
        public static void AutoHistory(this DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "操作记录异常");
            }

            context.AutoWriteHistory<AutoChangesHistory>();
        }

        public static void AutoHistory<T>(this DbContext context, Action<T> createHistory) where T : AutoChangesHistory, new()
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "操作记录异常");
            }

            context.AutoWriteHistory(createHistory);
        }

        private static void AutoWriteHistory<T>(this DbContext context, Action<T> createHistory = null) where T : AutoChangesHistory, new()
        {
            var entries = context.ChangeTracker.Entries()
                .Where(entry => entry.UseEntityHistory())
                .ToArray();

            foreach (var entry in entries)
            {
                var history = Activator.CreateInstance<T>();
                createHistory?.Invoke(history);

                if (entry.AutoChangesHistory(history))
                {
                    context.Add(history);
                }
            }
        }

        private static bool AutoChangesHistory<T>(this EntityEntry entry, T history) where T : AutoChangesHistory, new()
        {
            var properties = entry.GetPropertiesWithoutExcluded();

            var change = false;
            history.Source = entry.SourceName();
            switch (entry.State)
            {
                case EntityState.Added:
                    change = AddedHistory(history, entry, properties);
                    break;
                case EntityState.Modified:
                    change = ModifiedHistory(history, entry, properties);
                    break;
                case EntityState.Deleted:
                    change = DeletedHistory(history, entry, properties);
                    break;
                default:
                    break;
            }

            return change;
        }

        /// <summary>
        /// 是否启用实体记录
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static bool UseEntityHistory(this EntityEntry entry)
        {
            var historys = entry.Metadata.ClrType.GetCustomAttributes(typeof(HistoryAttribute), true).OfType<HistoryAttribute>();
            return historys.Any() && (historys.First().RecordStatus & entry.State) == entry.State;
        }

        /// <summary>
        /// 数据源标识
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string SourceName(this EntityEntry entry)
        {
            var sourceName = entry.Metadata.GetTableName();
            var historys = entry.Metadata.ClrType.GetCustomAttributes(typeof(HistoryAttribute), true).OfType<HistoryAttribute>();
            if (historys.Any())
            {
                sourceName = !string.IsNullOrWhiteSpace(historys.First().Source) ? historys.First().Source : sourceName;
            }

            return sourceName;
        }

        /// <summary>
        /// 排除字段
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static List<PropertyEntry> GetPropertiesWithoutExcluded(this EntityEntry entry)
        {
            var excludedProperties = entry.Metadata.ClrType.GetProperties()
                    .Where(x => x.GetCustomAttributes(typeof(IgnoreChangesAttribute), true).Any())
                    .Select(x => x.Name).ToArray();

            return entry.Properties.Where(f => !excludedProperties.Contains(f.Metadata.Name)).ToList();
        }

        private static string PrimaryKey(this EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey();

            var values = new List<object>();
            foreach (var property in key.Properties)
            {
                var value = entry.Property(property.Name).CurrentValue;
                if (value != null)
                {
                    values.Add(value);
                }
            }

            return string.Join(",", values);
        }

        /// <summary>
        /// Added
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="history"></param>
        /// <param name="entry"></param>
        /// <param name="properties"></param>
        private static bool AddedHistory<T>(T history, EntityEntry entry, IEnumerable<PropertyEntry> properties) where T : AutoChangesHistory, new()
        {
            var json = new ExpandoObject() as IDictionary<string, object>;

            foreach (var prop in properties)
            {
                json.TryAdd(prop.Metadata.Name, prop.CurrentValue);
            }

            history.SourceKey = entry.PrimaryKey() ?? "0";
            history.Kind = EntityState.Added;
            history.Before = JsonSerializer.SerializeToDocument(json);

            return true;
        }

        /// <summary>
        /// Modified
        /// </summary>
        /// <param name="history"></param>
        /// <param name="entry"></param>
        /// <param name="properties"></param>
        private static bool ModifiedHistory<T>(T history, EntityEntry entry, IEnumerable<PropertyEntry> properties)
            where T : AutoChangesHistory, new()
        {
            var before = new ExpandoObject() as IDictionary<string, object>;
            var after = new ExpandoObject() as IDictionary<string, object>;

            PropertyValues databaseValues = null;
            var modified = false;

            foreach (var prop in properties)
            {
                if (!Equals(prop.OriginalValue, prop.CurrentValue))
                {
                    before.TryAdd(prop.Metadata.Name, prop.OriginalValue);
                    modified = true;
                }
                else
                {
                    databaseValues ??= entry.GetDatabaseValues();
                    var originalValue = databaseValues?.GetValue<object>(prop.Metadata.Name);
                    before.TryAdd(prop.Metadata.Name, originalValue);

                    if (!Equals(originalValue, prop.CurrentValue))
                    {
                        modified = true;
                    }
                }

                after.TryAdd(prop.Metadata.Name, prop.CurrentValue);
            }

            if (!modified)
            {
                return false;
            }

            history.SourceKey = entry.PrimaryKey();
            history.Kind = EntityState.Modified;
            history.Before = JsonSerializer.SerializeToDocument(before, AutoChangesHistoryOptions.Instance.JsonSerializerOptions);
            history.After = JsonSerializer.SerializeToDocument(after, AutoChangesHistoryOptions.Instance.JsonSerializerOptions);

            return true;
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="history"></param>
        /// <param name="entry"></param>
        /// <param name="properties"></param>
        private static bool DeletedHistory<T>(T history, EntityEntry entry, IEnumerable<PropertyEntry> properties) where T : AutoChangesHistory, new()
        {
            var json = new ExpandoObject() as IDictionary<string, object>;

            foreach (var prop in properties)
            {
                json.TryAdd(prop.Metadata.Name, prop.OriginalValue);
            }

            history.SourceKey = entry.PrimaryKey();
            history.Kind = EntityState.Deleted;
            history.Before = JsonSerializer.SerializeToDocument(json, AutoChangesHistoryOptions.Instance.JsonSerializerOptions);

            return true;
        }
    }
}
