# Base image para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Stage de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o arquivo .csproj com caminho relativo correto
COPY ["src/Shopping.Api/Shopping.Api.csproj", "src/Shopping.Api/"]
RUN dotnet restore "src/Shopping.Api/Shopping.Api.csproj"

# Copia o restante do código
COPY . .
WORKDIR "/src/src/Shopping.Api"
RUN dotnet build "Shopping.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica a aplicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Shopping.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shopping.Api.dll"]
