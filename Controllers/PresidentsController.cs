using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using PresidentsList.API.Filters;
using PresidentsList.API.Models;
using Microsoft.Rest;
using Swashbuckle.Swagger.Annotations;

namespace PresidentsList.API.Filters
{
    public class HttpOperationExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is HttpOperationException)
            {
                var ex = (HttpOperationException)context.Exception;
                context.Response = ex.Response;
            }
        }
    }
}

namespace PresidentsList.API.Controllers
{
    [HttpOperationExceptionFilter]
    public class PresidentsController : ApiController
    {
        private const string FILENAME = "Presidents.json";
        private GenericStorage _storage;

        public PresidentsController()
        {
            _storage = new GenericStorage();
        }

        private async Task<IEnumerable<President>> GetPresidents()
        {
            var presidents = await _storage.Get(FILENAME);

            if (presidents == null)
            {
                presidents = await _storage.Save(new[]{
                        new President { Name="George Washington", Birthday="1732-2-22", BirthPlace="Westmoreland Co., Va.", DeathDay="1799-12-14", DeathPlace="Mount Vernon, Va.", Country="US", Age=0 },
                        new President { Name="John Adams", Birthday="1735-10-30", BirthPlace="Quincy, Mass.", DeathDay="1826-7-4", DeathPlace="Quincy, Mass.", Country="US", Age=0 }
                    }
                , FILENAME);

            }

            var presidentsList = presidents.ToList();
            return presidentsList;
        }

        /// <summary>
        /// Gets the list of presidents
        /// </summary>
        /// <returns>The presidents</returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Type = typeof(IEnumerable<President>))]
        public async Task<IEnumerable<President>> Get()
        {
            return await GetPresidents();
        }

        /// <summary>
        /// Gets a specific president
        /// </summary>
        /// <param name="id">Identifier for the president</param>
        /// <returns>The requested president</returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "OK",
            Type = typeof(IEnumerable<President>))]
        [SwaggerResponse(HttpStatusCode.NotFound,
            Description = "President not found",
            Type = typeof(IEnumerable<President>))]
        [SwaggerOperation("GetPresidentById")]
        public async Task<President> Get([FromUri] int id)
        {
            var presidents = await GetPresidents();
            return presidents.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Creates a new president
        /// </summary>
        /// <param name="presidents">The new president</param>
        /// <returns>The saved presidents</returns>
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created,
            Description = "Created",
            Type = typeof(President))]
        public async Task<President> Post([FromBody] President president)
        {
            var presidents = await GetPresidents();
            var presidentList = presidents.ToList();

            president.CreatedBy = "Trial User";
            presidentList.Add(president);
            await _storage.Save(presidentList, FILENAME);
            return president;
        }

        /// <summary>
        /// Updates a president
        /// </summary>
        /// <param name="president">The new president values</param>
        /// <returns>The updated president</returns>
        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Updated",
            Type = typeof(President))]
        public async Task<President> Put([FromBody] President president)
        {
            await Delete(president.Id);
            await Post(president);
            return president;
        }

        /// <summary>
        /// Deletes a president
        /// </summary>
        /// <param name="id">Identifier of the president to be deleted</param>
        /// <returns>True if the president was deleted</returns>
        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "OK",
            Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.NotFound,
            Description = "President not found",
            Type = typeof(bool))]
        public async Task<HttpResponseMessage> Delete([FromUri] int id)
        {
            var presidents = await GetPresidents();
            var presidentList = presidents.ToList();

            if (!presidentList.Any(x => x.Id == id))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, false);
            }
            presidentList.RemoveAll(x => x.Id == id);
            await _storage.Save(presidentList, FILENAME);
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }
    }
}
