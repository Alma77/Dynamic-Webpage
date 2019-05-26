using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW_06.Services
{
    public class MyIdentityDataService
    {
        public const string AdminRoleName = "Admin";
        public const string EditorRoleName = "Editor";
        public const string AuthenticatedRoleName = "Authenticated";
        public const string AnonymousRoleName = "Anonymous";

        public const string Policy_Add = "CanAddForumPosts";
        public const string Policy_Edit = "CanEditForumPosts";
        public const string Policy_Delete = "CanDeleteForumPosts";
        public const string Policy_Comment = "CanCommentForumPosts";

        internal static void SeedData(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in new[] { AdminRoleName, EditorRoleName, AuthenticatedRoleName, AnonymousRoleName })
            {
                var role = roleManager.FindByNameAsync(roleName).Result;
                if (role == null)
                {
                    role = new IdentityRole(roleName);
                    roleManager.CreateAsync(role).GetAwaiter().GetResult();
                }
            }
            foreach (var userName in new[] { "admin@snow.edu", "editor@snow.edu", "authenticated@snow.edu", "anonymous@snow.edu" })
            {
                var user = userManager.FindByNameAsync(userName).Result;
                if (user == null)
                {
                    user = new IdentityUser(userName)
                    {
                        Email = userName
                    };
                    userManager.CreateAsync(user, "Abc123!").GetAwaiter().GetResult();

                }
                if (userName.StartsWith("admin"))
                {
                    userManager.AddToRoleAsync(user, AdminRoleName).GetAwaiter().GetResult();
                }
                if (userName.StartsWith("editor"))
                {
                    userManager.AddToRoleAsync(user, EditorRoleName).GetAwaiter().GetResult();
                }
                if (userName.StartsWith("authenticated"))
                {
                    userManager.AddToRoleAsync(user, AuthenticatedRoleName).GetAwaiter().GetResult();
                }
                if (userName.StartsWith("Anonymous"))
                {
                    userManager.AddToRoleAsync(user, AnonymousRoleName).GetAwaiter().GetResult();
                }

            }
        }
    }
}
