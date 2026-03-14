using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    //veritabanına o anki tüm değişiklikleri tek seferde kaydetmeye yarar
    Task CommitAsync(); 

    //asenkron olmayan kayıt işlemi için
    void Commit();
}
