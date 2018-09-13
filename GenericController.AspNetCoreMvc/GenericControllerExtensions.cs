using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericController.AspNetMvc
{
    public static class GenericControllerExtensions
    {
        public static IMvcBuilder AddGenericControllers(this IMvcBuilder mvcBuilder, Type genericControllerType, IEnumerable<Type> entityTypes)
        {
            mvcBuilder.ConfigureApplicationPartManager(manager =>
            {
                manager.ApplicationParts.Add(new GenericControllerApplicationPart(genericControllerType, entityTypes));
            });

            return mvcBuilder;
        }

        public static IMvcBuilder AddGenericControllers(this IMvcBuilder mvcBuilder, Type genericControllerType, Type entityType)
        {
            mvcBuilder.ConfigureApplicationPartManager(manager =>
            {
                manager.ApplicationParts.Add(new GenericControllerApplicationPart(genericControllerType, entityType));
            });

            return mvcBuilder;
        }

        public static IMvcBuilder AddGenericControllers<TDbContext, TEntity>(this IMvcBuilder mvcBuilder, Type genericControllerType) where TDbContext : DbContext
        {
            var entityTypes = typeof(TDbContext)
                .GetProperties()
                .Select(p => p.PropertyType)
                .Where(pt => pt.IsGenericType
                    && pt.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(pt => pt.GetGenericArguments()[0])
                .Where(t => typeof(TEntity).IsAssignableFrom(t)
                    && t.IsClass
                    && !t.IsAbstract);

            mvcBuilder.ConfigureApplicationPartManager(manager =>
            {
                manager.ApplicationParts.Add(new GenericControllerApplicationPart(genericControllerType, entityTypes));
            });

            return mvcBuilder;
        }
    }
}
