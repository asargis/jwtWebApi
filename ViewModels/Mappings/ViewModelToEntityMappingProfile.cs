using Microsoft.AspNetCore.Identity;
using AutoMapper;
using MyApp.Models;


namespace MyApp.ViewModels.Mappings
{
	public class ViewModelToEntityMappingProfile : Profile
	{
		public ViewModelToEntityMappingProfile()
		{
			CreateMap<RegistrationViewModel, AppUser>()
			.ForMember(
				au => au.UserName,
				map => map.MapFrom(vm => vm.Phone)
			);
		}
	}
}