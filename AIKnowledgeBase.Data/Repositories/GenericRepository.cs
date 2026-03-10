using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIKnowledgeBase.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AIKnowledgeBase.Data.Repositories;

//ben IGenericRepository sözleşmesine uyacağım diyporuz(ınterface uygulaması)
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context; // veritabanı bağlantımız
    private readonly DbSet<T> _dbSet; // Tablomuz 

    public GenericRepository(AppDbContext context)
    {
        _context = context; // veritabanı bağlantısını alıyoruz
        _dbSet = _context.Set<T>(); // T tipindeki DbSet'i alıyoruz (örneğin, User, Document, Tag gibi)
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity); // Yeni bir varlık ekler
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync(); // Tüm varlıkları listeler halinde getirir
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity); // Var olan bir varlığı siler
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity); // Var olan bir varlığı günceller
    }

    public IQueryable<T> Where(Expression<Func<T, bool>> expression)
    {
        return _dbSet.Where(expression); // Belirli bir koşula göre varlıkları filtreler
    }
}

