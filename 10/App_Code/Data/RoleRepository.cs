using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using WebMatrix.Data;

/// <summary>
/// Summary description for UserRepository
/// </summary>
public class RoleRepository
{
    private static readonly string _connectionString = "DefaultConnection";

    public RoleRepository()
    {

    }

    public static dynamic Get(int id)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Roles WHERE ID = @0";

            return db.QuerySingle(sql, id);
        }
    }

    public static dynamic Get(string roleName)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Roles WHERE Name = @0";

            return db.QuerySingle(sql, roleName);
        }
    }

    public static IEnumerable<dynamic> GetAll(string orderBy = null, string where = null)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Roles";

            if (!string.IsNullOrEmpty(where))
            {
                sql += " WHERE " + where;
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                sql += " ORDER BY " + orderBy;
            }

            return db.Query(sql);
        }
    }

    public static void Add(string roleName)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Roles WHERE Name = @0";

            var role = db.QuerySingle(sql, roleName);

            if (role != null)
            {
                throw new Exception("User already exists");
            }
            
            sql = "INSERT INTO Roles (Name) " +
                "VALUES (@0)";

            db.Execute(sql, roleName);
        }
    }

    public static void Edit(int id, string roleName)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "UPDATE Roles SET Name = @0 WHERE Id = @1";

            db.Execute(sql, roleName, id);
        }
    }

    public static void Remove(string roleName)
    {
        var role = Get(roleName);

        if (role == null)
        {
            return;
        }

        using (var db = Database.Open(_connectionString))
        {
            var sql = "DELETE FROM Roles WHERE Name = @0";

            db.Execute(sql, roleName);

            sql = "DELETE FROM UsersRolesMap WHERE RoleId = @0";

            db.Execute(sql, role.Id);
        }
    }
}