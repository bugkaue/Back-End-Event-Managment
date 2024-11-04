using Microsoft.AspNetCore.Identity;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Services;
public static class SeedData
{
    public static async Task CreateAdminUser(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Cria a role "Admin" se não existir
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Cria um usuário administrador padrão
        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
        if (adminUser == null)
        {
            adminUser = new Usuario
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true,
                Nome = "Admin",
                Sobrenome = "User"  
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine("Usuário administrador criado com sucesso.");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Erro ao criar o usuário administrador: {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine("O usuário administrador já existe.");
        }
    }
}