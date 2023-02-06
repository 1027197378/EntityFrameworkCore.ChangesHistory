using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// 开启更改记录
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static ModelBuilder UseChangesHistory(this ModelBuilder modelBuilder, string tableName = "changes_history")
        {
            return ConfigChangesHistory<AutoChangesHistory>(modelBuilder, o =>
            {
                o.TableName = tableName;
            });
        }


        public static ModelBuilder UseChangesHistory<T>(this ModelBuilder modelBuilder, string tableName = "changes_history")
            where T : AutoChangesHistory, new()
        {
            return ConfigChangesHistory<T>(modelBuilder, o =>
            {
                o.TableName = tableName;
            });
        }


        private static ModelBuilder ConfigChangesHistory<T>(this ModelBuilder modelBuilder, Action<AutoChangesHistoryOptions> configure)
            where T : AutoChangesHistory, new()
        {
            var options = AutoChangesHistoryOptions.Instance;
            configure?.Invoke(options);

            modelBuilder.Entity<T>(b =>
            {
                b.ToTable(options.TableName).HasKey(a => a.Id);
                b.Property(c => c.Source).IsRequired();
                b.Property(c => c.SourceKey).IsRequired();
                b.Property(c => c.Kind).IsRequired();
            });

            return modelBuilder;
        }
    }
}
