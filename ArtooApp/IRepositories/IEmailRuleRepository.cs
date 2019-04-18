using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.IRepositories
{
    public interface IEmailRuleRepository
    {
        int CreateEmailRule(EmailRule email);
        void UpdateEmailRule(EmailRule email);
        void DeleteEmailRule(int emailRuleId);
        IEnumerable<EmailRule> EmailRules { get; }
        EmailRule GetEmailRuleById(int emailRuleId);
        IEnumerable<EmailRule> GetEmailRuleIncludePassion();
        void CreateEmailRuleDetail(List<EmailRuleDetail> erds);
        IEnumerable<EmailRuleDetail> GetEmailRuleDetailByEmailRuleId(int emailRuleId);
        void DeleteEmailRuleDetail(List<EmailRuleDetail> erds);
        void DeleteEmailRuleDetailByEmailRuleId(int emailRuleId);
    }
}
