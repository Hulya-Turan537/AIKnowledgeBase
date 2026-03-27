using AIKnowledgeBase.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using AIKnowledgeBase.Core.Entities;

namespace AIKnowledgeBase.Data.Repositories;

public class  DocumentRepository : GenericRepository<Document>, IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Document>> GetDocumentsByUserIdAsync(int userId)
    {
        //LINQ kullanarak veritabanında filtreleme yapıyoruz 
        return await _context.Documents.Where(x => x.UserId == userId).ToListAsync();
    }

}


