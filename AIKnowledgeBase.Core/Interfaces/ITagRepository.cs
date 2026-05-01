using AIKnowledgeBase.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Interfaces
{
    public interface  ITagRepository : IGenericRepository<Tag>
    {
        //verilen isimde bir etiket var mı bakar
        Task<Tag?> GetByNameAsync(string name);

        Task<int> SaveAsync();
    }
}
