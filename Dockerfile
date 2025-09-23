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

EXPOSE 8080

# Crear script inline para esperar MySQL y luego ejecutar la aplicación
RUN echo '#!/bin/bash\necho "Esperando MySQL..."\nwhile ! nc -z mysql-db 3306; do sleep 1; done\necho "MySQL disponible, iniciando aplicación..."\nexec dotnet WebApp.dll' > /app/start.sh \
    && chmod +x /app/start.sh

ENTRYPOINT ["/app/start.sh"]
