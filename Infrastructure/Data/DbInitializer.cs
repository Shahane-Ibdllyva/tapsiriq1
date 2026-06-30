using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void SeedRoles(AppDbContext context)
        {
            try
            {
                Console.WriteLine("🔍 Rollar yoxlanılır...");

                // Cədvəlin mövcud olub-olmadığını yoxlayırıq
                if (!context.Roles.Any())
                {
                    Console.WriteLine("📝 Rollar yoxdur, əlavə edilir...");

                    var roles = new[]
                    {
                        new Role { Name = "SuperAdmin", Description = "Tam sistem idarəçisi" },
                        new Role { Name = "Admin", Description = "İdarəçi" },
                        new Role { Name = "Manager", Description = "Menecer" },
                        new Role { Name = "User", Description = "Adi istifadəçi" }
                    };

                    context.Roles.AddRange(roles);
                    context.SaveChanges();

                    Console.WriteLine(" Rollar uğurla əlavə edildi!");
                }
                else
                {
                    Console.WriteLine(" Rollar artıq mövcuddur.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ XƏTA: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                throw; // Xətanı yuxarı ötür ki, proqram dayansın və səbəbi görəsən
            }
        }
    }
}