using GenericController.API.Models;
using GenericController.AspNetMvc;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GenericController.API
{
    [GenericController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class GenericController<T> : Controller where T : class, IApplicationEntity
    {
        private IApplicationRepository<T> _repository;
        private IGenericObjectGraphTypeCache _cache;

        public GenericController(IApplicationRepository<T> repository, IGenericObjectGraphTypeCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Query(int take = 10, int skip = 0)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(_repository.Get().Skip(skip).Take(take).ToList());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Find(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var record = await _repository.GetAsync(id);
            if (record == null)
                return NotFound();

            return Ok(record);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] T record)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            _repository.Create(record);
            if (await _repository.SaveAsync() == 0)
                return BadRequest();

            return CreatedAtAction("Find", new { id = record.Id }, record);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] T record)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (id != record.Id)
                return BadRequest();

            _repository.Update(record);
            if (await _repository.SaveAsync() == 0)
                return BadRequest();

            return Ok(record);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            _repository.Delete(id);
            if (await _repository.SaveAsync() == 0)
                return BadRequest();

            return NoContent();
        }

        [HttpPost]
        [Route("graph")]
        public async Task<IActionResult> GraphQL()
        {
            if (!ModelState.IsValid)
                return BadRequest();

            using (var streamReader = new StreamReader(HttpContext.Request.Body))
            {
                var query = await streamReader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest();

                var schema = new Schema
                {
                    Query = new GenericGraphQLQuery<T>(_repository),
                    DependencyResolver = new GenericDependencyResolver(_cache),
                };

                var result = await new DocumentExecuter()
                    .ExecuteAsync(options =>
                    {
                        options.Schema = schema;
                        options.Query = query;
                    });

                var json = new DocumentWriter(indent: true).Write(result);
                HttpContext.Response.ContentType = "application/json";
                await HttpContext.Response.WriteAsync(json);

                return result.Errors?.Any() == false ? (IActionResult)Ok() : (IActionResult)BadRequest();
            }
        }
    }
}
