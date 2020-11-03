using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Velvetech.Identity.API.Data
{
    /// <summary>
    /// Расширение для ModelBuilder
    /// </summary>
    public static class ModelBuilderExtension
    {
        public static ModelBuilder UseValueConverterForType<T>(this ModelBuilder modelBuilder, ValueConverter converter)
        {
            return modelBuilder.UseValueConverterForType(typeof(T), converter);
        }

        public static ModelBuilder UseValueConverterForType(this ModelBuilder modelBuilder, Type type,
            ValueConverter converter)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // note that entityType.GetProperties() will throw an exception, so we have to use reflection
                var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == type);

                foreach (var property in properties)
                {
                    modelBuilder.Entity(entityType.Name).Property(property.Name)
                        .HasConversion(converter);
                }
            }

            return modelBuilder;
        }

        public class DateTimeKindValueConverter : ValueConverter<DateTime, DateTime>
        {
            public DateTimeKindValueConverter(DateTimeKind kind, ConverterMappingHints mappingHints = null)
                : base(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, kind),
                    mappingHints)
            {
            }
        }

        /// <summary>
        /// Установка Kind по умолчанию для БД
        /// </summary>
        /// <param name="modelBuilder">Builder</param>
        /// <param name="kind">Kind</param>
        public static void SetDefaultDateTimeKind(this ModelBuilder modelBuilder, DateTimeKind kind)
        {
            modelBuilder.UseValueConverterForType<DateTime>(new DateTimeKindValueConverter(kind));
            modelBuilder.UseValueConverterForType<DateTime?>(new DateTimeKindValueConverter(kind));
        }
    }
}