using Artoo.IRepositories;
using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Repositories
{
    public class InspectionImportRepository : IInspectionImportRepository
    {
        private readonly AppDbContext _appDbContext;
        public InspectionImportRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        
    }
}
