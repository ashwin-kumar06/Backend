using Backend.Data;
using Backend.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Net;
using System.Net.Mail;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignupLoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtsettings;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public SignupLoginController(AppDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtsettings = jwtSettings.Value;
            _key = Encoding.ASCII.GetBytes(_jwtsettings.Key);
            _iv = new byte[16];

        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login loginuser)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == loginuser.Email);
            if (user == null)
            {
                return NotFound("Email or Password Not Found");
            }
            string decryptedPassword = DecryptString(user.Password);
            if (decryptedPassword != loginuser.Password)
            {
                return NotFound("Invalid Email or Password");
            }
            var tokenString = GenerateJwtToken(user);
            return Ok(new { tokenString,user.UserId });
        }

        private string GenerateJwtToken(Users user)
        {
            var claims = new[]
            {
                new Claim("user_id", user.UserId.ToString()),
                new Claim(ClaimTypes.Role,"user"),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtsettings.Issuer,
                Audience = _jwtsettings.Audience
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("signup")]
        public IActionResult Signup([FromBody] Users signupuser)
        {
            signupuser.Password = EncryptString(signupuser.Password);
            _context.Users.Add(signupuser);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Signup), new { id = signupuser.UserId }, signupuser);
        }

        private string EncryptString(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private string DecryptString(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        
        private string GetEmail([FromBody] Users users)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == users.Email);
            if (user == null)
            {
                return "Email Not Found";
            }

            return (users.Email);  
        }

        [HttpPost("forgotpassword")]
        public IActionResult GenerateOTP([FromQuery] string email)
        {
            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string sOTP = String.Empty;

            string sTempChars = String.Empty;

            Random rand = new Random();

            for (int i = 0; i < 6; i++)

            {

                int p = rand.Next(0, saAllowedCharacters.Length);

                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];

                sOTP += sTempChars;

            }

            string sender = "ashwinkumar0850@gmail.com";
            string senderPass = "tmlr cofs gohl lapn";
            string recieve = email;

            MailMessage mail = new MailMessage(sender, recieve);
            mail.Subject = "Forgot password";
            mail.Body = $"The OTP to change your password is: {sOTP}";

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(sender, senderPass);
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mail);
                Console.WriteLine("Sent Successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            return Ok(new { sOTP });
        }
    }
}
