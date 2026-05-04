using AIKnowledgeBase.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AIKnowledgeBase.Core.Interfaces;
using AIKnowledgeBase.Core.Entities;

namespace AIKnowledgeBase.Data.Repositories;

public class  DocumentRepository : GenericRepository<Document>, IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }


    //GenericRepsitorydeki GetByIdAsynci eziyoruz ki ınclude ekleyebilelim
    public override async Task<Document?> GetByIdAsync(int id)
    {
        //return await _context.Documents
        //    .Include(d => d.DocumentTags)
        //    .FirstOrDefaultAsync(x => x.Id == id);

        return await _context.Documents
    .Include(d => d.DocumentTags)      // Önce köprü tabloyu dahil et
        .ThenInclude(dt => dt.Tag)    // Sonra köprüden gerçek etiket tablosuna geç
    .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Document>> GetDocumentsByUserIdAsync(int userId)
    {
        //LINQ kullanarak veritabanında filtreleme yapıyoruz 
        return await _context.Documents
            .Include(d => d.DocumentTags)
            .Where(x => x.UserId == userId).ToListAsync();

        
    }

}


