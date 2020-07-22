using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TandemAPI.Helpers;
using TandemAPI.Models;
using TandemAPI.ViewModels;

namespace TandemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly CosmosClient cosmosClient;
        private readonly Container cosmosContainer;
        private readonly IMapper mapper;
        private readonly ILogger<PersonController> logger;

        public PersonController(CosmosClient cosmosClient, IMapper mapper, ILogger<PersonController> logger)
        {
            this.cosmosClient = cosmosClient;
            this.cosmosContainer = this.cosmosClient.GetContainer("tandemdb", "Persons");
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Gets specific Person Items.
        /// </summary>
        [Produces("application/json")]
        [HttpGet("{queryValue}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string queryValue)
        {
            string sqlQueryText = "SELECT * FROM c WHERE ";

            if (queryValue.IsGuidValid())
            {
                sqlQueryText += string.Format("c.id = '{0}'", queryValue);
            }
            else if (queryValue.IsEmailValid())
            {
                sqlQueryText += string.Format("c.emailAddress = '{0}'", queryValue);
            }
            else
            {
                return BadRequest("Failed to retrieve users: Invalid queryValue");
            }

            try
            {
                QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
                FeedIterator<Person> queryResultSetIterator = cosmosContainer.GetItemQueryIterator<Person>(queryDefinition);

                List<PersonViewModel> personsViewModel = new List<PersonViewModel>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<Person> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (Person person in currentResultSet)
                    {
                        var personViewModel = mapper.Map<PersonViewModel>(person);
                        personsViewModel.Add(personViewModel);
                    }
                }

                if (personsViewModel.Count == 0)
                {
                    return NotFound();
                }

                return Ok(personsViewModel);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to retrieve users: {0}", ex.Message);
                return BadRequest("Failed to retrieve users");
            }
        }

        /// <summary>
        /// Creates a person.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Add(Person model)
        {
            try
            {
                model.UserId = Guid.NewGuid().ToString();

                await this.cosmosContainer.CreateItemAsync(model, new PartitionKey(model.EmailAddress));

                return CreatedAtAction(nameof(Get), new { queryValue = model.UserId }, model);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to save person: {0}", ex.Message);
                return BadRequest("Failed to save person");
            }
        }

        /// <summary>
        /// Delete a person.
        /// </summary>
        [HttpDelete("{id}/{emailAddress}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id, string emailAddress)
        {
            try
            {
                ItemResponse<Person> personResponse = await this.cosmosContainer.DeleteItemAsync<Person>(id, new PartitionKey(emailAddress));

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete person: {0}", ex.Message);
                return BadRequest("Failed to delete person");
            }
        }
    }
}
