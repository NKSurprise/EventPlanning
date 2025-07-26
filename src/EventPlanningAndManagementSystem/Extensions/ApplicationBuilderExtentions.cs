using Microsoft.AspNetCore.Identity;

namespace EventPlanningAndManagementSystem.Extensions
{
    public static class ApplicationBuilderExtentions
    {

        public static IApplicationBuilder SeedAdmin(
            this IApplicationBuilder app)
        {
            using IServiceScope scopedServices = app.ApplicationServices.CreateScope();

            IServiceProvider services = scopedServices.ServiceProvider;

            UserManager<IdentityUser> userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            Task.Run(async () =>
            {
                if (!await roleManager.RoleExistsAsync("Administrator"))
                {
                    IdentityRole role = new("Administrator");
                    await roleManager.CreateAsync(role);
                }
                IdentityUser admin = await userManager.FindByEmailAsync("admin@events.com");
                await userManager.AddToRoleAsync(admin, "Administrator");
            })
            .GetAwaiter()
            .GetResult();

            return app;
        }
    }
}
