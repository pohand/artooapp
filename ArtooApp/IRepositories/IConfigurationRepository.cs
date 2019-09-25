using Artoo.Models;
using System.Collections.Generic;

namespace Artoo.IRepositories
{
    public interface IConfigurationRepository
    {
        void CreateConfiguration(ArtooConfiguration configuration);
        void UpdateCofigurationByName(ArtooConfiguration configuration);
        void DeleteConfiguration(int configurationId);
        void DeleteConfigurationByName(string Name);
        IEnumerable<ArtooConfiguration> ArtooConfigurations { get; }
        ArtooConfiguration GetConfigurationByName(string name);
        bool GetConfigurationStatusByName(string name);
    }
}
