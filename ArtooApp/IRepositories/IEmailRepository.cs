using Artoo.Common;
using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.IRepositories
{
    public interface IEmailRepository
    {
        void CreateEmail(Email email);
        void UpdateEmail(Email email);
        void DeleteEmail(int emailId);
        IEnumerable<Email> Emails { get; }
        Email GetEmailById(int emailId);
        IEnumerable<Email> GetEmailByIdList(List<int> emailIds);
        IEnumerable<Email> GetEmailByBrandResult(int brandId, InspectionResultEnum result);
    }
}
