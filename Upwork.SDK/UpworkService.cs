using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Upwork.SDK.Models;

namespace Upwork.SDK
{
    public interface IUpworkService
    {

    }

    public class UpworkService : IUpworkService
    {
        private readonly HttpClient client;

        public UpworkService(HttpClient client, ConnectionManager connectionManager)
        {
            if (client.DefaultRequestHeaders.Authorization is default)
            {
                var authToken = connectionManager.GetAuthorizationTokenAsync();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
            }

            this.client = client;

        }

        public dynamic GetJobs(JobsFilter filter)
        {
            return default;
        }


    }

}
