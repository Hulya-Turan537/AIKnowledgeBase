using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AIKnowledgeBase.Core.Dtos;

public class UserDto
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-20 karakter arası olmalıdır.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçersiz email formatı.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    public string Password { get; set; } = string.Empty;
}
