using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using WebMatrix.Data;

/// <summary>
/// Summary description for UserRepository
/// </summary>
public class UserRepository
{
    private static readonly string _connectionString = "DefaultConnection";

    public UserRepository()
    {

    }

    public static dynamic Get(int id)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Users WHERE ID = @0";

            return db.QuerySingle(sql, id);
        }
    }

    public static dynamic Get(string username)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Users WHERE Username = @0";

            return db.QuerySingle(sql, username);
        }
    }

    public static IEnumerable<dynamic> GetAll(string orderBy = null, string where = null)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Users";

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

    public static void Add(string username, string password, string email)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Users WHERE Username = @0";

            var user = db.QuerySingle(sql, username);

            if (user != null)
            {
                throw new Exception("User already exists");
            }
            
            sql = "INSERT INTO Users (Username, Password, Email) " +
                "VALUES (@0, @1, @2)";

            db.Execute(sql, username, Crypto.HashPassword(password), email);
        }
    }

    public static void Edit(int id, string username, string password, string email)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "UPDATE Users SET Username = @0, Password = @1, " +
                "Email = @2 WHERE Id = @3";

            db.Execute(sql, username, Crypto.HashPassword(password), email, id);

        }
    }

    public static void Remove(string username)
    {
        var user = Get(username);

        if (user == null)
        {
            return;
        }

        using (var db = Database.Open(_connectionString))
        {
            var sql = "DELETE FROM Users WHERE Username = @0";

            db.Execute(sql, username);

            sql = "DELETE FROM UsersRolesMap WHERE UserId = @0";

            db.Execute(sql, user.Id);
        }
    }
}