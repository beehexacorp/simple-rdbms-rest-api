
using AutoMapper;
using SimpleRDBMSRestfulAPI.Helpers;
using SimpleRDBMSRestfulAPI.Settings;

namespace SimpleRDBMSRestfulAPI;


public class ConnectionInfoMapper : Profile
{
    public ConnectionInfoMapper(IServiceProvider serviceProvider)
    {
        CreateMap<ConnectionInfoDTO, ConnectionInfoViewModel>()
            .ForMember(x => x.DbType, opt => opt.MapFrom(src => src.DbType))
            .ForMember(x => x.Database, opt => opt.MapFrom(src => serviceProvider.GetRequiredKeyedService<IDataHelper>(src.DbType).GetDatabase(src.ConnectionString)))
            .ForMember(x => x.Host, opt => opt.MapFrom(src => serviceProvider.GetRequiredKeyedService<IDataHelper>(src.DbType).GetHost(src.ConnectionString)))
            .ForMember(x => x.User, opt => opt.MapFrom(src => serviceProvider.GetRequiredKeyedService<IDataHelper>(src.DbType).GetUser(src.ConnectionString)))
            .ForMember(x => x.Port, opt => opt.MapFrom(src => serviceProvider.GetRequiredKeyedService<IDataHelper>(src.DbType).GetPort(src.ConnectionString)));
    }
}