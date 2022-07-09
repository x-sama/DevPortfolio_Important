using AutoMapper;
using Shared.Models;

namespace Server.Data;

public class DTOMapper : Profile
{
    public DTOMapper()
    {
        CreateMap<Post, PostDTO>().ReverseMap();
    }
}