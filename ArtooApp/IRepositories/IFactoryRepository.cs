using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.IRepositories
{
    public interface IFactoryRepository
    {
        IEnumerable<Factory> Factories { get; }

        int CreateFactory(Factory factory);

        void DeleteFactory(int factoryId);

        Factory GetFactoryById(int factoryId);

        void UpdateFactory(Factory factory);
    }
}
