using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIKnowledgeBase.Core.Interfaces;

namespace AIKnowledgeBase.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context; // veritabanı bağlantısı
    public UnitOfWork(AppDbContext context)
    {
        _context = context; // veritabanı bağlantısını alıyoruz
    }

    //tek bir tuşla her şeyi veritabanına kaydeder
    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync(); 
    }

    //askenron olmayan kayıt işlemi için
    public void Commit()
    {
        _context.SaveChanges();
    }

    //IDisposableden gelen görev: iş bitince bağlantıyı kapatır, kaynakları serbest bırakır
    public void Dispose()
    {
        _context.Dispose();
    }
}
