////using AutoMapper;
////using WebAPI_Code_First.Model;

////namespace WebAPI_Code_First.Mappings
////{
////    public class AutoMapperProfiles
////    {
////    }
////}
//using AutoMapper;
//using WebAPI_Code_First.Models; // Replace with your actual namespace
//using WebAPI_Code_First.Entities; // Replace with your actual namespace

//public class AutoMapperProfiles : Profile
//{
//    public AutoMapperProfiles()
//    {
//        // UserProfileModel Mapping
//        CreateMap<User, UserProfileModel>()
//            .ForMember(dest => dest.UserProfilePics, opt => opt.Ignore()); // We'll map this manually

//        // UserProfilePics Mapping from FileUpload
//        CreateMap<FileUpload, UserProfilePics>()
//            .ForMember(dest => dest.FilePath, opt => opt.Ignore()); // We'll manually map FilePath
//    }
//}
