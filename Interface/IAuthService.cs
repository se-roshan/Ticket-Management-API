using System.Threading.Tasks;
using WebAPI_Code_First.Entities;
using WebAPI_Code_First.Model;

namespace WebAPI_Code_First.Interface
{
    public interface IAuthService
    { 
        Task<(int UserId, string ErrorMessage)> RegisterUser(User user); 
        Task<ResponseModel<object>> ForgotPassword(string email);
        Task<(int UserId, string ErrorMessage)> UpdatePassword(PasswordModel passwordModel);
        Task<ResponseModel<object>> LoginUser(string emailOrContactNo, string password);
        string GenerateJwtToken(User user);
    }
}