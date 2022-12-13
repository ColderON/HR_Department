using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Windows;
using ClassLibrary.SQL_Tables_to_Classes;
using System.Xml.Linq;

namespace Sign_In;

public static class BIG_Helper
{
    public static IDbConnection? conn;
    public static List<Department> departmentsList= new List<Department>();
    public static List<Position> positionsList = new List<Position>();
    public static List<Status> statusList = new List<Status>();
    public static List<Employee> employeesList = new List<Employee>();
    public static List<Job_Order> job_OrdersList = new List<Job_Order>();


    public static void CreateConnection()
    {
        conn = new SqlConnection(new SqlConnectionStringBuilder
        {
            DataSource = "localhost",
            InitialCatalog = "HR_Department_SQL",
            IntegratedSecurity = true,
            MultipleActiveResultSets = true,
            TrustServerCertificate = true,
        }.ConnectionString);
    }
    public static void FillDepartments(IDbConnection connection)
    {
        departmentsList.Clear();
        IDbCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Name FROM Departments";
        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            departmentsList.Add(new Department(reader));
        }
    }
    public static void FillPositions(IDbConnection connection)
    {
        positionsList.Clear();
        IDbCommand cmd = connection.CreateCommand();
        cmd.CommandText = $"SELECT Id, Name, DepartId FROM Positions";
        IDataReader reader = cmd.ExecuteReader();
       
        while (reader.Read())
        {
            positionsList.Add(new Position(reader));
        }
    }

    public static void FillEmployees(IDbConnection connection)
    {
        employeesList.Clear();
        IDbCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, FirstName, LastName, Date_of_birth, Position_Id, Department_Id, PhotoName, PhotoImage, Beginning_of_Work, Status_Id from Employees";
        IDataReader reader = cmd.ExecuteReader();
                
        while (reader.Read())
        {
            employeesList.Add(new Employee(reader,connection));
        }
    }
    public static void AddNewEmployeeToList(IDbConnection connection)
    {       
        IDbCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, FirstName, LastName, Date_of_birth, Position_Id, Department_Id, PhotoName, PhotoImage, Beginning_of_Work, Status_Id from Employees ORDER BY Id DESC" +
            " OFFSET 0 ROWS FETCH FIRST 1 ROW ONLY";
        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            employeesList.Add(new Employee(reader, connection));
        }
    }   

    //public static void FillPhotos(IDbConnection connection)
    //{   
    //    //photos.Clear();
    //    //IDbCommand cmd = connection.CreateCommand();
    //    //cmd.CommandText = "SELECT Id, FileName, OwnerId from Photos";
    //    //IDataReader reader = cmd.ExecuteReader();

    //    //while (reader.Read())
    //    //{
    //    //    photos.Add(new Photo(reader, connection));
    //    //}
    //}

    public static void FillJobOrdersList(IDbConnection connection)
    {
        job_OrdersList.Clear();
        IDbCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, OrderNumber, DateOf, Person_Id, Position_Id, Department_Id from Job_Order";
        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            job_OrdersList.Add(new Job_Order(reader));
        }
    }
    public static void AddNewJobOrderToList(IDbConnection connection)
    {
        IDbCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, OrderNumber, DateOf, Person_Id, Position_Id, Department_Id from Job_Order ORDER BY Id DESC" +
            "OFFSET 0 ROWS FETCH FIRST 1 ROW ONLY";
        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            job_OrdersList.Add(new Job_Order(reader));
        }
    }

    public static void ChangeFName(IDbConnection connection, string fName, long emloyeeId)
    {
        IDbCommand cmd = connection.CreateCommand();
        cmd.CommandText = $"Update Personal SET FirstName = @fname where Id = @emplId";

        cmd.Parameters.Add(new SqlParameter
        {
            ParameterName = "fname",
            DbType = DbType.String,
            Size = 64,
            Direction = ParameterDirection.Input,
            Value = fName
        });
        cmd.Parameters.Add(new SqlParameter
        {
            ParameterName = "emplId",
            DbType = DbType.Int64,
            Size = 64,
            Direction = ParameterDirection.Input,
            Value = emloyeeId
        });
        cmd.ExecuteNonQuery();

        foreach (var item in employeesList)
        {
            if(item.Id == emloyeeId)
            {
                item.FirstName= fName;
                break;
            }
        }
    }
    public static void ChangeLName(IDbConnection connection, string lName, long emloyeeId)
    {
        IDbCommand cmd = connection.CreateCommand();
        cmd.CommandText = $"Update Personal SET LastName = @lname where Id = @emplId";

        cmd.Parameters.Add(new SqlParameter
        {
            ParameterName = "lname",
            DbType = DbType.String,
            Size = 64,
            Direction = ParameterDirection.Input,
            Value = lName
        });
        cmd.Parameters.Add(new SqlParameter
        {
            ParameterName = "emplId",
            DbType = DbType.Int64,
            Size = 64,
            Direction = ParameterDirection.Input,
            Value = emloyeeId
        });
        cmd.ExecuteNonQuery();

        foreach (var item in employeesList)
        {
            if (item.Id == emloyeeId)
            {
                item.LastName = lName;
                break;
            }
        }
    }
    public static void ChangeBirthday(IDbConnection connection, DateTime dt, long emloyeeId)
    {
        IDbCommand cmd = connection.CreateCommand();
        cmd.CommandText = $"Update Personal SET Date_of_birth = @dtBirth where Id = @emplId";

        cmd.Parameters.Add(new SqlParameter
        {
            ParameterName = "dtBirth",
            DbType = DbType.Date,
            Direction = ParameterDirection.Input,
            Value = dt
        });
        cmd.Parameters.Add(new SqlParameter
        {
            ParameterName = "emplId",
            DbType = DbType.Int64,
            Size = 64,
            Direction = ParameterDirection.Input,
            Value = emloyeeId
        });
        cmd.ExecuteNonQuery();

        foreach (var item in employeesList)
        {
            if (item.Id == emloyeeId)
            {
                item.DateOfBirth = dt;
                break;
            }
        }
    }
    public static void ChangeDepartmentAndOrPosition(IDbConnection connection, long departmentId, long positionId, long emloyeeId)
    {
        IDbCommand cmd = connection.CreateCommand();
        cmd.CommandText = $"Update Personal SET Position = @posId, Department_Id = @depId where Id = @emplId";

        cmd.Parameters.Add(new SqlParameter
        {
            ParameterName = "depId",
            DbType = DbType.Int64,
            Direction = ParameterDirection.Input,
            Value = departmentId
        });
        cmd.Parameters.Add(new SqlParameter
        {
            ParameterName = "posId",
            DbType = DbType.Int64,
            Direction = ParameterDirection.Input,
            Value = positionId
        });
        cmd.Parameters.Add(new SqlParameter
        {
            ParameterName = "emplId",
            DbType = DbType.Int64,
            Size = 64,
            Direction = ParameterDirection.Input,
            Value = emloyeeId
        });
        cmd.ExecuteNonQuery();

        foreach (var item in employeesList)
        {
            if (item.Id == emloyeeId)
            {
                item.DepId = departmentId;
                item.PosId= positionId;
                break;
            }
        }
    }
}
