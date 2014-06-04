using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using WebMatrix.Data;

/// <summary>
/// Summary description for RoleHandler
/// </summary>
public class RoleHandler : IHttpHandler
{
    public bool IsReusable
    {
        get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
        var mode = context.Request.Form["mode"];
        var name = context.Request.Form["roleName"];
        var id = context.Request.Form["roleId"];

        if (mode == "edit")
        {
            Edit(Convert.ToInt32(id), name);
        }
        else if (mode == "new")
        {
            Create(name);
        }
        else if (mode == "delete")
        {
            Delete(name);
        }


        context.Response.Redirect("~/admin/role/");
    }

    private static void Create(string name)
    {
        var result = RoleRepository.Get(name);

        if (result != null)
        {
            throw new HttpException(409, "Role is already in use.");
        }

        RoleRepository.Add(name);
    }

    private static void Edit(int id, string name)
    {
        var result = RoleRepository.Get(id);

        if (result == null)
        {
            throw new HttpException(404, "Role does not exist.");
        }

        RoleRepository.Edit(id, name);
    }

    private static void Delete(string name)
    {
        RoleRepository.Remove(name);
    }
}