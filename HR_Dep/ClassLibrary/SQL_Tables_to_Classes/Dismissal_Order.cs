using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.SQL_Tables_to_Classes
{
    public class Dismissal_Order
    {
        public long Id { get; set; }
        public long OrderNumber { get; set; }
        public DateTime DateOf { get; set; }
        public long PersId { get; set; }
        public long PosId { get; set; }
        public long DepId { get; set; }

        public Dismissal_Order(IDataReader reader)
        {
            Id = reader.GetInt64(reader.GetOrdinal("Id"));
            OrderNumber = reader.GetInt64(reader.GetOrdinal("OrderNumber"));
            DateOf = reader.GetDateTime(reader.GetOrdinal("DateOf"));
            PersId = reader.GetInt64(reader.GetOrdinal("Person_Id"));
            PosId = reader.GetInt64(reader.GetOrdinal("Position_Id"));
            DepId = reader.GetInt64(reader.GetOrdinal("Department_Id"));
        }
    }
}
