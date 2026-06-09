using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Todo.Data;
using Todo.DTO;
using Todo.Models;
using Todo.Services;
using System.Security.Claims;
using Todo.DTOs;

namespace Todo.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController( ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync( x => x.Email == dto.Email);

            if(existingUser != null)
            {
                return BadRequest(
                    new
                    {
                        message = "Email already exist"
                    }
                );
            }

            var user = new User {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(
                new
                {
                    message = "Registered user successfully.",
                    success = true,
                    Name = user.Name,
                    Email = user.Email
                }
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto) {
            var user = await _context.Users
                .FirstOrDefaultAsync(
                    x =>
                    x.Email == dto.Email &&
                    x.Password == dto.Password
                );
            if (user == null){
                return Unauthorized(
                    new
                    {
                        success = false,
                        message = "Invalid email or password"
                    }
                );
            }

            var token = _jwtService.GenerateToken(user);
            return Ok(
                new {
                    success = true,
                    message = "Login successful",

                    token = token,

                    user = new {
                        id = user.Id,
                        name = user.Name,
                        email = user.Email
                    }
                }
            );
        }
    }
}