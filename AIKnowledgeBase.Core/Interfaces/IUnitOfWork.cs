using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    //hangi tabloya ihtiyacımız varsa onu getirecek anahtar metor
    IGenericRepository<T> GetRepository<T>() where T : class; //generic repository döndüren bir metot, T tipi herhangi bir sınıf olabilir, bu sayede farklı entityler için aynı unit of work üzerinden repository alabiliriz
    //veritabanına o anki tüm değişiklikleri tek seferde kaydetmeye yarar
    Task CommitAsync(); 

    //asenkron olmayan kayıt işlemi için
    void Commit();


}
