FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo csproj y restaurar
COPY app/WebApp/*.csproj .
RUN dotnet restore

# Copiar todo el código
COPY app/WebApp/ .

# Publicar
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "WebApp.dll"]