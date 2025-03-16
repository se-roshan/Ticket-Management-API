using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI_Code_First.Model;

namespace WebAPI_Code_First.Interface
{
    public interface IUserService
    { 
        Task<ResponseModel<List<UserListModel>>> GetAllUsers();
        Task<int> AddUpdatedUser(UserListModel updatedUser);
        Task<List<UserProfileModel>> GetAllUserDetails(string uploadFolder);
        Task<UserProfilePictureModel> GetUserDetailById(string baseUrl, int userId);
        Task<int> AddUpdateRole(RoleModel role);
        Task<List<RoleModel>> GetRoles();

    }
}

