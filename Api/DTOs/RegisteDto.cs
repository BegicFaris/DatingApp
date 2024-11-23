using System;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

public class RegisteDto
{
    [Required]
    public required string Username {get;set;}

    [Required]

    public required string  Password { get; set; }       

}
