using System;
using System.Collections.Generic;
using System.Linq;
using Artoo.IRepositories;
using Artoo.Models;

namespace Artoo.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly AppDbContext _appDbContext;
        public ConfigurationRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IEnumerable<ArtooConfiguration> ArtooConfigurations => _appDbContext.ArtooConfigurations;

        public void CreateConfiguration(ArtooConfiguration configuration)
        {
            var existingConfiguration = _appDbContext.ArtooConfigurations.FirstOrDefault(p => p.Name == configuration.Name);
            if (existingConfiguration == null)
            {
                configuration.DateRegister = DateTime.Now;
                _appDbContext.ArtooConfigurations.Add(configuration);
                _appDbContext.SaveChanges();
            }
        }

        public void DeleteConfiguration(int configurationId)
        {
            var configuration = _appDbContext.ArtooConfigurations.SingleOrDefault(x => x.ArtooConfigurationId == configurationId);
            if (configuration != null)
            {
                _appDbContext.Remove(configuration);
                _appDbContext.SaveChanges();
            }
        }

        public void DeleteConfigurationByName(string name)
        {
            var configuration = _appDbContext.ArtooConfigurations.SingleOrDefault(x => x.Name == name);
            if (configuration != null)
            {
                DeleteConfiguration(configuration.ArtooConfigurationId);
            }
        }

        public ArtooConfiguration GetConfigurationByName(string name)
        {
            return _appDbContext.ArtooConfigurations.FirstOrDefault(p => p.Name == name);
        }

        public bool GetConfigurationStatusByName(string name)
        {
            ArtooConfiguration config = _appDbContext.ArtooConfigurations.FirstOrDefault(p => p.Name == name);
            if (config == null)
                return false;
            else
                return config.Status;
        }

        public void UpdateCofigurationByName(ArtooConfiguration configuration)
        {
            var existing = _appDbContext.ArtooConfigurations.SingleOrDefault(x => x.Name == configuration.Name);

            if (existing != null)
            {
                existing.Description = configuration.Description;
                existing.Scope = configuration.Scope;
                existing.Status = configuration.Status;
                _appDbContext.ArtooConfigurations.Update(existing);
                _appDbContext.SaveChanges();
            }
            else
            {
                CreateConfiguration(configuration);
            }
        }
    }
}
