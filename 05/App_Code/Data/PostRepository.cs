using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.Data;

/// <summary>
/// Summary description for BlogPostRepository
/// </summary>
public class PostRepository
{
    private static readonly string _connectionString = "DefaultConnection";

	public PostRepository()
	{
		
	}

    public static dynamic Get(int id)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Posts WHERE ID = @0";

            return db.QuerySingle(sql, id);
        }
    }

    public static dynamic Get(string slug)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Posts WHERE Slug = @0";

            return db.QuerySingle(sql, slug);
        }
    }

    public static IEnumerable<dynamic> GetAll()
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Posts";

            return db.Query(sql);
        }
    }

    public static void Add(string title, string content, string slug, DateTime? datePublished, int authorId)
    {
        using (var db = Database.Open("DefaultConnection"))
        {
            var sql = "INSERT INTO Posts (Title, Content, DatePublished, AuthorId, Slug) " +
                "VALUES (@0, @1, @2, @3, @4)";

            db.Execute(sql, title, content, datePublished, authorId, slug);
        }
    }

    public static void Edit(int id, string title, string content, string slug, DateTime? datePublished, int authorId)
    {
        using (var db = Database.Open("DefaultConnection"))
        {
            var sql = "UPDATE Posts SET Title = @0, Content = @1, " +
            "DatePublished = @2, AuthorId = @3, Slug = @4 " +
                "WHERE Id = @5";

            db.Execute(sql, title, content, datePublished, authorId, slug, id);
        }
    }
}