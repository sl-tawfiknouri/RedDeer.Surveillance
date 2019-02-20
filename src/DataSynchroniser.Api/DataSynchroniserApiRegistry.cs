using DataSynchroniser.Api.Policies;
using DataSynchroniser.Api.Policies.Interfaces;
using StructureMap;

namespace DataSynchroniser.Api
{
    public class DataSynchroniserApiRegistry : Registry
    {
        public DataSynchroniserApiRegistry()
        {
            For<IPolicyFactory>().Use<PolicyFactory>();
        }
    }
}
