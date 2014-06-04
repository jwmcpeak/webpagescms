using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.SessionState;

/// <summary>
/// Summary description for WebUser
/// </summary>
public class WebUser
{
    private static HttpSessionState Session
    {
        get { return HttpContext.Current.Session; }
    }

    public static IEnumerable<string> GetRolesForUser(int id)
    {
        return RoleRepository.GetRolesForUser(id)
            .Select(r => (string)r.Name);
    }

    public static bool Authenticate(string username, string password)
    {
        var user = AccountRepository.Get(username);

        if (user == null)
        {
            return false;
        }

        return Crypto.VerifyHashedPassword((string)user.Password, password);
    }

    public static void Login(string username)
    {
        var user = AccountRepository.Get(username);

        if (user == null)
        {
            return;
        }

        SetupSession(user);
    }

    public static bool AuthenticateAndLogin(string username, string password)
    {
        var user = AccountRepository.Get(username);

        if (user == null)
        {
            return false;
        }

        var verified = Crypto.VerifyHashedPassword((string)user.Password, password);

        if (!verified)
        {
            return false;
        }

        SetupSession(user);

        return true;
    }

    private static void SetupSession(dynamic user)
    {
        Session["userid"] = (int)user.Id;
        Session["username"] = (string)user.Username;
        Session["email"] = (string)user.Email;
    }

    public static int UserId
    {
        get
        {
            var value = Session["userid"];

            if (value == null)
            {
                return -1;
            }

            return (int)value;
        }
    }

    public static string Username
    {
        get
        {
            var value = Session["username"];

            if (value == null)
            {
                return string.Empty;
            }

            return (string)value;
        }
    }

    public static string Email
    {
        get
        {
            var value = Session["email"];

            if (value == null)
            {
                return string.Empty;
            }

            return (string)value;
        }
    }

    public static bool IsAuthenticated
    {
        get
        {
            return !string.IsNullOrEmpty(Username);
        }
    }
}