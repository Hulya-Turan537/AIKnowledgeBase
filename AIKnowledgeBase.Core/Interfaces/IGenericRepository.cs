using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace AIKnowledgeBase.Core.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id); // T tipindeki veriyi id'ye göre asenkron olarak getirir

    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null); // T tipindeki tüm verileri listeler halinde asenkron olarak getirir

    Task AddAsync(T entity); // T tipindeki yeni bir veriyi asenkron olarak ekler
    void Update(T entity); // T tipindeki mevcut bir veriyi günceller
    void Remove(T entity); // T tipindeki mevcut bir veriyi siler
    IQueryable<T> Where(Expression<Func<T, bool>> expression); // T tipindeki verileri belirli bir koşula göre filtreler ve sorgulanabilir bir koleksiyon döndürür
}

