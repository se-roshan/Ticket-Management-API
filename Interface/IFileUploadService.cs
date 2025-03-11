using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI_Code_First.Entities;
using WebAPI_Code_First.Model;

namespace WebAPI_Code_First.Interface
{
    public interface IFileUploadService
    {
        Task<int> SaveFileRecord(FileUpload fileUpload);
        Task<List<UserProfilePics>> GetFilesByUserId(int userId, string FilePath);
    }
}
