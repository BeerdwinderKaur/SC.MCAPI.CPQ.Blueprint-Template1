using Domain.Model;
using Domain.Service.Interface;
using MCAPI.Shared.Services.Models.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Model;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {
        private readonly ILogger<HelloController> _logger;
        private readonly IHelloDomainService _helloDomainService;
        private readonly IFeatureManager _featureManager;

        public HelloController(ILogger<HelloController> logger,
            IHelloDomainService helloDomainService,
            IFeatureManager featureManager)
        {
            _logger = logger;
            _helloDomainService = helloDomainService;
            _featureManager = featureManager;
        }

        [HttpGet]
        [Route("test")]
        public void Testing()
        {
            Console.WriteLine("Hello World");
        }

        [HttpGet]
        [Route("{homeId}")]
        public async Task<IActionResult> Get(string homeId)
        {
            var greetings = new GreetingsModel()
            { Id = homeId, Name = Request.HttpContext.Items["Commerce.CallerApi"]?.ToString() };

            var myNewFeatureEnabled = await _featureManager.IsEnabledAsync("MyNewFeature");

            if (myNewFeatureEnabled)
            {
                _helloDomainService.GetNewFeatureGreetings(greetings);
            }
            else
            {
                _helloDomainService.GetGreetings(greetings);
            }


            _logger.LogInformation(greetings.Greetings);
            return await Task.FromResult(Ok(greetings));
        }

        [HttpPost]
        [Route("")]
        public IActionResult PostWithError([FromBody] HomeRequest homeRequest)
        {
            // If you want to throw a Validation or 400 based error
            // You can also return Aggregate of validation and return an Aggregate error
            throw new ValidationError(1234, "FirstName is missing").WithProperty(nameof(homeRequest.FirstName));
        }
    }
}
