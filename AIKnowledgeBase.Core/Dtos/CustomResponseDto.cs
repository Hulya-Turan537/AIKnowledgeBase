using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Dtos;

public class CustomResponseDto<T>
{
    public T Data { get; set; } // döneceğimiz asıl veri (liste veya tekil nesne olabilir)
    public List<string> Errors { get; set; } // hata mesajlarını tutacak liste, eğer işlem başarılıysa bu liste boş kalır, ama bir hata varsa burada hata mesajları tutulur
    public int StatusCode { get; set; } // HTTP durum kodu

    //Başarılı durumlar için yardımcı metotlar (static Factory Methodlar)
    public static CustomResponseDto<T> Success(int statusCode, T dataContent)
    {
        return new CustomResponseDto<T> { Data = dataContent, StatusCode = statusCode };
    }

    public static CustomResponseDto<T> Fail(int statusCode, List<string> errors)
    {
        return new CustomResponseDto<T> { StatusCode = statusCode, Errors = errors };
    }
}

