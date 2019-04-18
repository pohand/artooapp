using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.IRepositories
{
    public interface IPassionBrandRepository
    {
        int CreatePassionBrand(PassionBrand passionBrand);

        void UpdatePassionBrand(PassionBrand passionBrand);
        void DeletePassionBrand(int passionBrandId);

        IEnumerable<PassionBrand> PassionBrands { get; }

        PassionBrand GetPassionBrandById(int passionBrandId);

        int GetMaxPassionBrandId();
    }
}
