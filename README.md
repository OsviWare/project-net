# Project-Net - Sistema de Marketplace

Este es un proyecto de marketplace desarrollado con ASP.NET Core 8.0 y MySQL, containerizado con Docker. Permite a los usuarios registrarse, autenticarse y gestionar productos en un sistema de comercio electrónico.

## 📋 Características del Sistema

### Funcionalidades Principales
- **Sistema de Autenticación**: Registro e inicio de sesión con roles
- **Gestión de Productos**: CRUD completo de productos con categorías
- **Sistema de Roles**: Administrador, Usuario del Sistema y Usuario Externo
- **Interfaz Responsiva**: Diseño moderno con Bootstrap
- **Base de Datos**: MySQL con Entity Framework Core
- **Containerización**: Docker y Docker Compose

### Tecnologías Utilizadas
- **Backend**: ASP.NET Core 8.0 (MVC)
- **Base de Datos**: MySQL 8.0
- **ORM**: Entity Framework Core
- **Autenticación**: Cookie Authentication
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap
- **Contenedores**: Docker & Docker Compose
- **Cifrado**: BCrypt.Net para contraseñas

## 🏗️ Arquitectura del Sistema

```
project-net/
├── docker-compose.yml      # Orquestación de contenedores
├── Dockerfile             # Imagen de la aplicación ASP.NET
├── entrypoint.sh          # Script de inicio
├── project-net.sln        # Solución de Visual Studio
└── app/WebApp/            # Aplicación principal
    ├── Controllers/       # Controladores MVC
    ├── Models/           # Modelos de datos y ViewModels
    ├── Views/            # Vistas Razor
    ├── Data/             # Contexto de Entity Framework
    ├── Services/         # Servicios de aplicación
    ├── wwwroot/          # Archivos estáticos
    └── appsettings.json  # Configuración
```

### Modelos de Datos
- **Usuario**: Información de usuarios con roles
- **Producto**: Catálogo de productos con categorías
- **Categoria**: Clasificación de productos
- **Orden**: Sistema de órdenes de compra
- **DetalleOrden**: Detalles de las órdenes
- **Rol**: Sistema de permisos

## 🚀 Instalación y Configuración

### Prerrequisitos Obligatorios
- **Docker Desktop**: Versión 4.0+ instalado y ejecutándose
- **Docker Compose**: Versión 2.0+ (incluido con Docker Desktop)
- **Navegador web**: Chrome, Firefox, Edge o Safari
- **Puertos disponibles**: 8080 (aplicación) y 3307 (MySQL)

### Prerrequisitos para Desarrollo (Opcional)
- **Visual Studio Code**: Editor recomendado
- **.NET SDK 8.0+**: Para desarrollo sin Docker
- **Git**: Para control de versiones

## 📥 Instalación Paso a Paso

### Paso 1: Obtener el Código
```bash
# Opción A: Clonar con Git
git clone https://github.com/OsviWare/project-net.git
cd project-net

# Opción B: Descargar ZIP
# 1. Descargar el archivo ZIP del proyecto
# 2. Extraer en una carpeta
# 3. Abrir terminal en esa carpeta
```

### Paso 2: Verificar Prerrequisitos
```bash
# Verificar Docker (OBLIGATORIO)
docker --version
# Debe mostrar: Docker version 20.10+

docker-compose --version  
# Debe mostrar: Docker Compose version v2.0+

# Verificar puertos libres (Windows)
netstat -an | findstr :8080
netstat -an | findstr :3307
# No debe mostrar nada (puertos libres)
```

### Paso 3: Construir y Ejecutar
```bash
# ⚠️ IMPORTANTE: Ejecutar desde la carpeta raíz del proyecto
# donde está el archivo docker-compose.yml

# Construir y ejecutar (primera vez)
docker-compose up --build

# ✅ La aplicación tardará 2-3 minutos en estar lista
# Espera a ver el mensaje: "Now listening on: http://[::]:8080"
```

### Paso 4: Verificar Instalación
```bash
# En otra terminal, verificar contenedores
docker-compose ps

# Debe mostrar algo como:
# NAME         STATUS                    PORTS
# aspnet-app   Up X minutes             0.0.0.0:8080->8080/tcp
# mysql-db     Up X minutes (healthy)   0.0.0.0:3307->3306/tcp
```

