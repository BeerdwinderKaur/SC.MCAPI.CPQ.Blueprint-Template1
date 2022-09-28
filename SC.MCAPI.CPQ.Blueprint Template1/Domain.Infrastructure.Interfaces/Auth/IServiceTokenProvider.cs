using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure.Interfaces.Auth
{
    public interface IServiceTokenProvider
    {
        /// <summary>
        /// Gets the token for the client app for the resource id.
        /// </summary>
        /// <param name="clientId">AAD clientId</param>
        /// <param name="authority"></param>
        /// <param name="resourceId">Id of the resource for which access token is requested.</param>
        /// <returns>Access token for the resource.</returns>
        Task<string> GetTokenForClient(string clientId, string authority, string resourceId);
    }
}
