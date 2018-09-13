using GenericController.API.Models;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericController.API
{
    public class GenericGraphQLQuery<T> : ObjectGraphType where T: class, IApplicationEntity
    {
        public GenericGraphQLQuery(IApplicationRepository<T> repository)
        {
            FieldAsync<GenericObjectGraphType<T>>("find",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                resolve: async context =>
                {
                    var id = Guid.Parse(context.GetArgument<string>("id"));
                    return await repository.GetAsync(id);
                });

            FieldAsync<ListGraphType<GenericObjectGraphType<T>>>("query",
                arguments: new QueryArguments {
                new QueryArgument<IntGraphType> { Name = "skip", DefaultValue = 0 },
                new QueryArgument<IntGraphType> { Name = "take", DefaultValue = 10 },
                },
                resolve: async context =>
                {
                    var skip = context.GetArgument<int>("skip");
                    var take = context.GetArgument<int>("take");
                    return await repository.Get().Skip(skip).Take(take).ToListAsync();
                });
        }
    }
}
