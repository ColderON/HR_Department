using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sign_In
{
    public class Position
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long DepId { get; set; }

        public Position(IDataReader reader)
        {
            Id = reader.GetInt64(reader.GetOrdinal("Id"));
            Name = reader.GetString(reader.GetOrdinal("Name"));
            DepId = reader.GetInt64(reader.GetOrdinal("DepartId"));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
