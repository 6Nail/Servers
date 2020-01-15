using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileClient
{
    public class File
    {
        public Guid Guid { get; set;} = Guid.NewGuid();
        public string Name { get; set;}
        public DateTime AddDate { get; set;} = DateTime.Now;
        public DateTime DeleteDate { get; set;}
    }
}