### Paso 5: Acceso a la Aplicación
1. Abrir navegador en: **http://localhost:8080**
2. Si ves la página de inicio con productos, ¡funciona! ✅
3. Para acceso admin:
   - Email: `admin@marketplace.com`
   - Password: `admin123`

## � Configuración de VS Code (Desarrollo)

### Extensiones Requeridas
Para desarrollar con VS Code, instala estas extensiones:

1. **C# Dev Kit** (ms-dotnettools.csharp) - Esencial para .NET
2. **Docker** (ms-azuretools.vscode-docker) - Para gestionar contenedores  
3. **PowerShell** (ms-vscode.powershell) - Para terminal mejorado

### Configuración Automática
El proyecto incluye configuración de VS Code en la carpeta `.vscode/`:
- `launch.json` - Configuración de debugging
- `tasks.json` - Tareas de compilación y Docker
- `settings.json` - Configuración del workspace

### Tareas Disponibles (Ctrl+Shift+P → "Tasks: Run Task")
- **docker-compose-up**: Iniciar con Docker
- **docker-compose-down**: Detener contenedores
- **build**: Compilar proyecto
- **watch**: Desarrollo con hot reload

## 🗃️ Configuración de Base de Datos

### Migración Automática de Entity Framework
El proyecto incluye migraciones que se aplican automáticamente al iniciar:

**Si es la primera vez ejecutando:**
```bash
# Las tablas se crean automáticamente
# Datos iniciales incluidos:
# - 3 Roles (administrador, usuario_sistema, externo)
# - 5 Categorías predefinidas
# - Usuario admin y productos de ejemplo
```

**Si tienes problemas con la base de datos:**
```bash
# 1. Detener contenedores
docker-compose down

# 2. Eliminar volúmenes (CUIDADO: borra todos los datos)
docker-compose down -v

# 3. Recrear todo
docker-compose up --build
```

### Acceso Directo a MySQL
```bash
# Conexión desde línea de comandos
docker exec -it mysql-db mysql -u appuser -papppassword appdb

# Verificar tablas creadas
SHOW TABLES;

# Ver productos de ejemplo
SELECT id, nombre, precio FROM productos LIMIT 5;
```

### Credenciales de Base de Datos
```
Servidor: localhost
Puerto: 3307 (externo)
Base de datos: appdb
Usuario: appuser
Contraseña: apppassword
Usuario root: rootpassword
```

## 👥 Sistema de Usuarios y Roles

### Roles Disponibles
1. **Administrador** (`administrador`)
   - Acceso completo al sistema
   - Panel de administración
   - Gestión de todos los productos

2. **Usuario del Sistema** (`usuario_sistema`)
   - Puede vender y comprar productos
   - Gestión de sus propios productos
   - Acceso al marketplace

3. **Usuario Externo** (`externo`)
   - Solo puede comprar productos
   - Visualización del catálogo

### Registro de Usuarios
1. Ir a `/Account/Register`
2. Completar el formulario
3. Por defecto se asigna el rol "usuario_sistema"
4. Iniciar sesión en `/Account/Login`

## 🛍️ Funcionalidades del Sistema

### Para Usuarios No Autenticados
- Ver catálogo de productos
- Ver detalles de productos
- Registrarse en el sistema

### Para Usuarios Autenticados
- Todas las funcionalidades anteriores
- Publicar productos
- Gestionar sus propios productos
- Editar/eliminar productos propios

### Para Administradores
- Todas las funcionalidades anteriores
- Panel de administración
- Gestión completa del sistema

## 🔧 Comandos Útiles

### Docker Compose
```bash
# Iniciar servicios
docker-compose up

# Iniciar en segundo plano
docker-compose up -d

# Reconstruir imágenes
docker-compose up --build

# Ver logs
docker-compose logs

# Ver logs de un servicio específico
docker-compose logs aspnet-app

# Parar servicios
docker-compose down

# Parar servicios y eliminar volúmenes
docker-compose down -v
```

### Gestión de Contenedores
```bash
# Listar contenedores
docker ps

# Acceder a contenedor de la aplicación
docker exec -it aspnet-app bash

# Acceder a contenedor de MySQL
docker exec -it mysql-db bash

# Ver logs de un contenedor
docker logs aspnet-app
```

## 🐛 Solución de Problemas

