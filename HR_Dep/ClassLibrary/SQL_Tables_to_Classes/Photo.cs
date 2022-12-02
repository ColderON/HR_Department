using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Sign_In
{
    public class Photo
    {
        public long Id { get; set; }
        public string imageFileName { get; set; }
        public byte[] imageBytes { get; set; }
        public long OwnerId { get; set; }
        
        public Photo(IDataReader reader, IDbConnection connection)
        {
            Id = reader.GetInt64(reader.GetOrdinal("Id"));
            imageFileName = reader.GetString(reader.GetOrdinal("FileName"));
            OwnerId = reader.GetInt64(reader.GetOrdinal("OwnerId"));


            IDbCommand imageCommand = connection.CreateCommand();
            imageCommand.CommandText= $"Select Image from Photos where OwnerId = {OwnerId}";
            reader = imageCommand.ExecuteReader();

            MemoryStream ms = new MemoryStream();
            int buffSize = 1024;
            byte[] buffer = new byte[buffSize];

            while (reader.Read())
            {
                int retval;
                int startIdex = 0;

                while (true)
                {
                    retval = (int)reader.GetBytes(0, startIdex, buffer, 0, buffSize);
                    if (retval < 1) break;

                    ms.Write(buffer, 0, retval);
                    ms.Flush();
                    startIdex += retval;
                }
            }

            if (ms.Length < 1) return;

            imageBytes = ms.ToArray();
            ms.Close();
            reader.Close(); 
        }        
    }
}
