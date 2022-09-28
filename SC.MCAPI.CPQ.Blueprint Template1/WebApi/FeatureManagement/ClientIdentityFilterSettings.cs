namespace WebApi.FeatureManagement
{
    public class ClientIdentityFilterSettings
    {
        public string[] AppIds { get; set; }

        public bool IsTestTraffic { get; set; }

        public string[] Regions { get; set; }
    }
}
