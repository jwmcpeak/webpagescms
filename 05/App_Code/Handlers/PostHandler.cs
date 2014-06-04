using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using WebMatrix.Data;

/// <summary>
/// Summary description for PostHandler
/// </summary>
public class PostHandler : IHttpHandler
{
    public PostHandler()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public bool IsReusable
    {
        get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
        var mode = context.Request.Form["mode"];
        var title = context.Request.Form["postTitle"];
        var content = context.Request.Form["postContent"];
        var slug = context.Request.Form["postSlug"];
        var id = context.Request.Form["postId"];
        var datePublished = context.Request.Form["postDatePublished"];

        if (string.IsNullOrWhiteSpace(slug))
        {
            slug = CreateSlug(title);
        }

        if (mode == "edit")
        {
            EditPost(Convert.ToInt32(id), title, content, slug, datePublished, 1);
        }
        else if (mode == "new")
        {
            CreatePost(title, content, slug, datePublished, 1);
        }

        

        

        context.Response.Redirect("~/admin/post/");
    }

    private static void CreatePost(string title, string content, 
        string slug, string datePublished, int authorId)
    {
        var result = PostRepository.Get(slug);
        DateTime? published = null;

        if (result != null)
        {
            throw new HttpException(409, "Slug is already in use.");
        }

        if (!string.IsNullOrWhiteSpace(datePublished))
        {
            published = DateTime.Parse(datePublished);
        }

        PostRepository.Add(title, content, slug, published, authorId);
    }

    private static void EditPost(int id, string title, string content,
        string slug, string datePublished, int authorId)
    {
        var result = PostRepository.Get(id);
        DateTime? published = null;

        if (result == null)
        {
            throw new HttpException(404, "Post does not exist.");
        }

        if (!string.IsNullOrWhiteSpace(datePublished))
        {
            published = DateTime.Parse(datePublished);
        }

        //PostRepository.Add(title, content, slug, published, authorId);
        PostRepository.Edit(id, title, content, slug, published, authorId);
    }

    private static string CreateSlug(string title)
    {
        title = title.ToLowerInvariant().Replace(" ", "-");
        title = Regex.Replace(title, @"[^0-9a-z-]", string.Empty);

        return title;
    }
}