using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.IRepositories
{
    public interface ITechManagerRepository
    {
        IEnumerable<TechManager> TechManagers { get; }

        int CreateTechManager(TechManager techManager);

        void DeleteTechManager(int techManagerId);

        TechManager GetTechManagerById(int techManagerId);

        void UpdateTechManager(TechManager techManager);
    }
}
