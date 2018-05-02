using System.Collections.Generic;

namespace BasketWebPanel.BindingModels
{
    public class CategoriesBindingModel
    {
        public IEnumerable<CategoryBindingModel> Categories { get; set; }
    }
}