### ❌ Error: "Port 8080 is already in use"
```bash
# Verificar qué proceso usa el puerto
netstat -ano | findstr :8080

# Cambiar puerto en docker-compose.yml
ports:
  - "8081:8080"  # Usar puerto 8081 en lugar de 8080

# O detener el proceso que usa el puerto
```

### ❌ Error 500: "Table 'productos' doesn't exist"
Este error indica que las migraciones no se aplicaron:

```bash
# Solución 1: Reinstalar completamente
docker-compose down -v
docker-compose up --build

# Solución 2: Aplicar migraciones manualmente
docker exec -it aspnet-app dotnet ef database update
```

### ❌ Error: "Docker daemon is not running"
```bash
# Windows: Abrir Docker Desktop
# Esperar a que aparezca el ícono verde en la bandeja

# Verificar estado
docker ps
# Debe mostrar contenedores o mensaje sin errores
```

### ❌ Error: "Failed to determine the https port"
Este es un warning normal en desarrollo con Docker, no afecta la funcionalidad.

### ❌ La aplicación tarda mucho en cargar
```bash
# Verificar logs
docker-compose logs mysql-db

# Esperar mensaje: "ready for connections"
# MySQL puede tardar 1-2 minutos en inicializar
```

### ❌ Error: "Connection refused" o "Can't connect to MySQL"
```bash
# Verificar que MySQL esté saludable
docker-compose ps

# STATUS debe mostrar "healthy" para mysql-db
# Si no, esperar más tiempo o reiniciar:
docker-compose restart mysql-db
```

### ❌ Página en blanco o sin productos
```bash
# Verificar datos en base
docker exec mysql-db mysql -u appuser -papppassword -e "USE appdb; SELECT COUNT(*) FROM productos;"

# Debe mostrar números > 0
# Si muestra 0, reiniciar con:
docker-compose down -v
docker-compose up --build
```

### 🔄 Solución Universal (Resetear Todo)
```bash
# CUIDADO: Elimina TODOS los datos
docker-compose down -v
docker system prune -f
docker-compose up --build

# Esperar 3-5 minutos hasta ver:
# "Now listening on: http://[::]:8080"
```

### 📋 Verificación Completa del Sistema
```bash
# 1. Verificar Docker
docker --version
docker-compose --version

# 2. Verificar contenedores
docker-compose ps

# 3. Verificar logs (sin errores críticos)
docker-compose logs aspnet-app | findstr "Error\|Exception\|Fail"

# 4. Verificar base de datos
docker exec mysql-db mysql -u appuser -papppassword -e "USE appdb; SHOW TABLES;"

# 5. Verificar aplicación
curl http://localhost:8080
# O abrir en navegador
```

## 📱 URLs Importantes

- **Inicio**: `http://localhost:8080/`
- **Productos**: `http://localhost:8080/Producto`
- **Registro**: `http://localhost:8080/Account/Register`
- **Login**: `http://localhost:8080/Account/Login`
- **Mis Productos**: `http://localhost:8080/Producto/Mine` (requiere login)
- **Crear Producto**: `http://localhost:8080/Producto/Create` (requiere login)

## 📝 Desarrollo Local (Sin Docker)

