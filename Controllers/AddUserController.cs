using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AddUserController : Controller
    {
        private readonly HostelComplaintsDB _DB;
        private readonly Auth _auth;

        public AddUserController(HostelComplaintsDB DB, Auth auth)
        {
            _DB = DB;
            _auth = auth;
        }

        [HttpGet("GetAllStudents"), Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllStudents()
        {
            return Ok(await _DB.Students.ToListAsync());
        }
        
        [HttpGet("GetAllWorers"), Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllWorers()
        {
            return Ok(await _DB.Workers.ToListAsync());
        }

        [HttpGet("GetAllCaretakers"), Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllCaretakers()
        {
            return Ok(await _DB.Caretakers.ToListAsync());
        }
        
        [HttpGet("GetStudent"), Authorize(Roles = "admin")]
        public async Task<IActionResult> GetStudent(int rollNo)
        {
            var result = await _DB.Students.FirstOrDefaultAsync(x => x.RollNo == rollNo);
            return Ok(result);
        }

        [HttpGet("GetWorker"), Authorize(Roles = "admin")]
        public async Task<IActionResult> GetWorker(string phoneNo)
        {
            var result = await _DB.Workers.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNo);
            return Ok(result);
        }

        [HttpGet("GetCaretaker"), Authorize(Roles = "admin")]
        public async Task<IActionResult> GetCaretaker(string phoneNo)
        {
            var result = await _DB.Caretakers.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNo);
            return Ok(result);
        }

        [HttpGet("GetAllHostel"), Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllHostel()
        {
            var result = await _DB.Hostels.ToListAsync();
            return Ok(result);
        }

        [HttpGet("GetAllRooom"), Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllRoom()
        {
            var result = await _DB.Rooms.ToListAsync();
            return Ok(result);
        }

        [HttpGet("GetRoom"), Authorize(Roles = "admin")]
        public async Task<IActionResult> GetHostelRooms(Guid HostelId)
        {
            var result = await _DB.Rooms.Where(h => h.HostelId == HostelId).ToListAsync();
            return Ok(result);
        }

        [HttpPost("UploadHostels"), Authorize(Roles = "admin") ]
        public async Task<IActionResult> UploadHostels(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is null or empty");

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<AddHostel>(); // Assuming AddHostel matches the CSV structure

                    foreach (var record in records)
                    {
                        var hostel = new Hostel
                        {
                            Id = Guid.NewGuid(),
                            HostelName = record.HostelName
                            // Set other properties here as needed
                        };

                        await _DB.Hostels.AddAsync(hostel);
                        
                    }

                    await _DB.SaveChangesAsync();
                }

                return Ok("Hostel Data uploaded successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("AddHostel"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddHostel([FromBody] AddHostel addHostel)
        {
            var request = new Hostel()
            {
                Id = Guid.NewGuid(),
                HostelName = addHostel.HostelName
            };
            await _DB.Hostels.AddAsync(request);
            await _DB.SaveChangesAsync();
            return Ok(request);
        }

        [HttpPost("UploadRooms"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UploadRooms(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is null or empty");

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<AddRoomDTO>(); // Assuming AddHostel matches the CSV structure

                    foreach (var record in records)
                    {
                        var hostel = await _DB.Hostels.Where(x => x.HostelName == record.HostelName).ToListAsync();
                        var room = new Room
                        {
                            Id = Guid.NewGuid(),
                            RoomNo = record.RoomNo,
                            HostelId = hostel.First().Id,
                            // Set other properties here as needed
                        };

                        await _DB.Rooms.AddAsync(room);

                    }

                    await _DB.SaveChangesAsync();
                }

                return Ok("Rooms Data uploaded successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("AddRoom"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddRoom([FromBody] AddRoom addRoom)
        {
            var request = new Room()
            {
                Id = Guid.NewGuid(),
                RoomNo = addRoom.RoomNo,
                HostelId = addRoom.HostelId
            };
            await _DB.Rooms.AddAsync(request);
            await _DB.SaveChangesAsync();
            return Ok(request);
        }

        [HttpPost("UploadStudents"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UploadStudents(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is null or empty");

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<AddStudentsDTO>(); // Assuming AddHostel matches the CSV structure

                    foreach (var record in records)
                    {
                        var hostel = await _DB.Hostels.Where(x => x.HostelName == record.HostelName).ToListAsync();
                        var room = await _DB.Rooms.Where(x => x.RoomNo == record.RoomNo).ToListAsync();
                        _auth.CreatePasswordHash(record.Password, out byte[] passwordHash, out byte[] passwordSalt);
                        var stu = new Student
                        {
                            Id = Guid.NewGuid(),
                            FirstName = record.FirstName,
                            LastName = record.LastName,
                            Email = record.Email,
                            PhoneNumber = record.PhoneNumber,
                            RollNo = record.RollNo,
                            HostelId = hostel.First().Id,
                            RoomId = room.First().Id,
                            PasswordHash = passwordHash,
                            PasswordSalt = passwordSalt
                        };

                        await _DB.Students.AddAsync(stu);

                    }

                    await _DB.SaveChangesAsync();
                }

                return Ok("Students Data uploaded successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("AddStudent"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddStudent([FromBody] AddStudent addStudent)
        {
            _auth.CreatePasswordHash(addStudent.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var request = new Student()
            {
                Id = Guid.NewGuid(),
                FirstName = addStudent.FirstName,
                LastName = addStudent.LastName,
                Email = addStudent.Email,
                PhoneNumber = addStudent.PhoneNumber,
                RollNo = addStudent.RollNo,
                HostelId=addStudent.HostelId,
                RoomId=addStudent.RoomId,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt

            };
            await _DB.Students.AddAsync(request);
            await _DB.SaveChangesAsync();
            return Ok(request);
        }

        [HttpPost("UploadWorker"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UploadWorker(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is null or empty");

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<AddWorker>(); // Assuming AddHostel matches the CSV structure

                    foreach (var record in records)
                    {
                        _auth.CreatePasswordHash(record.Password, out byte[] passwordHash, out byte[] passwordSalt);
                        var worker = new Worker
                        {
                            Id = Guid.NewGuid(),
                            Email = record.Email,
                            FirstName = record.FirstName,
                            LastName = record.LastName,
                            PhoneNumber = record.PhoneNumber,
                            Type = record.Type,
                            PasswordHash = passwordHash,
                            PasswordSalt = passwordSalt
                        };

                        await _DB.Workers.AddAsync(worker);

                    }

                    await _DB.SaveChangesAsync();
                }

                return Ok("Worker Data uploaded successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("AddWorker"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddWorker([FromBody] AddWorker addWorker)
        {
            _auth.CreatePasswordHash(addWorker.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var request = new Worker()
            {
                Id = Guid.NewGuid(),
                Email = addWorker.Email,
                FirstName = addWorker.FirstName,
                LastName = addWorker.LastName,
                PhoneNumber=addWorker.PhoneNumber,
                Type   = addWorker.Type,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            await _DB.Workers.AddAsync(request);
            await _DB.SaveChangesAsync();
            return Ok(request);
        }

        [HttpPost("UploadCaretaker"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UploadCaretaker(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is null or empty");

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<AddCaretakerDTO>(); // Assuming AddHostel matches the CSV structure

                    foreach (var record in records)
                    {
                        var hostel = await _DB.Hostels.Where(x => x.HostelName == record.HostelName).ToListAsync();
                        _auth.CreatePasswordHash(record.Password, out byte[] passwordHash, out byte[] passwordSalt);
                        var caretaker = new Caretaker
                        {
                            Id = Guid.NewGuid(),
                            FirstName = record.FirstName,
                            LastName = record.LastName,
                            Email = record.Email,
                            PhoneNumber = record.PhoneNumber,
                            HostelId = hostel.First().Id,
                            PasswordHash = passwordHash,
                            PasswordSalt = passwordSalt
                        };

                        await _DB.Caretakers.AddAsync(caretaker);

                    }

                    await _DB.SaveChangesAsync();
                }

                return Ok("Caretakers Data uploaded successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("AddCaretaker"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddCaretaker([FromBody] AddCaretaker addCaretaker)
        {
            _auth.CreatePasswordHash(addCaretaker.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var request = new Caretaker()
            {
                Id = Guid.NewGuid(),
                Email = addCaretaker.Email,
                FirstName = addCaretaker.FirstName,
                LastName = addCaretaker.LastName,
                PhoneNumber = addCaretaker.PhoneNumber,
                HostelId = addCaretaker.HostelId,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            await _DB.Caretakers.AddAsync(request);
            await _DB.SaveChangesAsync();
            return Ok(request);
        }

        [HttpPost("AddAdmin")]
        public async Task<IActionResult> AddAdmin(string username, string password)
        {
            _auth.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var request = new Admin()
            {
                Username = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            await _DB.Admins.AddAsync(request);
            await _DB.SaveChangesAsync();
            return Ok("New Admin Added");
        }
    }
}
