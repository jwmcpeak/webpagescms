using System;
using System.Collections.Generic;
using System.Dynamic;
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
        var sql = "SELECT p.*, t.Id as TagId, t.Name as TagName, " +
            "t.UrlFriendlyName as TagUrlFriendlyName FROM Posts p " +
            "LEFT JOIN PostsTagsMap m ON p.Id = m.PostId " +
            "LEFT JOIN Tags t ON t.Id = m.TagId " +
            "WHERE Id = @0";

        var results = DoGet(sql, id);

        return results.Any() ? results.First() : null;
    }

    public static dynamic Get(string slug)
    {
        var sql = "SELECT p.*, t.Id as TagId, t.Name as TagName, " +
            "t.UrlFriendlyName as TagUrlFriendlyName FROM Posts p " +
            "LEFT JOIN PostsTagsMap m ON p.Id = m.PostId " +
            "LEFT JOIN Tags t ON t.Id = m.TagId " +
            "WHERE Slug = @0";

        var results = DoGet(sql, slug);

        return results.Any() ? results.First() : null;
    }

    public static IEnumerable<dynamic> GetAll(string orderBy = null)
    {
        var sql = "SELECT p.*, t.Id as TagId, t.Name as TagName, " +
        "t.UrlFriendlyName as TagUrlFriendlyName FROM Posts p " +
        "LEFT JOIN PostsTagsMap m ON p.Id = m.PostId " +
        "LEFT JOIN Tags t ON t.Id = m.TagId";

        if (!string.IsNullOrEmpty(orderBy))
        {
            sql += " ORDER BY " + orderBy;
        }

        return DoGet(sql);
    }

    public static void Add(string title, string content, string slug, DateTime? datePublished, int authorId)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "INSERT INTO Posts (Title, Content, DatePublished, AuthorId, Slug) " +
                "VALUES (@0, @1, @2, @3, @4)";

            db.Execute(sql, title, content, datePublished, authorId, slug);
        }
    }

    public static void Edit(int id, string title, string content, string slug, DateTime? datePublished, int authorId)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "UPDATE Posts SET Title = @0, Content = @1, " +
            "DatePublished = @2, AuthorId = @3, Slug = @4 " +
                "WHERE Id = @5";

            db.Execute(sql, title, content, datePublished, authorId, slug, id);
        }
    }

    public static void Remove(string slug)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "DELETE FROM Posts WHERE Slug = @0";

            db.Execute(sql, slug);
        }
    }

    private static IEnumerable<dynamic> DoGet(string sql, params object[] values)
    {
        using (var db = Database.Open(_connectionString))
        {
            var posts = new List<dynamic>();

            var results = db.Query(sql, values);

            foreach (dynamic result in results)
            {
                dynamic post = posts.SingleOrDefault(p => p.Id == result.Id);

                if (post == null)
                {
                    post = CreatePostObject(result);

                    posts.Add(post);
                }

                if (result.TagId == null)
                {
                    continue;
                }

                dynamic tag = new ExpandoObject();

                tag.Id = result.TagId;
                tag.Name = result.TagName;
                tag.UrlFriendlyName = result.TagUrlFriendlyName;

                post.Tags.Add(tag);
            }

            return posts.ToArray();
        }

    }

    private static dynamic CreatePostObject(dynamic obj)
    {
        dynamic post = new ExpandoObject();

        post.Id = obj.Id;
        post.Title = obj.Title;
        post.Content = obj.Content;
        post.DateCreated = obj.DateCreated;
        post.DatePublished = obj.DatePublished;
        post.AuthorId = obj.AuthorId;
        post.Slug = obj.Slug;
        post.Tags = new List<dynamic>();

        return post;
    }
}