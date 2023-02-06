using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkCore.ChangesHistory
{
    [History("user", EntityState.Modified | EntityState.Added | EntityState.Deleted)]
    [Table("user")]
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Key, Column("id")]
        public string Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("age")]
        public int Age { get; set; }

        [IgnoreChanges]
        [Column("sex")]
        public string Sex { get; set; }
    }
}
