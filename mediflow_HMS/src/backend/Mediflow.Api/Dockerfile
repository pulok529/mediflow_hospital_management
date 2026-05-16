FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Mediflow.Api/Mediflow.Api.csproj Mediflow.Api/
COPY Mediflow.Application/Mediflow.Application.csproj Mediflow.Application/
COPY Mediflow.Domain/Mediflow.Domain.csproj Mediflow.Domain/
COPY Mediflow.Infrastructure/Mediflow.Infrastructure.csproj Mediflow.Infrastructure/
RUN dotnet restore Mediflow.Api/Mediflow.Api.csproj

COPY . .
RUN dotnet publish Mediflow.Api/Mediflow.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Mediflow.Api.dll"]
