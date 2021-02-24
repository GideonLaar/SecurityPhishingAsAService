using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;

namespace SecurityPhishingAsAService.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class EngineController : ControllerBase
    {
        [HttpGet("Start")]
        public async Task<bool> Start()
        {
            DockerClient client = new DockerClientConfiguration()
                .CreateClient();
            var progress = new Progress<JSONMessage>();
            var image = new ImagesCreateParameters()
            {
                FromImage = "bytemark/smtp",
                Tag = "latest",
                FromSrc = null,
                Repo = null,
            };
            var auth = new AuthConfig();
            await client.Images.CreateImageAsync(image,auth,progress);
            var ExposedPortss = new Dictionary<string,EmptyStruct>();
            var PortsBind = new Dictionary<string, IList<PortBinding>>();
            PortsBind.Add("25/tcp",null);
            ExposedPortss.Add("25/tcp", default(EmptyStruct));

            var container = new CreateContainerParameters()
            {
                Name = "phishingasaservice_smtp",
                Image = "bytemark/smtp:latest",
                ExposedPorts = ExposedPortss,
                HostConfig = new HostConfig()
                {
                    PortBindings = PortsBind
                }
            };
            await client.Containers.CreateContainerAsync(container);
            await client.Containers.StartContainerAsync("phishingasaservice_smtp", null);
            return true;
        }
        [HttpGet("Stop")]
        public async Task<bool> Stop()
        {
            try
            {
                DockerClient client = new DockerClientConfiguration()
                    .CreateClient();
                await client.Containers.StopContainerAsync("phishingasaservice_smtp", null);
                await client.Containers.RemoveContainerAsync("phishingasaservice_smtp", null);
                return true;
            }
            catch
            {
                return false;
            }
           
        }
    }
}