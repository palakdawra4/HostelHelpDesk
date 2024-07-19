using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : Controller
    {
        private readonly HostelComplaintsDB _DB;
        private readonly Auth _auth;
        public UserController(HostelComplaintsDB DB, Auth auth)
        {
            _DB = DB;
            _auth = auth;
        }

        [HttpGet("StudentLogin")]
        public async Task<IActionResult> StudentLogin(string Email, string Password)
        {
            User? student = await _DB.Students.FirstOrDefaultAsync(x => x.Email == Email);
            if(student == null)
            {
                return BadRequest("User Not Found");
            }
            if(!_auth.VerifyPassowrd(Password, student.PasswordHash, student.PasswordSalt))
            {
                return BadRequest("Incorrect Password");
            }
            string token = _auth.CreateToken(student.Email, "student");
            return Ok(token);
        }

        [HttpGet("WorkerLogin")]
        public async Task<IActionResult> WorkerLogin(string Email, string Password)
        {
            User? worker = await _DB.Workers.FirstOrDefaultAsync(x => x.Email == Email);
            if (worker == null)
            {
                return BadRequest("User Not Found");
            }
            if (!_auth.VerifyPassowrd(Password, worker.PasswordHash, worker.PasswordSalt))
            {
                return BadRequest("Incorrect Password");
            }
            string token = _auth.CreateToken(worker.Email, "worker");
            return Ok(token);
        }

        [HttpGet("CaretakerLogin")]
        public async Task<IActionResult> CaretakerLogin(string Email, string Password)
        {
            User? caretaker = await _DB.Caretakers.FirstOrDefaultAsync(x => x.Email == Email);
            if (caretaker == null)
            {
                return BadRequest("User Not Found");
            }
            if (!_auth.VerifyPassowrd(Password, caretaker.PasswordHash, caretaker.PasswordSalt))
            {
                return BadRequest("Incorrect Password");
            }
            string token = _auth.CreateToken(caretaker.Email, "caretaker");
            return Ok(token);
        }

        /*[HttpPost("AdminLogin")]
        public async Task<IActionResult> AdminLogin(string username, string password)
        {
            Admin? admin = await _DB.Admins.FirstOrDefaultAsync(x => x.Username == username);
            if (admin == null)
            {
                return BadRequest("Invaild Crediantials");
            }
            if (!_auth.VerifyPassowrd(password, admin.PasswordHash, admin.PasswordSalt))
            {
                return BadRequest("Invaid Crediantials");
            }
            string token = _auth.CreateToken(admin.Username, "admin");
            *//*if(_auth.IsTokenValid(token)) 
            { 
                return Ok(token);
            }
            return BadRequest("Token Expired");*//*
            return Ok(token);
        }*/

        [HttpPost("AdminLogin")]
        public async Task<IActionResult> AdminLogin(LoginDTO request)
        {
            Admin? admin = await _DB.Admins.FirstOrDefaultAsync(x => x.Username == request.username);
            if (admin == null)
            {
                return BadRequest("Invaild Crediantials");
            }
            if (!_auth.VerifyPassowrd(request.password, admin.PasswordHash, admin.PasswordSalt))
            {
                return BadRequest("Invaid Crediantials");
            }
            string token = _auth.CreateToken(admin.Username, "admin");
            /*if(_auth.IsTokenValid(token)) 
            { 
                return Ok(token);
            }
            return BadRequest("Token Expired");*/
            return Ok(new { token });
        }
    }
}
