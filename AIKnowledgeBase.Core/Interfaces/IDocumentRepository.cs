using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using AIKnowledgeBase.Core.Entities;
namespace AIKnowledgeBase.Core.Interfaces;

//IGenericRepositroy den miras alıyoruz boylece standart metotlar geliyor, ayrıca bu interface sadece Document entitysi için geçerli olacak, diğer entityler için ayrı repositoryler oluşturacağız, böylece her repository kendi entitysiyle ilgili özel metotlara sahip olabilir, ama aynı zamanda ortak metotları da kullanabiliriz
public interface IDocumentRepository : IGenericRepository<Document>
{
    Task<List<Document>> GetDocumentsByUserIdAsync(int userId);
}
