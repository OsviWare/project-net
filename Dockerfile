# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY app/WebApp/*.csproj ./
RUN dotnet restore
COPY app/WebApp/ ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime (con SDK para poder usar dotnet ef)
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

# Instalar netcat y herramientas de red
RUN apt-get update && \
    apt-get install -y netcat-openbsd iputils-ping telnet && \
    rm -rf /var/lib/apt/lists/*

# Instalar herramienta ef globalmente
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copiar la aplicación desde la etapa build
COPY --from=build /app/publish .

# Copiar entrypoint
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh

EXPOSE 8080
ENTRYPOINT ["./entrypoint.sh"]
