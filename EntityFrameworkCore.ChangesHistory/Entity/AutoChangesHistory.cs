using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Microsoft.EntityFrameworkCore
{
    public class AutoChangesHistory: AutoChangesHistory<string>
    {
        public AutoChangesHistory()
        {
            Id = Guid.NewGuid().ToString();
            ModifyTime = DateTime.Now;
        }
    }

    public class AutoChangesHistory<T>
    {
        public AutoChangesHistory()
        {
        }

        [Key, Column("id")]
        public T Id { get; set; }

        [Column("source_key")]
        public string SourceKey { get; set; }

        [Column("source")]
        public string Source { get; set; }

        [Column("kind")]
        public EntityState Kind { get; set; }

        [Column("before")]
        public JsonDocument Before { get; set; }

        [Column("after")]
        public JsonDocument After { get; set; }

        [Column("modify_time")]
        public DateTime? ModifyTime { get; set; }
    }
}
