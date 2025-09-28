# Archivo: Dockerfile

# --- Etapa 1: Construcción (Usa el SDK completo de .NET 9.0) ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia los archivos de proyecto (.csproj) primero para aprovechar el caché de Docker
COPY ["ApiMain/ApiMain.csproj", "ApiMain/"]
RUN dotnet restore "ApiMain/ApiMain.csproj"

# Copia el resto del código fuente
COPY . .
WORKDIR "/src/ApiMain"

# Compila y publica la aplicación en modo Release
RUN dotnet publish "ApiMain.csproj" -c Release -o /app/publish


# --- Etapa 2: Publicación Final (Usa una imagen ligera de ASP.NET) ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copia solo la aplicación compilada desde la etapa de construcción
COPY --from=build /app/publish .

# Define el comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "ApiMain.dll"]