using AIKnowledgeBase.Core.Entities;
using AIKnowledgeBase.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Data.Repositories
{
    //generic reositoryden miras alarak temel crud işlemlerini hazır aldık
    public class  TagRepository : GenericRepository<Tag>, ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            //verilen isimde bir etiket var mı bakar .ToLower() ile büyük küçük harf duyarlılığını kaldırıyoruz
            return await _context.Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
    
}
