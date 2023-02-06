using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkCore.ChangesHistory
{
    public class ChangesHistory : AutoChangesHistory
    {
        public ChangesHistory()
        {
            ModifyTime = DateTime.Now;
        }

        [Column("modify_user")]
        public string ModifyUser { get; set; }

        [Column("modify_name")]
        public string ModifyName { get; set; }
    }
}
