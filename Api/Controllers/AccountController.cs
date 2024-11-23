using System;
using System.Security.Cryptography;
using System.Text;
using Api.Data;
using Api.DTOs;
using Api.Entities;
using Api.Interfaceces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisteDto registeDto)
    {
        if (await UserExsists(registeDto.Username))
            return BadRequest("Username is taken!");


        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName = registeDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registeDto.Password)),
            PasswordSalt = hmac.Key
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return new UserDto
        {
            UserName= user.UserName,
            Token=tokenService.CreateToken(user)
        };
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");

        return new UserDto
        {
            UserName= user.UserName,
            Token=tokenService.CreateToken(user)
        };
    }
    private async Task<bool> UserExsists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
