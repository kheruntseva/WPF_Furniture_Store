using System;
using System.Collections.Generic;

namespace mywpf
{
    public interface IRectangleItemRepository : IRepository<RectangleItem>
    {
        IEnumerable<RectangleItem> GetWithTranslations(string languageCode);
    }
}