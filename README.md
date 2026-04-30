<!-- AUTOREADME:START -->
<p align="center">
  <h1>📦 GestorAdmi.Core</h1>
</p>

<p align="center">
  <a href="https://github.com/FaiCP/ApiGetionCNE/stargazers"><img src="https://img.shields.io/github/stars/FaiCP/ApiGetionCNE?style=flat&color=yellow" alt="Stars" /></a>
  <a href="https://github.com/FaiCP/ApiGetionCNE/commits"><img src="https://img.shields.io/github/last-commit/FaiCP/ApiGetionCNE?style=flat" alt="Last Commit" /></a>
</p>

## Project Structure

```
├─ publish/
├─ src/
├─ tests/
```

## Contributing

1. Fork the repository
2. Create a branch: `git checkout -b feature/your-feature`
3. Commit your changes
4. Push to the branch: `git push origin feature/your-feature`
5. Open a pull request against `main`
<!-- AUTOREADME:END -->

# 🚀 GestorAdmi Core API

![.NET 10](https://img.shields.io/badge/.NET-10.0-512bd4.svg)
![Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-blue.svg)
![Database](https://img.shields.io/badge/Database-SQL_Server-red.svg)
![License](https://img.shields.io/badge/License-MIT-green.svg)

**GestorAdmi Core** es una API REST robusta diseñada para la gestión integral de activos tecnológicos. Permite el control de inventario, asignación de custodios, gestión de suministros y generación automatizada de documentos legales en PDF y Excel.

📍 **Producción:** [CNEAPI.somee.com](https://CNEAPI.somee.com)  
📖 **Swagger UI:** [Ver Documentación Interactiva](https://CNEAPI.somee.com/swagger)

---

## 🛠️ Stack Tecnológico

* **Framework:** .NET 10.0 (C#)
* **ORM:** Entity Framework Core 8 (SQL Server)
* **Patrones de Diseño:** CQRS, Mediator (MediatR), Repository Pattern, Unit of Work.
* **Seguridad:** Autenticación JWT Bearer & Hashing de contraseñas con BCrypt.
* **Reportes:** iTextSharp (PDF) & EPPlus (Excel).
* **Pruebas:** xUnit, Moq, FluentAssertions y WebApplicationFactory.
* **Logging:** Serilog estructurado.

---

## 🏛️ Arquitectura del Sistema

El proyecto implementa **Clean Architecture**, dividiendo la lógica en capas con dependencias unidireccionales para garantizar un código mantenible, testeable y desacoplado:

1.  **Domain:** Entidades de negocio, interfaces de repositorio y excepciones personalizadas.
2.  **Application:** Casos de uso (Commands/Queries), DTOs y lógica de validación (FluentValidation).
3.  **Infrastructure:** Implementación de persistencia, servicios de correo/reportes y seguridad.
4.  **API (Presentation):** Controladores REST, Middlewares de excepciones y configuración de DI.

---

## 📦 Módulos Principales

| Módulo | Descripción |
| :--- | :--- |
| 🔑 **Auth** | Gestión de acceso con tokens JWT expirables. |
| 💻 **Hardware** | Inventario de equipos con búsqueda paginada avanzada. |
| 👤 **Custodios** | Administración de responsables de activos. |
| 🔄 **Gestión de Activos** | Flujo completo de asignación y devolución con actas digitales. |
| 📈 **Reportes** | Estadísticas mensuales y dashboard de estado del inventario. |

---

## 🚀 Instalación y Uso Local

### Requisitos
* .NET 10 SDK
* SQL Server

### Pasos
1. **Clonar el repositorio:**
   ```bash
   git clone [https://github.com/tu-usuario/GestorAdmi.Core.git](https://github.com/tu-usuario/GestorAdmi.Core.git)
Configurar Connection String:
Edita el archivo src/API/appsettings.Development.json con tus credenciales de base de datos local.

Migraciones:

Bash
dotnet ef database update --project src/Infrastructure --startup-project src/API
Ejecutar:

Bash
dotnet run --project src/API
🧪 Estrategia de Pruebas
Se garantiza la integridad del sistema mediante dos niveles de testing:

Unit Tests: Validación de Handlers y lógica de dominio de forma aislada.

Integration Tests: Pruebas de endpoints HTTP utilizando una base de datos en memoria para simular escenarios reales.

Ejecución de tests:

Bash
dotnet test
🚢 Despliegue Automatizado
El repositorio incluye un script de PowerShell deploy.ps1 que gestiona el ciclo de vida del despliegue a Somee.com:

Restauración y compilación.

Ejecución obligatoria de pruebas (el despliegue se detiene si fallan).

Publicación del artefacto en modo Release.

Carga vía FTP al servidor de producción.
