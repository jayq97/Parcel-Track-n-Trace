#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Container we use for final publish
FROM mcr.microsoft.com/dotnet/core/aspnet:6.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build container
FROM mcr.microsoft.com/dotnet/core/sdk:6.0-buster AS build

# Copy the code into the container
WORKDIR /src
COPY ["src/B3B4G7.SKS.Package.Services/B3B4G7.SKS.Package.Services.csproj", "B3B4G7.SKS.Package.Services/"]

# NuGet restore
RUN dotnet restore "B3B4G7.SKS.Package.Services/B3B4G7.SKS.Package.Services.csproj"
COPY ["src/B3B4G7.SKS.Package.Services/", "B3B4G7.SKS.Package.Services/"]

# Build the API
WORKDIR "B3B4G7.SKS.Package.Services"
RUN dotnet build "B3B4G7.SKS.Package.Services.csproj" -c Release -o /app/build

# Publish it
FROM build AS publish
RUN dotnet publish "B3B4G7.SKS.Package.Services.csproj" -c Release -o /app/publish

# Make the final image for publishing
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "B3B4G7.SKS.Package.Services.dll"]
