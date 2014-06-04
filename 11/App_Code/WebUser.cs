using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for WebUser
/// </summary>
public class WebUser
{
    public static IEnumerable<string> GetRolesForUser(int id)
    {
        return RoleRepository.GetRolesForUser(id)
            .Select(r => (string)r.Name);
    }
}