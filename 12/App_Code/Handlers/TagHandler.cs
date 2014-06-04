using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using WebMatrix.Data;

/// <summary>
/// Summary description for PostHandler
/// </summary>
public class TagHandler : IHttpHandler
{
    public TagHandler()
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
        var name = context.Request.Form["tagName"];
        var friendlyName = context.Request.Form["tagFriendlyName"];
        var id = context.Request.Form["tagId"];

        if (string.IsNullOrWhiteSpace(friendlyName))
        {
            friendlyName = CreateTag(name);
        }

        if (mode == "edit")
        {
            EditTag(Convert.ToInt32(id), name, friendlyName);
        }
        else if (mode == "new")
        {
            CreateTag(name, friendlyName);
        }
        else if (mode == "delete")
        {
            DeleteTag(friendlyName);
        }


        context.Response.Redirect("~/admin/tag/");
    }

    private static void CreateTag(string name, string friendlyName)
    {
        var result = TagRepository.Get(friendlyName);

        if (result != null)
        {
            throw new HttpException(409, "Tag is already in use.");
        }

        TagRepository.Add(name, friendlyName);
    }

    private static void EditTag(int id, string name, string friendlyName)
    {
        var result = TagRepository.Get(id);

        if (result == null)
        {
            throw new HttpException(404, "Post does not exist.");
        }

        TagRepository.Edit(id, name, friendlyName);
    }

    private static void DeleteTag(string friendlyName)
    {
        TagRepository.Remove(friendlyName);
    }

    private static string CreateTag(string name)
    {
        name = name.ToLowerInvariant().Replace(" ", "-");
        name = Regex.Replace(name, @"[^0-9a-z-]", string.Empty);

        return name;
    }
}