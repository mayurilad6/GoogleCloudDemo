# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GoogleCloudDemo.csproj", "./"]
RUN dotnet restore "GoogleCloudDemo.csproj"
COPY . .
RUN dotnet build "GoogleCloudDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GoogleCloudDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoogleCloudDemo.dll"]
