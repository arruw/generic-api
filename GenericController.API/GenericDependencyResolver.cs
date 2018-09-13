using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GenericController.API
{
    public class GenericDependencyResolver : IDependencyResolver
    {
        private IGenericObjectGraphTypeCache _cache;

        public GenericDependencyResolver(IGenericObjectGraphTypeCache cache)
        {
            _cache = cache;
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            if (!_cache.TryGetGraphType(type, out var graphType))
            {
                graphType = type
                   .GetConstructor(new Type[] { })
                   .Invoke(new object[] { });

                _cache.AddGraphType(type, graphType);
            }

            return graphType;
        }
    }
}
