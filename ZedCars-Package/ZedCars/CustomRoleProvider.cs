// CustomRoleProvider.cs
// This file previously contained an ASP.NET Membership RoleProvider implementation
// that relied on System.Web.Security.RoleProvider.
//
// Role management has been fully migrated to ASP.NET Core Claims-based authentication.
// Roles ("Admin", "Customer", etc.) are now embedded as ClaimTypes.Role claims when
// the user signs in via AccountController (Login / AdminLogin actions) and are
// evaluated automatically by the [Authorize(Roles = "...")] attribute through the
// cookie authentication middleware configured in Program.cs.
//
// No replacement implementation is required here.
namespace ZedCars
{
    // Intentionally empty — role management is performed via claims in AccountController.
}
