using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.SessionState;
using WebMatrix.Data;

/// <summary>
/// Summary description for AccountHandler
/// </summary>
public class AccountHandler : IHttpHandler, IReadOnlySessionState
{
    public bool IsReusable
    {
        get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
        AntiForgery.Validate();

        if (!WebUser.IsAuthenticated)
        {
            throw new HttpException(401, "You must login to do this.");
        }

        if (!WebUser.HasRole(UserRoles.Admin))
        {
            throw new HttpException(401, "You do not have permission to do this.");
        }


        var mode = context.Request.Form["mode"];
        var username = context.Request.Form["accountName"];
        var password1 = context.Request.Form["accountPassword1"];
        var password2 = context.Request.Form["accountPassword2"];
        var id = context.Request.Form["accountId"];
        var email = context.Request.Form["accountEmail"];
        var userRoles = context.Request.Form["accountRoles"];
        var roles = userRoles.Split(',').Select(v => Convert.ToInt32(v));


        if (mode == "delete")
        {
            Delete(username);
        }
        else
        {
            if (password1 != password2)
            {
                throw new Exception("Passwords do not match.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("Email cannot be blank.");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new Exception("Username cannot be blank.");
            }

            if (mode == "edit")
            {
                Edit(Convert.ToInt32(id), username, password1, email, roles);
            }
            else if (mode == "new")
            {
                Create(username, password1, email, roles);
            }
        }

        context.Response.Redirect("~/admin/account/");
    }

    private static void Create(string username, string password,
        string email, IEnumerable<int> roles)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new Exception("Password cannot be blank.");
        }

        var result = AccountRepository.Get(username);

        if (result != null)
        {
            throw new HttpException(409, "User alread exists.");
        }

        AccountRepository.Add(username, Crypto.HashPassword(password), email, roles);
    }

    private static void Edit(int id, string username, string password,
        string email, IEnumerable<int> roles)
    {
        var result = AccountRepository.Get(id);

        if (result == null)
        {
            throw new HttpException(404, "User does not exist.");
        }

        var updatedPassword = result.Password;

        if (!string.IsNullOrWhiteSpace(password))
        {
            updatedPassword = Crypto.HashPassword(password);
        }

        AccountRepository.Edit(id, result.Username, updatedPassword, email, roles);
    }

    private static void Delete(string username)
    {
        AccountRepository.Remove(username);
    }
}