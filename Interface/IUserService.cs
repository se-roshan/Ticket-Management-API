using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI_Code_First.Model;

namespace WebAPI_Code_First.Interface
{
    public interface IUserService
    { 
        Task<ResponseModel<List<UserListModel>>> GetAllUsers();
        Task<int> UpdateUser(UserListModel updatedUser);
        Task<List<UserProfileModel>> GetAllUserDetails(string uploadFolder);
        Task<int> AddUpdateRole(RoleModel role);
        Task<List<RoleModel>> GetRoles();
    }
}

