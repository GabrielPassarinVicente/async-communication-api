FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Communication.Worker/Communication.Worker.csproj", "Communication.Worker/"]
COPY ["Communication.Application/Communication.Application.csproj", "Communication.Application/"]
COPY ["Communication.Domain/Communication.Domain.csproj", "Communication.Domain/"]
COPY ["Communication.Infrastructure/Communication.Infrastructure.csproj", "Communication.Infrastructure/"]

RUN dotnet restore "Communication.Worker/Communication.Worker.csproj"

COPY . .
WORKDIR "/src/Communication.Worker"
RUN dotnet publish "Communication.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Communication.Worker.dll"]
