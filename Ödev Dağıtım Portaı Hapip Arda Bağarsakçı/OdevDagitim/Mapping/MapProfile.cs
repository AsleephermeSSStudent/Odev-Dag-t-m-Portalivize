using AutoMapper;
using OdevDagitim.Models;
using OdevDagitim.ViewModel;

namespace OdevDagitim.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<User,RegisterModel> ().ReverseMap();
            CreateMap<Class, ClassViewModel>().ReverseMap();
            CreateMap<Assignment, AssignmentViewModel>()
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class.ClassName))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.Username));
            
            CreateMap<AssignmentViewModel, Assignment>();
            CreateMap<User, UserListViewModel>()
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class.ClassName));

            CreateMap<Assignment, AssignmentDetailViewModel>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.Username))
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class.ClassName));

            CreateMap<AssignmentSubmission, SubmissionDetailViewModel>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.User.Username));
        }
    }
}
