using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artoo.Models;
using Microsoft.EntityFrameworkCore;
using Artoo.IRepositories;
using Artoo.Common;

namespace Artoo.Repositories
{
    public class EmailRuleRepository : IEmailRuleRepository
    {
        private readonly AppDbContext _appDbContext;

        public EmailRuleRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IEnumerable<EmailRule> EmailRules => _appDbContext.EmailRules;

        public IEnumerable<EmailRule> GetEmailRuleIncludePassion()
        {
            var emailRules = _appDbContext.EmailRules.Include(m => m.PassionBrand);
            return emailRules;
            //return _appDbContext.EmailRules.Include(m => m.Email).Include(m => m.PassionBrand);
        }
        public int CreateEmailRule(EmailRule emailRule)
        {
            try
            {
                _appDbContext.EmailRules.Add(emailRule);

                _appDbContext.SaveChanges();

                return emailRule.EmailRuleId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteEmailRule(int emailRuleId)
        {
            //using (TransactionScope scope = new TransactionScope())
            {
                DeleteEmailRuleDetailByEmailRuleId(emailRuleId);
                var emailRule = _appDbContext.EmailRules.SingleOrDefault(x => x.EmailRuleId == emailRuleId);
                _appDbContext.Remove(emailRule);
                _appDbContext.SaveChanges();
                //scope.Complete();
            }
        }

        public EmailRule GetEmailRuleById(int emailRuleId)
        {
            return _appDbContext.EmailRules.FirstOrDefault(p => p.EmailRuleId == emailRuleId);
        }

        public void UpdateEmailRule(EmailRule emailRule)
        {
            try
            {
                var existing = _appDbContext.EmailRules.SingleOrDefault(x => x.EmailRuleId == emailRule.EmailRuleId);

                if (existing != null)
                {
                    existing.PassionBrandId = emailRule.PassionBrandId;
                    existing.Result = emailRule.Result;
                    //existing.EmailId = emailRule.EmailId;

                    _appDbContext.EmailRules.Update(existing);

                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateEmailRuleDetail(List<EmailRuleDetail> erds)
        {
            try
            {
                _appDbContext.EmailRuleDetails.AddRange(erds);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<EmailRuleDetail> GetEmailRuleDetailByEmailRuleId(int emailRuleId)
        {
            return _appDbContext.EmailRuleDetails.Where(x => x.EmailRuleId == emailRuleId);
        }

        public void DeleteEmailRuleDetail(List<EmailRuleDetail> erds)
        {
            _appDbContext.EmailRuleDetails.RemoveRange(erds);
            _appDbContext.SaveChanges();
        }

        public void DeleteEmailRuleDetailByEmailRuleId(int emailRuleId)
        {
            var erds = _appDbContext.EmailRuleDetails.Where(x => x.EmailRuleId == emailRuleId);
            _appDbContext.EmailRuleDetails.RemoveRange(erds);
            _appDbContext.SaveChanges();
        }        
    }
}
