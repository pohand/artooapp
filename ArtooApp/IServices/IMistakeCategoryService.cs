using Artoo.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Artoo.IServices
{
    public interface IMistakeCategoryService
    {
        List<SelectListItem> MistakeCategories();
        List<SelectListItem> MistakeCategoriesByType(MistakeEnum mistakeType);
    }
}
