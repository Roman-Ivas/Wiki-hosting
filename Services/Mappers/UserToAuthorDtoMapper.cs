using viki_01.Entities;
using viki_01.Models.Dto;

namespace viki_01.Services.Mappers;

public class UserToAuthorDtoMapper : IMapper<User, AuthorDto>
{
    public AuthorDto Map(User original)
    {
        return new AuthorDto
        {
            Id = original.Id,
            UserName = original.UserName!,
            AvatarPath = original.AvatarPath
        };
    }

    public User Map(AuthorDto transformed)
    {
        return new User
        {
            Id = transformed.Id,
            UserName = transformed.UserName,
            AvatarPath = transformed.AvatarPath
        };
    }

    public void Map(User source, User destination)
    {
        destination.Id = source.Id;
        destination.UserName = source.UserName;
        destination.AvatarPath = source.AvatarPath;
    }
}