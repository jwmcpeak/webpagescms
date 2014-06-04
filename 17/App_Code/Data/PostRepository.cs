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

    public static IEnumerable<dynamic> GetPublishedPosts(int count = 0)
    {
        var topClause = count > 0 ? string.Format(" TOP {0} ", count) : string.Empty;

        var sql = string.Format("SELECT {0} p.*, t.Id as TagId, t.Name as TagName, " +
            "t.UrlFriendlyName as TagUrlFriendlyName, u.Username FROM Posts p " +
            "LEFT JOIN PostsTagsMap m ON p.Id = m.PostId " +
            "LEFT JOIN Tags t ON t.Id = m.TagId " +
            "INNER JOIN Users u ON u.Id = p.AuthorId " +
            "WHERE DatePublished IS NOT NULL AND DatePublished < getdate() " +
            "ORDER BY DatePublished DESC", topClause);

        return DoGet(sql);
    }

    public static IEnumerable<dynamic> GetPublishedPostsByTag(string tagName)
    {
        var sql = "SELECT p.*, t.Id as TagId, t.Name as TagName, " +
            "t.UrlFriendlyName as TagUrlFriendlyName, u.Username FROM Posts p " +
            "LEFT JOIN PostsTagsMap m ON p.Id = m.PostId " +
            "LEFT JOIN Tags t ON t.Id = m.TagId " +
            "INNER JOIN Users u ON u.Id = p.AuthorId " +
            "WHERE DatePublished IS NOT NULL AND DatePublished < getdate() " +
            "AND t.UrlFriendlyName = @0 " +
            "ORDER BY DatePublished DESC";

        return DoGet(sql, tagName);
    }

    public static dynamic Get(int id)
    {
        var sql = "SELECT p.*, t.Id as TagId, t.Name as TagName, " +
            "t.UrlFriendlyName as TagUrlFriendlyName, u.Username FROM Posts p " +
            "LEFT JOIN PostsTagsMap m ON p.Id = m.PostId " +
            "LEFT JOIN Tags t ON t.Id = m.TagId " +
            "INNER JOIN Users u ON u.Id = p.AuthorId " +
            "WHERE p.Id = @0";

        var results = DoGet(sql, id);

        return results.Any() ? results.First() : null;
    }

    public static dynamic Get(string slug)
    {
        var sql = "SELECT p.*, t.Id as TagId, t.Name as TagName, " +
            "t.UrlFriendlyName as TagUrlFriendlyName, u.Username FROM Posts p " +
            "LEFT JOIN PostsTagsMap m ON p.Id = m.PostId " +
            "LEFT JOIN Tags t ON t.Id = m.TagId " +
            "INNER JOIN Users u ON u.Id = p.AuthorId " +

            "WHERE Slug = @0";

        var results = DoGet(sql, slug);

        return results.Any() ? results.First() : null;
    }

    public static IEnumerable<dynamic> GetAll(string orderBy = null)
    {
        var sql = "SELECT p.*, t.Id as TagId, t.Name as TagName, " +
        "t.UrlFriendlyName as TagUrlFriendlyName, u.Username FROM Posts p " +
        "LEFT JOIN PostsTagsMap m ON p.Id = m.PostId " +
        "LEFT JOIN Tags t ON t.Id = m.TagId " +
        "INNER JOIN Users u ON u.Id = p.AuthorId";

        if (!string.IsNullOrEmpty(orderBy))
        {
            sql += " ORDER BY " + orderBy;
        }

        return DoGet(sql);
    }

    public static void Add(string title, string content, string slug, DateTime? datePublished, int authorId, IEnumerable<int> tags)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "INSERT INTO Posts (Title, Content, DatePublished, AuthorId, Slug) " +
                "VALUES (@0, @1, @2, @3, @4)";

            db.Execute(sql, title, content, datePublished, authorId, slug);

            var post = db.QuerySingle("SELECT * FROM Posts WHERE Slug = @0", slug);

            AddTags(post.Id, tags, db);
        }
    }

    public static void Edit(int id, string title, string content, string slug, DateTime? datePublished, int authorId, IEnumerable<int> tags)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "UPDATE Posts SET Title = @0, Content = @1, " +
            "DatePublished = @2, AuthorId = @3, Slug = @4 " +
                "WHERE Id = @5";

            db.Execute(sql, title, content, datePublished, authorId, slug, id);

            DeleteTags(id, db);

            AddTags(id, tags, db);
        }
    }

    public static void Remove(string slug)
    {
        using (var db = Database.Open(_connectionString))
        {
            var sql = "SELECT * FROM Posts WHERE Slug = @0";

            var post = db.QuerySingle(sql, slug);

            if (post == null)
            {
                return;
            }

            DeleteTags(post.Id, db);

            sql = "DELETE FROM Posts WHERE Id = @0";

            db.Execute(sql, post.Id);
        }
    }

    private static void AddTags(int postId, IEnumerable<int> tags, Database db)
    {
        if (!tags.Any())
        {
            return;
        }

        var sql = "INSERT INTO PostsTagsMap (PostId, TagId) VALUES (@0, @1)";

        foreach (var tag in tags)
        {
            db.Execute(sql, postId, tag);
        }
    }

    private static void DeleteTags(int id, Database db)
    {
        var sql = "DELETE FROM PostsTagsMap WHERE PostId = @0";

        db.Execute(sql, id);
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
        post.Username = obj.Username;

        return post;
    }
}