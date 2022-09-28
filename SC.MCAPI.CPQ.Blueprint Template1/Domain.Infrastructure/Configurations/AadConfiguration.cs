using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure.Configurations
{
    public class AadConfiguration
    {
        /// <summary>
        /// Use this ID where downstream clients allow only PPE authority in INT. Ex : Office, Billing Group..etc.
        /// </summary>
        public string FirstPartyAppIdWithPpeSupport { get; set; }
        /// <summary>
        /// Use this ID where downstream clients allow only PROD authority in INT. Ex : BigCat
        /// </summary>
        public string FirstPartyProdAppIdWithProdOnlySupport { get; set; }
        public IList<string> AllowedAppIds { get; set; }
        public IList<string> AllowedTenants { get; set; }
        public IList<string> AllowedAudiences { get; set; }

        /// <summary>
        /// This Authority will always have PROD AAD tenant for all our environments
        /// </summary>
        public string FirstPartyProdOnlyAuthoritySupport { get; set; }
        /// <summary>
        /// This Authority will always have PPE AAD  in our INT and PROD for PROD environment
        /// </summary>
        public string FirstPartyPpeAuthoritySupport { get; set; }

        /// <summary>
        /// App Id used for BigCat hydration end point to get AAD token to access Big cat service
        /// </summary>
        public string ThirdPartyAppId { get; set; }

        /// <summary>
        /// Big cat resource id for hydration end point to get AAD token to access Big cat service
        /// </summary>
        public string ThirdPartyAuthority { get; set; }
    }
}
