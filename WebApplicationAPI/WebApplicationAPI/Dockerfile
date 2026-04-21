# Dockerfile para backend .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WebApplicationAPI.csproj", "."]
RUN dotnet restore "WebApplicationAPI.csproj"
COPY . .
RUN dotnet publish "WebApplicationAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApplicationAPI.dll"]
