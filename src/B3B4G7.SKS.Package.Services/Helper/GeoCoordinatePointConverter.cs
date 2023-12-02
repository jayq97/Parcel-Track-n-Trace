using AutoMapper;
using NetTopologySuite.Geometries;
using B3B4G7.SKS.Package.BusinessLogic.Entities;
using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.Services.Helper
{
    [ExcludeFromCodeCoverage]
    internal class GeoCoordinatePointConverter :
    IValueConverter<GeoCoordinate, Point>,
    IValueConverter<Point, GeoCoordinate>,
    ITypeConverter<GeoCoordinate, Point>,
    ITypeConverter<Point, GeoCoordinate>
    {
        public Point Convert(GeoCoordinate source, Point dest, ResolutionContext context)
        {
            return this.Convert(source, context);
        }
        public GeoCoordinate Convert(Point source, GeoCoordinate dest, ResolutionContext context)
        {
            return this.Convert(source, context);
        }

        public GeoCoordinate Convert(Point sourceMember, ResolutionContext context)
        {
            if (sourceMember == null)
                return null;

            return new GeoCoordinate()
            {
                Lat = sourceMember.Coordinate.X,
                Lon = sourceMember.Coordinate.Y
            };
        }

        public Point Convert(GeoCoordinate sourceMember, ResolutionContext context)
        {
            if (sourceMember == null)
                return null;

            return new Point(sourceMember.Lat, sourceMember.Lon);
        }
    }
}
