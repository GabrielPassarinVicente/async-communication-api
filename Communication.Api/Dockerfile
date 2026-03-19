FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Communication.Api/Communication.Api.csproj", "Communication.Api/"]
COPY ["Communication.Application/Communication.Application.csproj", "Communication.Application/"]
COPY ["Communication.Domain/Communication.Domain.csproj", "Communication.Domain/"]
COPY ["Communication.Infrastructure/Communication.Infrastructure.csproj", "Communication.Infrastructure/"]

RUN dotnet restore "Communication.Api/Communication.Api.csproj"

COPY . .
WORKDIR "/src/Communication.Api"
RUN dotnet publish "Communication.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Communication.Api.dll"]
