using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.SQL_Tables_to_Classes
{
    public class Status
    {
        public long Id { get; set; }
        public string Name { get; set; }       

        public Status(IDataReader reader)
        {
            Id = reader.GetInt64(reader.GetOrdinal("Id"));
            Name = reader.GetString(reader.GetOrdinal("Name"));           
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
