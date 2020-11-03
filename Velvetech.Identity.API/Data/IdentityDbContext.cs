using System;
using Marques.EFCore.SnakeCase;
using Microsoft.EntityFrameworkCore;
using Velvetech.Identity.API.Entities;

namespace Velvetech.Identity.API.Data
{
    /// <summary>
    /// DbContext
    /// </summary>
    public class IdentityDbContext : DbContext
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Пользователи
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Конфигурация БД
        /// </summary>
        /// <param name="modelBuilder">Builder модели для конфигурации БД</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ToSnakeCase();
            modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);
        }
    }
}