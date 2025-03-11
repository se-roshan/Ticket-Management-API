using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebAPI_Code_First.Data;
using WebAPI_Code_First.Model;
using BCrypt.Net;
using WebAPI_Code_First.Entities;
using WebAPI_Code_First.Interface;


namespace WebAPI_Code_First.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly RSA _rsa;

        public AuthService(ApplicationDbContext context, IConfiguration config, RSA rsa)
        {
            _context = context;
            _config = config;
            _rsa = rsa;
        }

        //-- Register User
        public async Task<(int, string)> RegisterUser(User user)
        {
            var existingUser = await _context.Users
                .Where(u => u.Email == user.Email || u.ContactNo == user.ContactNo)
                .Select(u => new { u.Email, u.ContactNo })
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                if (existingUser.Email == user.Email && existingUser.ContactNo == user.ContactNo)
                    return (0, "Both email and contact number already exist.");
                else if (existingUser.Email == user.Email)
                    return (0, "Email already exists.");
                else
                    return (0, "Contact number already exists.");
            }

            //user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            //-- Set password hash
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            //-- Set default values for new user
            user.IsActive = true;
            user.CreatedBy = 0;
            user.CreatedDateTime = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return (user.Id, null);
        }

        //-- Forgot Password
        public async Task<ResponseModel<object>> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ResponseModel<object>(400, "Email is required", null);
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser == null)
            {
                return new ResponseModel<object>(400, "Email not found", null);
            }

            //string token = GenerateJwtToken(existingUser); // Using your existing method
            //var responseData = new
            //{
            //    UserId = existingUser.Id,
            //    Username = existingUser.Name,
            //    Token = token
            //};

            //return new ResponseModel<object>(200, "User registered successfully.", responseData);
            return new ResponseModel<object>(200, "Successfully.", existingUser.Id);
        }

        //-- Update Password 
        public async Task<(int, string)> UpdatePassword(PasswordModel passwordModel)
        {
            var getUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == passwordModel.Id);

            if (getUser == null)
            {
                return (0, "Incorrect User Id");
            }

            // Update User Password
            getUser.Password = BCrypt.Net.BCrypt.HashPassword(passwordModel.NewPassword);
            getUser.LastPasswordChange = DateTime.Now;
            getUser.LastPasswordChangeCount = getUser.LastPasswordChangeCount == null ? 1 : getUser.LastPasswordChangeCount + 1;
            getUser.UpdatedDateTime = DateTime.Now;
            getUser.UpdatedBy = passwordModel.UpdatedBy; // Ensure UpdatedBy is set

            // Save changes
            await _context.SaveChangesAsync();

            return (getUser.Id, "Password Updated Successfully");
        }
         
        //-- Login User
        public async Task<ResponseModel<object>> LoginUser(string emailOrContactNo, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == emailOrContactNo || u.ContactNo == emailOrContactNo);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return new ResponseModel<object>(401, "Invalid credentials.", null);
            }

            string token = GenerateJwtToken(user);

            var responseData = new
            {
                UserId = user.Id,
                Username = user.Name,
                Token = token
            };

            return new ResponseModel<object>(200, "Login successful.", responseData);
        }

        //-- Generate JWT Token
        public string GenerateJwtToken(User user)
        {
            //-- Header info (Alogrithm)
            var alog = SecurityAlgorithms.RsaSha256;
            //var alog = SecurityAlgorithms.HmacSha256;//Best Alog

            // -- Payload (Claims)
            var claims = new[]
            {
                //new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                //new Claim(ClaimTypes.Name, user.Email),
                //new Claim("ContactNo", user.ContactNo ?? "")
                new Claim("NameIdentifier", user.Id.ToString()),
                new Claim("Name", user.Name),
                new Claim("Email", user.Email),
                new Claim("ContactNo", user.ContactNo)
            };
             
            //--  Signature
            var jwtSettings = _config.GetSection("JwtSettings");
            //var secretKey = jwtSettings["Key"];
            //// -- Ensure Key is NOT null or empty
            //if (string.IsNullOrEmpty(secretKey))
            //{
            //    throw new Exception("JWT Secret Key is missing in configuration.");
            //}
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            //var credentials = new SigningCredentials(key, alog);  

            //-- If Using RSA 
            var rsaSecurityKey = new RsaSecurityKey(_rsa);
            var credentials = new SigningCredentials(rsaSecurityKey, alog);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}