using System.Collections.Generic;

namespace Domain.Service.Configurations
{
    public class AadConfiguration
    {
        public string FirstPartyAppId { get; set; }
        public IList<string> AllowedAppIds { get; set; }
        public IList<string> AllowedTenants { get; set; }
        public IList<string> AllowedAudiences { get; set; }
    }
}
