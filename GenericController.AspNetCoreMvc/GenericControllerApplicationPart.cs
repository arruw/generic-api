using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GenericController.AspNetMvc
{
    public class GenericControllerApplicationPart : ApplicationPart, IApplicationPartTypeProvider
    {
        public IEnumerable<TypeInfo> Types { get; }

        public override string Name => "GenericController";

        private Type _genericControllerType { get; }

        public GenericControllerApplicationPart(Type genericControllerType, Type entityType)
        {
            _genericControllerType = genericControllerType;

            var entityTypes = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => entityType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            Types = entityTypes
                .Select(et => _genericControllerType.MakeGenericType(et))
                .Select(cct => cct.GetTypeInfo())
                .ToArray();
        }

        public GenericControllerApplicationPart(Type genericControllerType, IEnumerable<Type> entityTypes)
        {
            _genericControllerType = genericControllerType;

            Types = entityTypes
                .Select(et => _genericControllerType.MakeGenericType(et))
                .Select(cct => cct.GetTypeInfo())
                .ToArray();
        }
    }
}