### Prerrequisitos Adicionales
1. **.NET 8.0 SDK** - [Descargar aquí](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **MySQL Server 8.0** - [Descargar aquí](https://dev.mysql.com/downloads/mysql/)
3. **Entity Framework Tools**

### Configuración Paso a Paso
```bash
# 1. Instalar herramientas EF
dotnet tool install --global dotnet-ef

# 2. Navegar al proyecto
cd app/WebApp

# 3. Restaurar paquetes
dotnet restore

# 4. Configurar connection string en appsettings.json
# Cambiar "Server=127.0.0.1;Port=3306;..." según tu MySQL local

# 5. Crear y aplicar migraciones
dotnet ef migrations add InitialCreate
dotnet ef database update

# 6. Ejecutar aplicación
dotnet run

# 7. Acceder en: https://localhost:5001 o http://localhost:5000
```

### Configuración de MySQL Local
```sql
-- Crear base de datos y usuario
CREATE DATABASE appdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'appuser'@'localhost' IDENTIFIED BY 'apppassword';
GRANT ALL PRIVILEGES ON appdb.* TO 'appuser'@'localhost';
FLUSH PRIVILEGES;
```

### Connection Strings para Desarrollo Local
```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1;Port=3306;Database=appdb;User=appuser;Password=apppassword;"
  }
}
```

## 🚀 Despliegue en Producción

### Variables de Entorno Importantes
```bash
# En producción, configurar:
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection="tu-connection-string-produccion"
ASPNETCORE_URLS=http://+:80
```

### Docker Compose para Producción
```yaml
# docker-compose.prod.yml
version: '3.8'
services:
  aspnet-app:
    build: .
    ports:
      - "80:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=mysql-db;Database=appdb;User=appuser;Password=${DB_PASSWORD}
    depends_on:
      mysql-db:
        condition: service_healthy

  mysql-db:
    image: mysql:8.0
    environment:
      MYSQL_ROOT_PASSWORD: ${MYSQL_ROOT_PASSWORD}
      MYSQL_DATABASE: appdb
      MYSQL_USER: appuser
      MYSQL_PASSWORD: ${DB_PASSWORD}
    volumes:
      - mysql-prod-data:/var/lib/mysql

volumes:
  mysql-prod-data:
```

### Comandos para Producción
```bash
# Usar archivo de producción
docker-compose -f docker-compose.prod.yml up --build -d

# Con variables de entorno
DB_PASSWORD=tu-password-seguro MYSQL_ROOT_PASSWORD=root-password-seguro docker-compose -f docker-compose.prod.yml up -d
```

## 🔐 Seguridad

- Contraseñas encriptadas con BCrypt
- Autenticación basada en cookies
- Validación de formularios con tokens CSRF
- Autorización por roles
- Logs de acceso y actividad

## 📞 Soporte y Contacto

### ⚡ Solución Rápida
1. **Reiniciar Todo**: `docker-compose down -v && docker-compose up --build`
2. **Verificar Puertos**: Usar 8081 si 8080 está ocupado
3. **Esperar Pacientemente**: MySQL tarda 1-2 minutos en inicializar
4. **Verificar Docker**: Asegurar que Docker Desktop esté ejecutándose

### 📋 Lista de Verificación Pre-Instalación
- [ ] Docker Desktop instalado y ejecutándose
- [ ] Puertos 8080 y 3307 libres
- [ ] Suficiente espacio en disco (2GB mínimo)
- [ ] Conexión a internet (para descargar imágenes)
- [ ] Permisos de administrador (si es necesario)

### 🔍 Información del Sistema
```bash
# Para reportar problemas, incluir esta información:
docker --version
docker-compose --version
docker system df
docker-compose ps
docker-compose logs aspnet-app --tail 50
```

### 📧 Contacto
Para reportar bugs o solicitar características:
1. **GitHub Issues**: [Crear issue](https://github.com/OsviWare/project-net/issues)
2. **Email**: tucorreo@dominio.com
3. **Documentación**: [Wiki del proyecto](https://github.com/OsviWare/project-net/wiki)

### 🤝 Contribuir
1. Fork el proyecto
2. Crear rama feature (`git checkout -b feature/nueva-caracteristica`)
3. Commit cambios (`git commit -m 'Agregar nueva característica'`)
4. Push a la rama (`git push origin feature/nueva-caracteristica`)
5. Abrir Pull Request

### 📄 Licencia
Este proyecto está licenciado bajo la [MIT License](LICENSE) - ver el archivo LICENSE para detalles.

---

## 🎉 ¡Listo para Usar!

Si seguiste todos los pasos correctamente, deberías tener:

✅ **Aplicación funcionando** en http://localhost:8080  
✅ **Base de datos MySQL** con datos de ejemplo  
✅ **Sistema de autenticación** funcionando  
✅ **Productos de muestra** cargados  
✅ **Usuario admin** configurado (admin@marketplace.com / admin123)  

### URLs Principales
- **🏠 Inicio**: http://localhost:8080
- **🛍️ Productos**: http://localhost:8080/Producto
- **👤 Registro**: http://localhost:8080/Account/Register
- **🔑 Login**: http://localhost:8080/Account/Login
- **📦 Mis Productos**: http://localhost:8080/Producto/Mine
- **➕ Crear Producto**: http://localhost:8080/Producto/Create

**¡Bienvenido a tu nuevo sistema de marketplace!** 🚀