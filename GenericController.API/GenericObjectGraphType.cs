using GenericController.API.Models;
using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GenericController.API
{
    public class GenericObjectGraphType<T> : ObjectGraphType<T> where T: class, IApplicationEntity
    {
        public GenericObjectGraphType()
        {
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.GetTypeInfo().IsValueType 
                    || p.PropertyType == typeof(string));

            foreach (var propertyInfo in properties)
            {
                try
                {
                    var graphType = propertyInfo.PropertyType.GetGraphTypeFromType(propertyInfo.PropertyType.IsNullable());
                    Field(graphType, propertyInfo.Name);
                }
                catch
                {
                    Field<StringGraphType>(propertyInfo.Name,
                        resolve: context => context.Source.GetProperyValue(propertyInfo.Name).ToString());
                }
            }
        }
    }
}
