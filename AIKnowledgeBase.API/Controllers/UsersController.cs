using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AIKnowledgeBase.Core.Interfaces;
using AIKnowledgeBase.Core.Entities;

namespace AIKnowledgeBase.API.Controllers;

[Route("api/[controller]")] //tarayıcıda api/users yazınca bu controller çalışır
[ApiController] // bu sııfın bir API oldugunu belirtir
public class UsersController : ControllerBase
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    //consturctor injection : benim çalışmama için bu iki şeye ihtiyacım var, bana verin diyorum, ben de kullanacağım
    public UsersController(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet] // Sadece /api/users yazınca burası çalışır
    public async Task<IActionResult> GetAll()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users);
    }

    [HttpPost] //bu method post isteğiyle çalışır, yani veri eklemek için kullanılır
    
    public async Task<IActionResult> CreateUser(User user)
    {
        //repository aracılığıyla kullanıcıyı ekliyoruz, ama henüz veritabanına kaydetmedik, çünkü commit işlemi yapmadık, bu sayede birden fazla işlem yapıp tek seferde kaydedebiliriz
        await _userRepository.AddAsync(user);


        //UNİToFwork ile tüm değişiiklikleir sql e mühürle (işte şimdi sql e gitti)
        await _unitOfWork.CommitAsync();

        return Ok(user); //eklenen kullanıcıyı döndürüyoruz, istek başarılı olduysa 200 OK status kodu ile birlikte kullanıcıyı döndürür
    }
}

