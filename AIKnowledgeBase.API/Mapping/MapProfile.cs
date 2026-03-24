using AutoMapper;
using AIKnowledgeBase.Core.Entities;
using AIKnowledgeBase.Core.Dtos;


public class MapProfile : Profile
{
    public MapProfile()
    {
        // User nesnesini UserDto ya , UserDto nesnesini de User nesnesine dönüştürmek için iki yönlü eşleme yapıyoruz
        CreateMap<User, UserDto>().ReverseMap(); // ReverseMap() ile iki yönlü eşleme sağlanır, yani User -> UserDto ve UserDto -> User dönüşümleri otomatik olarak yapılır
    }
}
