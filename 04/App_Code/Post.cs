using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using WebMatrix.Data;

/// <summary>
/// Summary description for Post
/// </summary>
public class Post
{
	public Post()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    private static WebPageRenderingBase Page
    {
        get { return WebPageContext.Current.Page; }
    }

    public static string Mode
    {
        get
        {
            if (Page.UrlData.Any())
            {
                return Page.UrlData[0].ToLower();
            }

            return string.Empty;
        }
    }

    public static string Slug
    {
        get {
            if (Mode == "edit")
            {
                return Page.UrlData[1];
            }

            return string.Empty;
        }
    }

    public static dynamic Current
    {
        get
        {
            using (var db = Database.Open("DefaultConnection"))
            {
                var sql = "SELECT * FROM Posts WHERE Slug = @0";

                var result = db.QuerySingle(sql, Slug);

                return result ?? CreatePostObject();
            }
        }
    }

    private static dynamic CreatePostObject()
    {
        dynamic obj = new ExpandoObject();

        obj.Title = string.Empty;
        obj.Content = string.Empty;
        obj.DateCreated = DateTime.Now;
        obj.DatePublished = null;
        obj.Slug = string.Empty;
        obj.AuthorId = null;

        return obj;
    }
}