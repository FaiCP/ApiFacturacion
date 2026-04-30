# GestorAdmi Core — Documentación de la API

> **Versión:** 1.0 | **Framework:** .NET 10.0 | **Hospedaje:** somee.com
> **URL producción:** `https://CNEAPI.somee.com`
> **Swagger:** `https://CNEAPI.somee.com/swagger`

---

## Tabla de contenidos

1. [Descripción general](#1-descripción-general)
2. [Arquitectura del proyecto](#2-arquitectura-del-proyecto)
3. [Patrones de diseño utilizados](#3-patrones-de-diseño-utilizados)
4. [Estructura de carpetas](#4-estructura-de-carpetas)
5. [Endpoints de la API](#5-endpoints-de-la-api)
6. [Autenticación y seguridad](#6-autenticación-y-seguridad)
7. [Base de datos](#7-base-de-datos)
8. [Generación de reportes](#8-generación-de-reportes)
9. [Configuración por entorno](#9-configuración-por-entorno)
10. [Pruebas](#10-pruebas)
11. [Despliegue a somee.com](#11-despliegue-a-someecom)
12. [Dependencias principales](#12-dependencias-principales)

---

## 1. Descripción general

**GestorAdmi.Core** es una API REST para la gestión de activos tecnológicos de una organización. Permite administrar el inventario de hardware, asignar equipos a custodios, gestionar suministros y generar reportes en PDF y Excel.

### Capacidades principales

| Módulo             | Descripción                                               |
|--------------------|-----------------------------------------------------------|
| Hardware           | Inventario de equipos (computadoras, monitores, etc.)     |
| Custodios          | Personas responsables de los equipos                     |
| Gestión de Activos | Asignación y devolución de equipos a custodios           |
| Departamentos      | Organización por áreas o unidades                        |
| Personal           | Registro de empleados                                    |
| Kits               | Conjuntos de equipos agrupados                           |
| Suministros        | Consumibles y materiales remanufacturados                |
| Reportes           | Estadísticas de inventario y préstamos                   |
| Documentos         | Exportación a PDF y Excel                                |

---

## 2. Arquitectura del proyecto

El proyecto implementa **Clean Architecture** (Arquitectura Limpia), dividida en 4 capas con dependencias unidireccionales hacia adentro:

```
┌────────────────────────────────────────────────────────────┐
│                     API (Presentación)                      │
│         Controllers  •  Middleware  •  Models               │
├────────────────────────────────────────────────────────────┤
│                 Application (Casos de uso)                  │
│      Commands  •  Queries  •  DTOs  •  Interfaces           │
├────────────────────────────────────────────────────────────┤
│                  Domain (Reglas de negocio)                 │
│        Entities  •  Interfaces  •  Exceptions               │
├────────────────────────────────────────────────────────────┤
│              Infrastructure (Datos / Servicios)             │
│    Repositories  •  DbContext  •  JwtService  •  PdfService │
└────────────────────────────────────────────────────────────┘
```

### Principios que guían la arquitectura

- **Inversión de dependencias:** las capas internas (Domain, Application) solo conocen interfaces; las implementaciones concretas viven en Infrastructure.
- **Independencia de frameworks:** el dominio no tiene dependencias de ASP.NET ni de EF Core.
- **Separación de responsabilidades:** cada capa tiene una función clara y delimitada.

---

## 3. Patrones de diseño utilizados

### 3.1 CQRS (Command Query Responsibility Segregation)

Se separan las operaciones de **lectura** (Queries) de las de **escritura** (Commands):

```
Application/
├── Commands/           ← Modifican estado (Create, Update, Delete)
│   ├── Hardware/
│   │   └── CreateHardwareCommand.cs   + Handler
│   ├── Auth/
│   │   └── LoginCommand.cs            + Handler
│   └── ...
└── Queries/            ← Solo leen datos (sin efectos secundarios)
    ├── Hardware/
    │   └── GetHardwareQuery.cs        + Handler
    └── ...
```

**Ejemplo de flujo de escritura:**

```
HTTP POST /api/hardware/Crear
    └─> HardwareController.Crear(dto)
        └─> mediator.Send(new CreateHardwareCommand(dto))
            └─> CreateHardwareCommandHandler.Handle()
                └─> _hardwareRepository.AddAsync(entity)
```

### 3.2 Mediator (con MediatR)

Todos los comandos y queries pasan por el mediador, desacoplando controladores de handlers:

```csharp
// Controlador solo conoce MediatR, no el Handler directamente
var result = await _mediator.Send(new GetHardwareQuery(page, pageSize, search));
```

### 3.3 Repository Pattern + Generic Repository

Existe un `IGenericRepository<T>` con operaciones base (Get, Add, Update, Delete), y repositorios específicos para cada entidad que extienden funcionalidad:

```
IGenericRepository<T>        ← CRUD base
IHardwareRepository          ← extiende con búsqueda paginada + joins
ICustodioRepository          ← consultas especializadas de custodios
IGestionActivoRepository     ← lógica de asignación/devolución
```

### 3.4 Dependency Injection

Toda la inyección de dependencias se configura mediante métodos de extensión:

```csharp
// Program.cs
builder.Services.AddApplicationServices();     // MediatR, AutoMapper, FluentValidation
builder.Services.AddInfrastructureServices();  // DbContext, Repos, JWT, PDF, Excel
```

### 3.5 DTO (Data Transfer Objects) + AutoMapper

Las entidades del dominio nunca se exponen directamente. Se usan DTOs para entrada/salida y AutoMapper para las conversiones:

```
Entidad (Domain) ←→ AutoMapper ←→ DTO (Application) ←→ JSON (API)
```

### 3.6 Soft Delete

Las entidades heredan de `BaseEntity` que incluye la propiedad `Borrado (bool)`. Al "eliminar" un registro, solo se marca `Borrado = true`; el dato persiste en la base de datos.

```csharp
public abstract class BaseEntity {
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Borrado { get; set; }
}
```

### 3.7 Global Exception Handling (Middleware)

Un middleware centralizado captura todas las excepciones y devuelve respuestas consistentes:

| Excepción             | HTTP Status |
|-----------------------|-------------|
| `NotFoundException`   | 404         |
| `ValidationException` | 400         |
| `DomainException`     | 400         |
| Excepción genérica    | 500         |

### 3.8 Paginación estandarizada

Todas las consultas de listados retornan `PaginatedResponse<T>`:

```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 45,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

---

## 4. Estructura de carpetas

```
GestorAdmi.Core/
│
├── src/
│   ├── API/                         ← Capa de presentación
│   │   ├── Controllers/             ← 11 controladores REST
│   │   ├── Middleware/              ← Manejo global de excepciones
│   │   ├── Models/                  ← ApiResponse, PaginationParams
│   │   ├── Program.cs               ← Punto de entrada y configuración
│   │   ├── appsettings.json         ← Configuración base
│   │   ├── appsettings.Development.json
│   │   ├── appsettings.Production.json
│   │   └── web.config               ← Configuración para IIS/somee.com
│   │
│   ├── Application/                 ← Casos de uso
│   │   ├── Commands/                ← Por dominio: Auth, Hardware, etc.
│   │   ├── Queries/                 ← Por dominio
│   │   ├── DTOs/                    ← Por dominio
│   │   ├── Mappings/MappingProfile.cs
│   │   ├── Interfaces/              ← IPdfService, IExcelService
│   │   ├── Common/PaginatedResponse.cs
│   │   └── DependencyInjection.cs
│   │
│   ├── Domain/                      ← Núcleo del negocio (sin dependencias externas)
│   │   ├── Entities/                ← BaseEntity + 11 entidades
│   │   ├── Interfaces/              ← Contratos de repositorios
│   │   └── Exceptions/              ← DomainException, NotFoundException, ValidationException
│   │
│   └── Infrastructure/              ← Implementaciones concretas
│       ├── Persistence/ApplicationDbContext.cs
│       ├── Repositories/            ← 10 repositorios
│       ├── Services/                ← JwtService, PdfService, ExcelService
│       └── DependencyInjection.cs
│
├── tests/
│   ├── GestorAdmi.Tests.Unit/       ← xUnit + Moq + FluentAssertions
│   └── GestorAdmi.Tests.Integration/← WebApplicationFactory + BD en memoria
│
├── deploy.ps1                       ← Script de despliegue automatizado
└── DOCUMENTACION_API.md             ← Este archivo
```

---

## 5. Endpoints de la API

> **Base URL:** `https://CNEAPI.somee.com`
> Todos los endpoints (excepto `/login`) requieren el header:
> `Authorization: Bearer <token>`

---

### Auth

| Método | Ruta    | Descripción                          | Auth requerida |
|--------|---------|--------------------------------------|----------------|
| POST   | /login  | Obtiene token JWT                   | No             |

**Request body (`/login`):**
```json
{
  "username": "admin",
  "password": "tu_contraseña"
}
```

**Response exitosa:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGci...",
    "expiration": "2026-03-19T16:00:00Z"
  }
}
```

---

### Hardware — `/api/hardware`

| Método | Ruta                  | Descripción                             |
|--------|-----------------------|-----------------------------------------|
| GET    | /LeerTodo             | Lista paginada con búsqueda             |
| GET    | /GenerarActa          | Descarga PDF del inventario             |
| GET    | /GenerarActaExcel     | Descarga Excel del inventario           |
| POST   | /Crear                | Registra nuevo equipo                   |
| PUT    | /Actualizar/{id}      | Actualiza datos del equipo              |
| DELETE | /Eliminar             | Elimina (soft delete) varios equipos    |

**Parámetros de paginación (query string):**
```
?page=1&pageSize=10&search=monitor
```

---

### Custodios — `/api/custodios`

| Método | Ruta             | Descripción                      |
|--------|------------------|----------------------------------|
| GET    | /LeerTodo        | Lista paginada                   |
| GET    | /acta            | PDF de custodios                 |
| POST   | /Crear           | Crea custodio                    |
| PUT    | /Actualizar/{id} | Actualiza custodio               |
| DELETE | /Eliminar        | Soft delete                      |

---

### Gestión de Activos — `/api/gestionactivos`

| Método | Ruta             | Descripción                                |
|--------|------------------|--------------------------------------------|
| GET    | /LeerTodo        | Lista de asignaciones paginada             |
| GET    | /acta            | PDF de acta de entrega                     |
| GET    | /devolucion      | PDF de acta de devolución                  |
| POST   | /Crear           | Asigna equipo a un custodio                |
| PUT    | /Actualizar/{id} | Registra devolución o actualiza asignación |
| DELETE | /Eliminar        | Soft delete                                |

---

### Departamentos — `/api/departamentos`

| Método | Ruta      | Descripción           |
|--------|-----------|-----------------------|
| GET    | /LeerTodo | Lista paginada        |

---

### Personal — `/api/personal`

| Método | Ruta             | Descripción    |
|--------|------------------|----------------|
| GET    | /LeerTodo        | Lista paginada |
| POST   | /Crear           | Crea empleado  |
| PUT    | /Actualizar/{id} | Actualiza      |
| DELETE | /Eliminar        | Soft delete    |

---

### Kits — `/api/kits`

| Método | Ruta             | Descripción       |
|--------|------------------|-------------------|
| GET    | /LeerTodo        | Lista paginada    |
| POST   | /Crear           | Crea kit          |
| PUT    | /Actualizar/{id} | Actualiza kit     |
| DELETE | /Eliminar        | Soft delete       |

---

### Suministros — `/api/suministros`

| Método | Ruta             | Descripción           |
|--------|------------------|-----------------------|
| GET    | /LeerTodo        | Lista paginada        |
| POST   | /Crear           | Registra suministro   |
| PUT    | /Actualizar/{id} | Actualiza suministro  |
| DELETE | /Eliminar        | Soft delete           |

---

### Características de Computadoras — `/api/caracteristicas`

| Método | Ruta             | Descripción                         |
|--------|------------------|-------------------------------------|
| GET    | /LeerTodo        | Lista de specs de computadoras      |
| POST   | /Crear           | Registra especificaciones           |
| PUT    | /Actualizar/{id} | Actualiza specs                     |
| DELETE | /Eliminar        | Soft delete                         |

---

### Historial de Préstamos — `/api/historialprestamos`

| Método | Ruta      | Descripción                          |
|--------|-----------|--------------------------------------|
| GET    | /LeerTodo | Historial paginado de préstamos      |

---

### Reportes — `/api/reportes`

| Método | Ruta                     | Descripción                                |
|--------|--------------------------|--------------------------------------------|
| GET    | /InventarioTotal         | Conteo total de equipos por tipo           |
| GET    | /PrestamosPorMes         | Préstamos y devoluciones agrupados por mes |
| GET    | /EquiposPrestadosPorTipo | Equipos prestados por tipo y mes           |

---

## 6. Autenticación y seguridad

### JWT Bearer Token

1. El cliente hace `POST /login` con credenciales.
2. El servidor devuelve un token JWT firmado.
3. El cliente incluye el token en cada petición:
   ```
   Authorization: Bearer eyJhbGci...
   ```

### Configuración del token

| Parámetro   | Valor             |
|-------------|-------------------|
| Issuer      | `GestorAdmi`      |
| Audience    | `GestorAdmiClient`|
| Expiración  | 8 horas           |
| Algoritmo   | HMAC-SHA256       |

### Contraseñas

Las contraseñas se almacenan con **BCrypt** (factor de costo adaptativo). Nunca se guardan en texto plano.

### CORS

- **Producción:** solo permite orígenes configurados en `appsettings.Production.json`.
- **Desarrollo:** permite todos los orígenes.

---

## 7. Base de datos

### Servidor
- **Host:** `TICSADMI.mssql.somee.com`
- **Base de datos:** `TICSADMI`
- **Motor:** SQL Server (via Entity Framework Core 8)

### Tablas

| Tabla                        | Entidad               | Descripción                          |
|------------------------------|-----------------------|--------------------------------------|
| `gestion_hardware`           | Hardware              | Inventario de equipos                |
| `caracteristicas_computadora`| CaracteristicaComputadora | Specs de PCs (RAM, ROM, CPU)    |
| `departamentos`              | Departamento          | Áreas de la organización             |
| `Custodios`                  | Custodio              | Responsables de equipos              |
| `gestion_activos`            | GestionActivo         | Asignaciones equipo-custodio         |
| `Kits`                       | Kit                   | Conjuntos de equipos                 |
| `Personal`                   | Persona               | Empleados                            |
| `suministros_remanufacturados`| Suministro           | Consumibles y materiales             |
| `control_activos`            | ControlActivo         | Auditoría de activos                 |
| `historial_custodios`        | HistorialCustodio     | Historial de cambios de custodio     |
| `Usuarios`                   | Usuario               | Cuentas de acceso a la API           |

### Características del modelo de datos

- **Soft delete:** campo `Borrado (bit)` en todas las tablas.
- **Auditoría:** campos `CreatedAt` y `UpdatedAt` en todas las tablas.
- **Reintentos automáticos:** EF Core reintenta conexión hasta 5 veces ante fallos transitorios.

---

## 8. Generación de reportes

### PDF (iTextSharp)

Los PDFs se generan en memoria y se envían directamente como respuesta HTTP:

- **Acta de entrega** (`GET /api/gestionactivos/acta`): documento formal de asignación de equipo.
- **Acta de devolución** (`GET /api/gestionactivos/devolucion`): documento de regreso de equipo.
- **Inventario de hardware** (`GET /api/hardware/GenerarActa`): reporte del inventario completo.
- **Listado de custodios** (`GET /api/custodios/acta`): reporte de responsables.

### Excel (EPPlus)

- **Inventario Excel** (`GET /api/hardware/GenerarActaExcel`): hoja de cálculo con todo el inventario.

---

## 9. Configuración por entorno

| Archivo                          | Entorno     | Uso                                   |
|----------------------------------|-------------|---------------------------------------|
| `appsettings.json`               | Base        | Valores por defecto / desarrollo      |
| `appsettings.Development.json`   | Development | Token 24h, log Debug, Swagger activo  |
| `appsettings.Production.json`    | Production  | BD real, CORS estricto, log Warning   |

### Variables clave en producción

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "...TICSADMI.mssql.somee.com..."
  },
  "Jwt": {
    "Key": "<mínimo 32 caracteres, cambiar antes de ir a producción>",
    "ExpirationHours": 8
  },
  "Cors": {
    "AllowedOrigins": ["https://CNEAPI.somee.com"]
  }
}
```

---

## 10. Pruebas

### Pruebas unitarias (`tests/GestorAdmi.Tests.Unit`)

- **Framework:** xUnit
- **Mocking:** Moq
- **Aserciones:** FluentAssertions
- **Cobertura:** handlers de comandos y queries

```bash
dotnet test tests/GestorAdmi.Tests.Unit/GestorAdmi.Tests.Unit.csproj
```

| Clase de prueba                          | Qué prueba                         |
|------------------------------------------|------------------------------------|
| `CreateHardwareCommandHandlerTests`      | Creación de hardware con repo mock |
| `GetHardwareQueryHandlerTests`           | Consulta paginada de hardware      |
| `LoginCommandHandlerTests`               | Flujo completo de autenticación    |

### Pruebas de integración (`tests/GestorAdmi.Tests.Integration`)

- **Framework:** xUnit + WebApplicationFactory
- **Base de datos:** En memoria (aislada por prueba)
- **Incluye:** `DatabaseSeeder` para datos de prueba

```bash
dotnet test tests/GestorAdmi.Tests.Integration/GestorAdmi.Tests.Integration.csproj
```

| Clase de prueba             | Qué prueba                              |
|-----------------------------|------------------------------------------|
| `AuthIntegrationTests`      | Login completo via HTTP                  |
| `HardwareIntegrationTests`  | CRUD de hardware via HTTP                |

---

## 11. Despliegue a somee.com

### Pre-requisitos

1. .NET 10.0 SDK instalado localmente.
2. PowerShell 5.1+ (viene con Windows).
3. Acceso FTP a `ftp://CNEAPI.somee.com`.

### Comando de despliegue

```powershell
# Desde la raíz del proyecto:
.\deploy.ps1
```

El script realiza automáticamente:

```
[1] Restaurar NuGet packages
[2] Correr pruebas unitarias       ← Aborta si fallan
[3] Correr pruebas de integración  ← Aborta si fallan
[4] Publicar proyecto (Release)
[5] Subir todo por FTP a somee.com
```

### Opciones del script

```powershell
.\deploy.ps1 -SkipTests    # Omite las pruebas (no recomendado)
.\deploy.ps1 -WhatIf       # Modo simulación: muestra pasos sin ejecutarlos
```

### Estructura en somee.com

```
www.CNEAPI.somee.com/
├── API.dll               ← Punto de entrada de la aplicación
├── web.config            ← Configuración IIS
├── appsettings.json
├── appsettings.Production.json
├── *.dll                 ← Dependencias
└── logs/                 ← Directorio de logs (creado automáticamente)
```

---

## 12. Dependencias principales

| Paquete                                  | Versión | Propósito                          |
|------------------------------------------|---------|------------------------------------|
| MediatR                                  | 12.2.0  | Patrón Mediator / CQRS             |
| AutoMapper                               | 12.0.1  | Mapeo de objetos                   |
| FluentValidation                         | 11.8.0  | Validación de entradas             |
| Microsoft.EntityFrameworkCore.SqlServer  | 8.0.0   | ORM + SQL Server                   |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.0 | Autenticación JWT             |
| BCrypt.Net-Next                          | 4.1.0   | Hash de contraseñas                |
| Serilog.AspNetCore                       | 8.0.0   | Logging estructurado               |
| Swashbuckle.AspNetCore                   | 6.5.0   | Documentación Swagger/OpenAPI      |
| EPPlus                                   | 7.0.0   | Generación de archivos Excel       |
| iTextSharp                               | 3.4.10  | Generación de archivos PDF         |
| xUnit                                    | 2.x     | Framework de pruebas               |
| Moq                                      | 4.x     | Mocking para pruebas unitarias     |
| FluentAssertions                         | 6.x     | Aserciones legibles                |

---

*Documentación generada el 2026-03-19.*
