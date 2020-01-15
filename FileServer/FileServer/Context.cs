using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    public class Context: DbContext
    {
        public Context() : base("Server=DESKTOP-T1EO1QD;Database=FilesDb;Trusted_Connection=true;")
        {

        }
        public DbSet<FileDb> Files { get; set; }
    }
}
