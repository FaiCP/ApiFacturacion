<!-- AUTOREADME:START -->
<p align="center">
  <h1>📦 tiendaFacturacionB</h1>
</p>

<p align="center">
  <a href="https://github.com/FaiCP/ApiFacturacion/stargazers"><img src="https://img.shields.io/github/stars/FaiCP/ApiFacturacion?style=flat&color=yellow" alt="Stars" /></a>
  <a href="https://github.com/FaiCP/ApiFacturacion/commits"><img src="https://img.shields.io/github/last-commit/FaiCP/ApiFacturacion?style=flat" alt="Last Commit" /></a>
</p>

## Requirements

- Docker

## Project Structure

```
├─ src/
│  ├─ API/
│  ├─ Application/
│  ├─ Domain/
│  └─ Infrastructure/
├─ tests/
```

## Docker

```bash
docker build -t app .
```

## Contributing

1. Fork the repository
2. Create a branch: `git checkout -b feature/your-feature`
3. Commit your changes
4. Push to the branch: `git push origin feature/your-feature`
5. Open a pull request against `main`
<!-- AUTOREADME:END -->

# 🧾 Facturación Electrónica Ecuador API

![.NET 10](https://img.shields.io/badge/.NET-10.0-512bd4.svg)
![Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-blue.svg)
![Database](https://img.shields.io/badge/Database-PostgreSQL-336791.svg)
![License](https://img.shields.io/badge/License-MIT-green.svg)

API REST para **facturación electrónica multi-tenant** integrada con el SRI de Ecuador. Permite emitir facturas, notas de crédito, notas de débito y retenciones, generar y firmar el XML con certificado `.p12`, autorizarlo ante el SRI y producir el RIDE en PDF — todo por emisor (multi-tenant: cada emisor maneja su propio certificado y configuración SRI).

📖 **Swagger UI:** `/swagger` (en el host de despliegue)

---

## 🛠️ Stack Tecnológico

* **Framework:** .NET 10.0 (C#)
* **ORM:** Entity Framework Core 10 + Npgsql (PostgreSQL / Neon.tech)
* **Patrones de Diseño:** CQRS con MediatR, Repository Pattern.
* **Seguridad:** Autenticación JWT Bearer, hashing de contraseñas, firma XML con certificado `.p12`.
* **SRI:** Generación de clave de acceso, firma XAdES-BES, comunicación con webservices de recepción/autorización del SRI.
* **Reportes:** Generación de RIDE en PDF.
* **Pruebas:** xUnit, Moq, FluentAssertions y WebApplicationFactory.
* **Logging:** Serilog estructurado.
* **Despliegue:** Docker sobre Render.com (ver `Dockerfile` / `render.yaml`).

---

## 🏛️ Arquitectura del Sistema

El proyecto implementa **Clean Architecture**, dividiendo la lógica en capas con dependencias unidireccionales:

1.  **Domain:** Entidades de negocio (Emisor, Factura, Cliente, Producto, Retención, ConfiguracionSRI...), interfaces de repositorio y excepciones personalizadas.
2.  **Application:** Casos de uso (Commands/Queries vía MediatR), DTOs y validaciones.
3.  **Infrastructure:** Persistencia (EF Core + PostgreSQL), firma/comunicación con el SRI, generación de PDF, hashing y JWT.
4.  **API (Presentation):** Controladores REST, middlewares de excepciones y configuración de DI/Swagger.

---

## 📦 Módulos Principales

| Módulo | Descripción |
| :--- | :--- |
| 🔑 **Auth** | Login con JWT, roles Admin/Vendedor. |
| 🏢 **Emisores** | Registro de emisores (multi-tenant), cada uno con su propia configuración SRI y certificado `.p12`. |
| 🧾 **Facturas** | Creación, emisión, anulación, descarga de RIDE y exportación de XMLs autorizados. |
| 📝 **Notas de Crédito / Débito** | Documentos asociados a una factura existente. |
| 💰 **Retenciones** | Generación de comprobantes de retención ligados a una factura. |
| 👥 **Clientes / Productos** | Catálogos base para la emisión de comprobantes. |
| 🔐 **ConfiguracionSRI** | Carga del certificado `.p12`, ambiente (pruebas/producción) por emisor. |

---

## 🚀 Instalación y Uso Local

### Requisitos
* .NET 10 SDK
* PostgreSQL (o Docker)

### Pasos
1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/FaiCP/ApiFacturacion.git
   ```
2. **Configurar Connection String:**
   Edita `src/API/appsettings.Development.json` con tus credenciales de base de datos local (o usa `appsettings.Example.json` como plantilla).
3. **Migraciones:**
   ```bash
   dotnet ef database update --project src/Infrastructure --startup-project src/API
   ```
4. **Ejecutar:**
   ```bash
   dotnet run --project src/API
   ```

## 🧪 Estrategia de Pruebas

Dos niveles de testing:

* **Unit Tests:** Validación de Handlers y lógica de dominio de forma aislada.
* **Integration Tests:** Pruebas de endpoints HTTP con `WebApplicationFactory`.

Ejecución:
```bash
dotnet test
```

## 🚢 Despliegue

Despliegue vía **Docker en Render.com** (`render.yaml` + `Dockerfile`), con auto-deploy en cada push a `main`. Variables sensibles (connection string, `Jwt__Key`, `Cors__AllowedOrigins__*`) se configuran como variables de entorno en Render — nunca en archivos versionados.
