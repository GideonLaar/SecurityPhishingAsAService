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
            var PortBindings = new Dictionary<string, IList<PortBinding>>
            {
                {
                    "25/tcp", new List<PortBinding>
                    {
                        new PortBinding {HostPort = "25/tcp"}
                    }
                }
            };
            ExposedPortss.Add("25/tcp", default(EmptyStruct));

            var container = new CreateContainerParameters()
            {
                Name = "phishingasaservice_smtp",
                Image = "bytemark/smtp:latest",
                ExposedPorts = ExposedPortss,
                HostConfig = new HostConfig()
                {
                    PortBindings = PortBindings
                }
            };
            await client.Containers.CreateContainerAsync(container);
            await client.Containers.StartContainerAsync("phishingasaservice_smtp", null);
            return true;
        }
        [HttpGet("Stop")]
        public async Task<bool> Stop()
        {
           DockerClient client = new DockerClientConfiguration()
                    .CreateClient();
                var te = await client.Containers.ListContainersAsync(new ContainersListParameters()
                {
                    All = true
                });
                foreach (var con in te)
                {
                    
                    foreach (var name in con.Names)
                    {
                        //Console.WriteLine(name);
                        if (name == "/phishingasaservice_smtp")
                        {
                            await client.Containers.StopContainerAsync(con.ID, new ContainerStopParameters()
                            {
                                WaitBeforeKillSeconds = 1
                            });
                            await client.Containers.RemoveContainerAsync(con.ID, new ContainerRemoveParameters()
                            {
                                Force = true
                            });
                        }
                    }
                }

                return true;
        }

        [HttpGet("Status")]
        public async Task<string> Status()
        {
            string state = "";
            try
            {
                DockerClient client = new DockerClientConfiguration()
                    .CreateClient();
                var te = await client.Containers.ListContainersAsync(new ContainersListParameters()
                {
                    All = true
                });
                foreach (var con in te)
                {
                    
                    foreach (var name in con.Names)
                    {
                        //Console.WriteLine(name);
                        if (name == "/phishingasaservice_smtp")
                        {
                            if (con.State == "running")
                            {
                                state = "online";
                            }
                            else
                            {
                                state = "offline";
                            }
                        }
                    }
                }

                return state;
            }
            catch
            {
                return "offline";
            }
        }
    }
}