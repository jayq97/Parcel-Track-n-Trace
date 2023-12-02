using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.Services.Helper
{
    [ExcludeFromCodeCoverage]
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<DTOs.GeoCoordinate, BusinessLogic.Entities.GeoCoordinate>().ReverseMap();
            CreateMap<DTOs.Hop, BusinessLogic.Entities.Hop>().ReverseMap();
            CreateMap<DTOs.HopArrival, BusinessLogic.Entities.HopArrival>().ReverseMap();
            CreateMap<DTOs.Parcel, BusinessLogic.Entities.Parcel>().ReverseMap();
            CreateMap<DTOs.Recipient, BusinessLogic.Entities.Recipient>().ReverseMap();
            CreateMap<DTOs.WebhookMessage, BusinessLogic.Entities.WebhookMessage>().ReverseMap();
            CreateMap<DTOs.WebhookResponse, BusinessLogic.Entities.WebhookResponse>().ReverseMap();
            CreateMap<DTOs.TrackingInformation, DTOs.WebhookMessage>()
                .ForMember(dest => dest.VisitedHops, opt => opt.MapFrom(src => src.VisitedHops))
                .ForMember(dest => dest.FutureHops, opt => opt.MapFrom(src => src.FutureHops))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State));

            CreateMap<DTOs.Transferwarehouse, BusinessLogic.Entities.Transferwarehouse>()
                .IncludeBase<DTOs.Hop, BusinessLogic.Entities.Hop>();
            CreateMap<DTOs.Truck, BusinessLogic.Entities.Truck>()
                .IncludeBase<DTOs.Hop, BusinessLogic.Entities.Hop>();
            CreateMap<DTOs.Warehouse, BusinessLogic.Entities.Warehouse>()
                .IncludeBase<DTOs.Hop, BusinessLogic.Entities.Hop>();

            CreateMap<BusinessLogic.Entities.Transferwarehouse, DTOs.Transferwarehouse>()
                .IncludeBase<BusinessLogic.Entities.Hop, DTOs.Hop>();
            CreateMap<BusinessLogic.Entities.Truck, DTOs.Truck>()
                .IncludeBase<BusinessLogic.Entities.Hop, DTOs.Hop>();
            CreateMap<BusinessLogic.Entities.Warehouse, DTOs.Warehouse>()
                .IncludeBase<BusinessLogic.Entities.Hop, DTOs.Hop>();

            CreateMap<DTOs.WarehouseNextHops, BusinessLogic.Entities.WarehouseNextHops>().ReverseMap();

            CreateMap<BusinessLogic.Entities.Parcel, DTOs.TrackingInformation>()
                .ForMember(dest => dest.VisitedHops, opt => opt.MapFrom(src => src.VisitedHops))
                .ForMember(dest => dest.FutureHops, opt => opt.MapFrom(src => src.FutureHops))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State));

            CreateMap<BusinessLogic.Entities.Parcel, DTOs.NewParcelInfo>()
                .ForMember(dest => dest.TrackingId, opt => opt.MapFrom(src => src.TrackingId));

            CreateMap<BusinessLogic.Entities.Hop, DataAccess.Entities.Hop>().ReverseMap();

            CreateMap<BusinessLogic.Entities.Hop, DataAccess.Entities.Hop>()
                .ForMember(dest => dest.LocationCoordinates, opt => opt.ConvertUsing(new GeoCoordinatePointConverter(), src => src.LocationCoordinates));

            CreateMap<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>()
                .ForMember(dest => dest.LocationCoordinates, opt => opt.ConvertUsing(new GeoCoordinatePointConverter(), src => src.LocationCoordinates));

            CreateMap<BusinessLogic.Entities.HopArrival, DataAccess.Entities.HopArrival>().ReverseMap();
            CreateMap<BusinessLogic.Entities.Parcel, DataAccess.Entities.Parcel>().ReverseMap();
            CreateMap<BusinessLogic.Entities.Recipient, DataAccess.Entities.Recipient>().ReverseMap();
            CreateMap<BusinessLogic.Entities.WebhookMessage, DataAccess.Entities.WebhookMessage>().ReverseMap();
            CreateMap<BusinessLogic.Entities.WebhookResponse, DataAccess.Entities.WebhookResponse>().ReverseMap();

            CreateMap<BusinessLogic.Entities.Transferwarehouse, DataAccess.Entities.Transferwarehouse>()
                .ForMember(dest => dest.Region, opt => opt.ConvertUsing(new GeoJsonConverter(), src => src.RegionGeoJson))
                 .IncludeBase<BusinessLogic.Entities.Hop, DataAccess.Entities.Hop>();
            CreateMap<BusinessLogic.Entities.Truck, DataAccess.Entities.Truck>()
                .ForMember(dest => dest.Region, opt => opt.ConvertUsing(new GeoJsonConverter(), src => src.RegionGeoJson))
                .IncludeBase<BusinessLogic.Entities.Hop, DataAccess.Entities.Hop>();
            CreateMap<BusinessLogic.Entities.Warehouse, DataAccess.Entities.Warehouse>()
                .IncludeBase<BusinessLogic.Entities.Hop, DataAccess.Entities.Hop>();

            CreateMap<DataAccess.Entities.Transferwarehouse, BusinessLogic.Entities.Transferwarehouse>()
                .ForMember(dest => dest.RegionGeoJson, opt => opt.ConvertUsing(new GeoJsonConverter(), src => src.Region))
                .IncludeBase<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>();

            CreateMap<DataAccess.Entities.Truck, BusinessLogic.Entities.Truck>()
                .ForMember(dest => dest.RegionGeoJson, opt => opt.ConvertUsing(new GeoJsonConverter(), src => src.Region))
                .IncludeBase<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>();

            CreateMap<DataAccess.Entities.Warehouse, BusinessLogic.Entities.Warehouse>()
                .IncludeBase<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>();

            CreateMap<DataAccess.Entities.Hop, DataAccess.Entities.Warehouse>();

            CreateMap<BusinessLogic.Entities.WarehouseNextHops, DataAccess.Entities.WarehouseNextHops>().ReverseMap();
        }
    }
}
