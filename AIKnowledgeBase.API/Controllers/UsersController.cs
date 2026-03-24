using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AIKnowledgeBase.Core.Interfaces;
using AIKnowledgeBase.Core.Entities;
using AIKnowledgeBase.Core.Dtos;
using AutoMapper;

namespace AIKnowledgeBase.API.Controllers;

[Route("api/[controller]")] //tarayıcıda api/users yazınca bu controller çalışır
[ApiController] // bu sııfın bir API oldugunu belirtir
public class UsersController : ControllerBase
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper; // AutoMapper'ı kullanarak DTO ile Entity arasında dönüşüm yapacağız

    //consturctor injection : benim çalışmama için bu iki şeye ihtiyacım var, bana verin diyorum, ben de kullanacağım
    public UsersController(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userRepository.GetAllAsync();

        var usersDto = _mapper.Map <List<UserDto>> (users);

        return Ok(CustomResponseDto<List<UserDto>>.Success(200, usersDto));
    }

    [HttpPost] //bu method post isteğiyle çalışır, yani veri eklemek için kullanılır 
    public async Task<IActionResult> CreateUser(UserDto userDto)
    {
        var user = _mapper.Map<User>(userDto); // AutoMapper a diyoruz ki userDto yu User nesnesine dönüştür, böylece DTO dan gelen veriyi Entity formatına çevirmiş oluyoruz, bu sayede repository ile çalışabiliriz

        //şifrelemeyi unutmuyoruz çünkü UserDto içinde Password alanı var ama User entity'sinde yok, onun yerine PasswordHash var, bu yüzden DTO'dan gelen şifreyi hashleyip User entity'sindeki PasswordHash alanına atıyoruz, böylece şifre güvenli bir şekilde saklanır
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
        user.CreatedDate = DateTime.Now;

        //repository aracılığıyla kullanıcıyı ekliyoruz, ama henüz veritabanına kaydetmedik, çünkü commit işlemi yapmadık, bu sayede birden fazla işlem yapıp tek seferde kaydedebiliriz
        await _userRepository.AddAsync(user);


        //UNİToFwork ile tüm değişiiklikleir sql e mühürle (işte şimdi sql e gitti)
        await _unitOfWork.CommitAsync();

        return Ok(_mapper.Map<UserDto>(user)); // Oluşturulan kullanıcıyı DTO formatında geri döndürüyoruz, böylece istemciye sadece gerekli bilgileri vermiş oluyoruz, şifre hash'i gibi hassas bilgileri gizlemiş oluyoruz
    }














}





