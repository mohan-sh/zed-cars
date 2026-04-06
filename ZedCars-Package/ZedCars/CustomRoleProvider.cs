using System;
using System.Web.Security;
using ZedCars.Database;
using MySql.Data.MySqlClient;

namespace ZedCars
{
    public class CustomRoleProvider : RoleProvider
    {
        public override string ApplicationName { get; set; }

        public override string[] GetRolesForUser(string username)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT Role FROM Users WHERE Username=@u AND IsActive=TRUE",
                        connection))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        var role = cmd.ExecuteScalar()?.ToString();
                        if (!string.IsNullOrEmpty(role))
                            return new[] { role };
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetRolesForUser error: " + ex.Message);
            }
            return new string[0];
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var roles = GetRolesForUser(username);
            return Array.IndexOf(roles, roleName) >= 0;
        }

        #region Not Implemented
        public override void AddUsersToRoles(string[] u, string[] r)    { throw new NotImplementedException(); }
        public override void CreateRole(string roleName)                 { throw new NotImplementedException(); }
        public override bool DeleteRole(string r, bool throwOnPopulated) { throw new NotImplementedException(); }
        public override string[] FindUsersInRole(string r, string u)     { throw new NotImplementedException(); }
        public override string[] GetAllRoles()                           { throw new NotImplementedException(); }
        public override string[] GetUsersInRole(string roleName)         { throw new NotImplementedException(); }
        public override void RemoveUsersFromRoles(string[] u, string[] r){ throw new NotImplementedException(); }
        public override bool RoleExists(string roleName)                 { throw new NotImplementedException(); }
        #endregion
    }
}
