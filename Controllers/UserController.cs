using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApplication1.Interface;
using WebApplication1.Models;
using WebApplication1.Service;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(MongoDbService dbService, ICloudnaryInterface cloudnary, IMail mail) : ControllerBase
{
    private readonly MongoDbService dbService = dbService;
    private readonly ICloudnaryInterface _cloudnary = cloudnary;
    private readonly IMail _mail = mail;


    [HttpPost("create")]
    public async Task<IActionResult> Register(User user)
    {
        try
        {


            if (string.IsNullOrWhiteSpace(user.Username) ||
            string.IsNullOrWhiteSpace(user.Email) ||
            string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest(new { message = "All creditials required" });
            }
            var finduser = await dbService.Users.Find(uemail => uemail.Email.ToString() == user.Email).FirstOrDefaultAsync();
            if (finduser != null)
            {
                return BadRequest(new { message = "User already exists" });
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.DateCreated = DateTime.UtcNow;

            await dbService.Users.InsertOneAsync(user);
            await _mail.SendEmailAsync(user.Email, "Welcome to our website", "welcome to userlogin", false);
            return Ok(new { message = "User created suceefully" });
        }
        catch (Exception)
        {

            return NotFound(new { message = "Server error" });
        }


    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login request, IToken token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "All creditials required" });
            }

            var finduser = await dbService.Users.Find(u => u.Email.ToString() == request.Email).FirstOrDefaultAsync();
            if (finduser == null)
            {
                return BadRequest(new { message = "User not found!" });
            }
            var Verify = BCrypt.Net.BCrypt.Verify(request.Password, finduser.Password);

            if (Verify)
            {
                var cook = token.CreateToken(finduser.Id.ToString(), finduser.Email, finduser.Username);
                var idValue = finduser.Id.ToString(); // Converts ObjectId to string
                return Ok(new { message = "Logged in sucessfully", payload = cook, username= finduser.Username, id= idValue  });
            }
            else
            {
                return BadRequest(new { message = "Incorrect password" });
            }
        }
        catch (Exception)
        {

            return NotFound(new { message = "Server error" });
        }
    }


    [HttpPost("forget/password")]

    public async Task<IActionResult> Forgetpass([FromBody] Email request)
    {
        try
        {
            var finduser = await dbService.Users.Find(u => u.Email.ToString() == request.UserEmail).FirstOrDefaultAsync();

            if (finduser == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            var random = new Random();
            var plainOtp = random.Next(1000, 9999).ToString();

            // Hash the OTP using BCrypt
            var hashedOtp = BCrypt.Net.BCrypt.HashPassword(plainOtp);

            // Save hashed OTP and expiry to the database
            finduser.Otp = hashedOtp;
            finduser.OtpExpiry = DateTime.UtcNow.AddMinutes(3); // OTP valid for 3 minutes
            await dbService.Users.ReplaceOneAsync(u => u.Id == finduser.Id, finduser);

            // Send the plain OTP to the user's email
            await _mail.SendEmailAsync(request.UserEmail, "OTP for Password Reset", $"Your OTP is {plainOtp}", false);
            return Ok(new { message = "OTP sent suceesfully" });
        }
        catch (Exception ex)
        {

            throw new InvalidOperationException("Cant send otp", ex);
        }
    }


    [HttpPost("verify/{id}")]
    public async Task<IActionResult> Verifyotp(string id, [FromBody] OTP request)
    {
        try
        {
            var finduser = await dbService.Users.Find(u => u.Id.ToString() == id).FirstOrDefaultAsync();
            if (finduser == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            if (finduser.Otp == null || finduser.OtpExpiry == null || finduser.OtpExpiry <= DateTime.UtcNow)
            {
                return Ok(new { message = "OTP expires or doesnt match" });
            }

            var isotpvalid = BCrypt.Net.BCrypt.Verify(request.Otp.ToString(), finduser.Otp);

            if (!isotpvalid)
            {
                return BadRequest(new { message = "Invalid OTP" });
            }
            if (request.Pass != request.Confpass)
            {
                return BadRequest(new { message = "Password and confirm password dont match" });
            }

            finduser.Password = BCrypt.Net.BCrypt.HashPassword(request.Pass);
            finduser.Otp = null;
            finduser.OtpExpiry = null;
            request.DateModified = DateTime.UtcNow;
            await dbService.Users.ReplaceOneAsync(u => u.Id.ToString() == id, finduser);
            return Ok(new { message = "Password reset sucessfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpPut("edit/{id}")]
    public async Task<IActionResult> Edit(string id, User user)
    {
        var finduser = await dbService.Users.Find(u => u.Id.ToString() == id).FirstOrDefaultAsync();
        if (finduser == null)
        {
            return NotFound(new { message = "User not found" });
        }

        finduser.Username = user.Username ?? finduser.Username;
        finduser.Email = user.Email ?? finduser.Email;
        finduser.Password = user.Password ?? finduser.Password;
        user.DateModified = DateTime.UtcNow;
        await dbService.Users.ReplaceOneAsync(u => u.Id.ToString() == id, finduser);
        return Ok(new { message = "User edited suceefully" });
    }


    [HttpDelete("delete{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(new { message = "Please fill required details" });
        }

        var finduser = await dbService.Users.Find(u => u.Id.ToString() == id).FirstOrDefaultAsync();
        if (finduser == null)
        {
            return NotFound(new { message = "User not found" });
        }
        await dbService.Users.DeleteOneAsync(u => u.Id.ToString() == id);
        return Ok(new { message = "User deleted sucessfully" });
    }


[HttpPost("upload/profile/{id}")]
public async Task<IActionResult> Upload(string id, IFormFile file)
{
    try
    {
        var findUser = await dbService.Users.Find(u => u.Id.ToString() == id).FirstOrDefaultAsync();
        if (findUser == null)
        {
            return NotFound(new { message = "User not found." });
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "Invalid file uploaded." });
        }

        // Upload the image to Cloudinary
        var uploadResult = await _cloudnary.UploadImageAsync(file);  // Using your Cloudinary service
        findUser.ProfilePictureUrl = uploadResult.ToString();  // Save Cloudinary URL

        await dbService.Users.ReplaceOneAsync(u => u.Id.ToString() == id, findUser);

        return Ok(new
        {
            message = "File uploaded successfully.",
            imageUrl = uploadResult.ToString()  // Return Cloudinary URL to frontend
        });
    }
    catch (Exception error)
    {
        return StatusCode(500, new { message = $"Server Error: {error.Message}" });
    }
}

}


