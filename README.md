# prueba-stefanini-bac

Sistema de gestión de ventas con API REST y aplicación web ASP.NET Core.

### Base de Datos

#### Procedimiento Almacenado
```sql
-- Ejecutar en SQL Server
USE master;
GO

-- Crear el procedimiento almacenado
-- (copiar y ejecutar el contenido de db/database.sql)

-- Ejecutar el procedimiento para crear la base de datos
EXEC sp_CrearDBVentas
```

## Estructura del proyecto

```
prueba-stefanini-bac/
├── API-REST/              # API REST con ASP.NET Core
│   ├── Controllers/       # Endpoints de la API
│   ├── Models/            # Modelos EF Core
│   └── Services/          # Autenticación, utilidades
├── WEB/                   # Aplicación web con Razor Pages
│   ├── Controllers/       # Controladores MVC
│   ├── Models/            # ViewModels
│   ├── Views/             # Vistas Razor
│   └── Services/          # Servicio para llamar API
└── database.sql           # Script para crear BD
```

La gestión del sistema permite:
- Autenticación y autorización de usuarios por roles Admin y Operador
- Gestionar productos (CRUD)
- Registro de ventas y sus detalles
- Generación de reportes de ventas

### Configurar la API

#### Cadena de Conexión
Editar `src/API-REST/API-REST/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "CadenaSQL": "Server=TU_SERVIDOR;Database=db_ventas;Trusted_Connection=true;TrustServerCertificate=true"
  },
  "Jwt": {
    "Key": "Mi_Clave_Super_Secreta_Para_JWT_2024_PruebaTecnica_Stefanini",
  }
}
```

### Configurar la Aplicación Web

#### Configuración de la API
Editar `src/WEB/WEB/appsettings.json`:
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7023"
  }
}
```

**Endpoints disponibles:**
- `GET /api/productos` - Listar productos (con paginado y búsqueda)
- `GET /api/productos/{id}` - Obtener producto por ID
- `POST /api/productos` - Crear producto
- `PUT /api/productos/{id}` - Actualizar producto
- `DELETE /api/productos/{id}` - Eliminar producto
- `POST /api/auth/login` - Autenticación
- `GET /api/ventas` - Listar ventas
- `POST /api/ventas` - Crear venta
- `GET /api/reportes/ventas-por-vendedor` - Ventas agrupadas por vendedor. Parámetros opcionales: `fechaInicio`, `fechaFin` (formato YYYY-MM-DD).
- `GET /api/reportes/productos-mas-vendidos` - Productos más vendidos. Parámetros opcionales: `fechaInicio`, `fechaFin`, `top` (entero, por defecto 10).
- `GET /api/reportes/ventas-por-fecha` - Ventas agrupadas por fecha. Parámetros opcionales: `fechaInicio`, `fechaFin`.
- `GET /api/reportes/resumen-general` - Resumen general (totales y promedio). Parámetros opcionales: `fechaInicio`, `fechaFin`.
