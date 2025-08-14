using Application.Features.Schools;
using Application.Features.Schools.Commands;
using Application.Features.Schools.Queries;
using Infrastructure.Constants;
using Infrastructure.Identity.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolsController : BaseApiController
    {
        /// <summary>
        /// For add/creating a new school
        /// </summary>
        /// <param name="schoolRequest"></param>
        /// <returns>
        /// REturns successful if the school is create else it returns an error
        /// </returns>
        [HttpPost("add")]
        [ShouldHavePermission(SchoolAction.Create, SchoolFeature.Schools)]
        public async Task<IActionResult> CreateSchoolAsync([FromBody] CreateSchoolRequest schoolRequest)
        {
            var response = await Sender.Send(new CreateSchoolCommand
            {
                CreateSchool = schoolRequest
            });

            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// For updating and existing school entity
        /// </summary>
        /// <param name="schoolRequest"></param>
        /// <returns>
        /// Resturns a successful response when an update occurs else it returns a not found error
        /// </returns>
        [HttpPut("update")]
        [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Schools)]
        public async Task<IActionResult> UpdateSchoolAsync([FromBody] UpdateSchoolRequest schoolRequest)
        {
            var response = await Sender.Send(new UpdateSchoolCommand
            {
                UpdateSchool = schoolRequest
            });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        /// <summary>
        /// Endpoint for deleting an existing school entity
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns>
        /// Resturns a successful response when an update occurs else it returns a not found error
        /// </returns>
        [HttpDelete("{schoolId}")]
        [ShouldHavePermission(SchoolAction.Delete, SchoolFeature.Schools)]
        public async Task<IActionResult> DeleteSchoolAsync(int schoolId)
        {
            var response = await Sender.Send(new DeleteSchoolCommand {SchoolId = schoolId});
            
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        /// <summary>
        /// For fetching a single schools' data using id
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns>
        /// returns a single school's data
        /// </returns>
        [HttpGet("by-id/{schoolId}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Schools)]
        public async Task<IActionResult> GetSchoolsByIdAsync(int schoolId)
        {
            var response = await Sender.Send(new GetSchoolByIdQuery
            {
                SchoolId = schoolId
            });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// For fetching a single schools' data using name
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns>
        /// returns a single school's data
        /// </returns>
        [HttpGet("by-name/{schoolId}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Schools)]
        public async Task<IActionResult> GetSchoolsByNameAsync(string name)
        {
            var response = await Sender.Send(new GetSchoolByNameQuery { Name = name });
            
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// For fetching a single schools' data 
        /// </summary>
        /// <returns>
        /// returns all school's data
        /// </returns>
        [HttpGet("all")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Schools)]
        public async Task<IActionResult> GetAllSchoolsAsync(string name)
        {
            var response = await Sender.Send(new GetSchoolsQuery());

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
