using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LeaveManagement.Common.Constants;

namespace LeaveManagement.Data.Configurations.Entities;

/* SETTING ADMIN USER FOR SYSTEM 
--------------------------------------------------------------------*/

public class UserSeedConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        var hash = new PasswordHasher<Employee>();
        builder.HasData(
            new Employee
            {
                Id = "62b3adbb-e3a7-4f5f-8a05-61d2074df6c2",
                Email = "admin@host.com",
                NormalizedEmail = "ADMIN@HOST.COM",
                UserName = "admin@host.com",
                NormalizedUserName = "ADMIN@HOST.COM",
                FirstName = "System",
                LastName = "Admin",
                PasswordHash = hash.HashPassword(null, "L0g!n@123"),
                EmailConfirmed = true
            },
            new Employee
            {
                Id = "f6c2adbb-4f5f-8a05-e3a7-62b3adbb61d2",
                Email = "user@login.com",
                NormalizedEmail = "USER@LOGIN.COM",
                UserName = "user@login.com",
                NormalizedUserName = "USER@LOGIN.COM",
                FirstName = "User",
                LastName = "Test",
                PasswordHash = hash.HashPassword(null, "P@ssW0rd!23"),
                EmailConfirmed = true
            }
        );
    }
}