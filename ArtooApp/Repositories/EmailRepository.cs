using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artoo.Models;
using Artoo.IRepositories;
using Artoo.Common;

namespace Artoo.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly AppDbContext _appDbContext;
        public EmailRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IEnumerable<Email> Emails => _appDbContext.Emails;

        public void CreateEmail(Email email)
        {
            var existingEmail = _appDbContext.Emails.FirstOrDefault(p => p.EmailAddress == email.EmailAddress);
            if (existingEmail == null)
            {
                email.DateRegister = DateTime.Now;

                _appDbContext.Emails.Add(email);

                _appDbContext.SaveChanges();
            }
        }

        public void DeleteEmail(int emailId)
        {
            var email = _appDbContext.Emails.SingleOrDefault(x => x.EmailId == emailId);
            _appDbContext.Remove(email);
            _appDbContext.SaveChanges();
        }

        public Email GetEmailById(int emailId)
        {
            return _appDbContext.Emails.FirstOrDefault(p => p.EmailId == emailId);
        }

        public void UpdateEmail(Email email)
        {
            var existing = _appDbContext.Emails.SingleOrDefault(x => x.EmailId == email.EmailId);

            if (existing != null)
            {
                existing.Description = email.Description;
                existing.EmailAddress = email.EmailAddress;
                existing.FirstName = email.FirstName;
                existing.LastName = email.LastName;

                _appDbContext.Emails.Update(existing);

                _appDbContext.SaveChanges();
            }
        }

        public IEnumerable<Email> GetEmailByIdList(List<int> emailIds)
        {
            return _appDbContext.Emails.Where(x => emailIds.Contains(x.EmailId));
        }

        public IEnumerable<Email> GetEmailByBrandResult(int brandId, InspectionResultEnum result)
        {
            var emailRule = _appDbContext.EmailRules.Where(x => x.PassionBrandId == brandId && x.Result == ((int)(InspectionResultEnum)result));
            var emailIds = _appDbContext.EmailRuleDetails.Where(x => emailRule.Select(p => p.EmailRuleId).Contains(x.EmailRuleId));
            var emailList = _appDbContext.Emails.Where(x => emailIds.Select(p => p.EmailId).Contains(x.EmailId));

            return emailList;
        }
    }
}
