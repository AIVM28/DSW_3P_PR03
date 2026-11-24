
using System.ComponentModel.DataAnnotations;

namespace DSW_3P_PR02.DTOs;

public class LoginDto
{
    [Required]
    public string Usuario { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}
