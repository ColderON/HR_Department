using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sign_In
{
    public class Employee
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }       
        public long PosId { get; set; }
        public long DepId { get; set; }
        public string ImageFileName { get; set; }
        public byte[] ImageBytes { get; set; }
        public DateTime StartedOn { get; set; }
        public long StatusId { get; set; }

        public Employee(IDataReader reader, IDbConnection connection)
        {
            int pht;
            Id = reader.GetInt64(reader.GetOrdinal("Id"));
            FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
            LastName = reader.GetString(reader.GetOrdinal("LastName"));
            DateOfBirth = reader.GetDateTime(reader.GetOrdinal("Date_of_birth"));
            PosId = reader.GetInt64(reader.GetOrdinal("Position_Id"));
            DepId = reader.GetInt64(reader.GetOrdinal("Department_Id"));

            pht = reader.GetOrdinal("PhotoName");
            if (!reader.IsDBNull(pht)) { ImageFileName = reader.GetString(reader.GetOrdinal("PhotoName")); }

            pht = reader.GetOrdinal("PhotoImage");
            if(!reader.IsDBNull(pht))
            {
                IDbCommand imageCommand = connection.CreateCommand();
                imageCommand.CommandText = $"Select PhotoImage from Employees where Id = {Id}";
                IDataReader ImageReader = imageCommand.ExecuteReader();

                MemoryStream ms = new MemoryStream();
                int buffSize = 1024;
                byte[] buffer = new byte[buffSize];

                while (ImageReader.Read())
                {
                    int retval;
                    int startIdex = 0;

                    while (true)
                    {
                        retval = (int)ImageReader.GetBytes(0, startIdex, buffer, 0, buffSize);
                        if (retval < 1) break;

                        ms.Write(buffer, 0, retval);
                        ms.Flush();
                        startIdex += retval;
                    }
                }
                if (ms.Length < 1) return;
                ImageBytes = ms.ToArray();
                ms.Close();
                ImageReader.Close();                
            }           
            StartedOn = reader.GetDateTime(reader.GetOrdinal("Beginning_of_Work"));
            StatusId = reader.GetInt64(reader.GetOrdinal("Status_Id"));   
            
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
