using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    public class FileDb
    {
        [Key]
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public DateTime? AddDate { get; set; } = DateTime.Now;

    }
}
