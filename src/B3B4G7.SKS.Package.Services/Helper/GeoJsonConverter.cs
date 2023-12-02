using AutoMapper;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace B3B4G7.SKS.Package.Services.Helper
{
    [ExcludeFromCodeCoverage]
    internal class GeoJsonConverter :
        IValueConverter<string, Geometry>,
        IValueConverter<Geometry, string>,
        ITypeConverter<string, Geometry>,
        ITypeConverter<Geometry, string>
    {
        public string Convert(Geometry source, string destination, ResolutionContext context)
        {
            return this.Convert(source, context);
        }

        public Geometry Convert(string source, Geometry destination, ResolutionContext context)
        {
            return this.Convert(source, context);
        }

        public Geometry Convert(string source, ResolutionContext context)
        {
            var serializer = GeoJsonSerializer.CreateDefault();
            var feature = (Feature)serializer.Deserialize(new StringReader(source), typeof(Feature));
            return feature?.Geometry;
        }

        public string Convert(Geometry sourceMember, ResolutionContext context)
        {
            Feature feature = new(sourceMember, null);

            var serializer = GeoJsonSerializer.CreateDefault();

            StringWriter writer = new();
            serializer.Serialize(writer, feature);
            writer.Flush();

            return writer.ToString();
        }
    }
}