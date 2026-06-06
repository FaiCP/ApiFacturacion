# Plan: Facturación Electrónica Ecuador — Backend .NET 10

## Stack base (no tocar)
- Clean Architecture: Domain → Application → Infrastructure → API
- CQRS + MediatR | Repository pattern | FluentValidation | AutoMapper
- JWT Auth + BCrypt | Serilog | iTextSharp (PDF) | EPPlus (Excel)
- EF Core 10 + SQL Server

---

## FASE 1 — Preparación y nuevas entidades Domain
> Crear base de datos y modelos para el dominio de facturación. Sin lógica de negocio aún.

### 1.1 NuGet packages
- [ ] Agregar `Portable.BouncyCastle` → firma digital XAdES-BES
- [ ] Agregar `System.ServiceModel.Http` → SOAP con SRI
- [ ] Verificar que iTextSharp y EPPlus estén actualizados

### 1.2 Entidades Domain (`src/Domain/Entities/`)
- [x] `Emisor.cs` — RUC, razonSocial, nombreComercial, dirección, logo, ambiente, serie establecimiento/pto emisión
- [x] `Cliente.cs` — tipoIdentificacion (RUC/CI/PASAPORTE), numeroIdentificacion, razonSocial, email, telefono, dirección
- [x] `Producto.cs` — codigoPrincipal, descripcion, precioUnitario, tarifaIva (0/15/EXENTO/NO_OBJETO), codigoSRI
- [x] `Factura.cs` — claveAcceso, numeroAutorizacion, estado (EstadoSRI enum), fecha, referencias a Cliente/Emisor, totales
- [x] `DetalleFactura.cs` — cantidad, precioUnitario, descuento, subtotal, impuesto, referencia a Producto y Factura
- [x] `ConfiguracionSRI.cs` — rutaCertificado, passwordCertificado (encrypted), ambiente (Pruebas/Produccion), activo
- [x] Enums: `EstadoSRI`, `AmbienteSRI`, `TipoIdentificacion`, `TarifaIva`, `TipoDocumentoSRI`

### 1.3 Interfaces Domain (`src/Domain/Interfaces/`)
- [x] `IEmisorRepository.cs`
- [x] `IClienteRepository.cs`
- [x] `IProductoRepository.cs`
- [x] `IFacturaRepository.cs`
- [x] `IConfiguracionSRIRepository.cs`
- [x] `ISRIService.cs` — interfaz para servicio SOAP SRI
- [x] `IXmlFacturaService.cs` — interfaz para generación XML
- [x] `IFirmaDigitalService.cs` — interfaz para firma XAdES-BES
- [x] `IRideService.cs` — interfaz para PDF RIDE

### 1.4 Validadores Domain
- [x] `RucValidator.cs` — algoritmo módulo 11 para RUC Ecuador (13 dígitos)
- [x] `CedulaValidator.cs` — algoritmo módulo 10 para CI Ecuador (10 dígitos)
- [x] `ClaveAccesoGenerator.cs` — genera clave 49 dígitos (fecha+tipo+ruc+ambiente+serie+seq+cod+emisión+verificador)

- [x] ✅ **FASE 1 COMPLETADA** — build limpio, 0 errores

---

## FASE 2 — Infraestructura y datos maestros

### 2.1 DbContext (`src/Infrastructure/Persistence/ApplicationDbContext.cs`)
- [x] Agregar `DbSet<Emisor>`
- [x] Agregar `DbSet<Cliente>`
- [x] Agregar `DbSet<Producto>`
- [x] Agregar `DbSet<Factura>`
- [x] Agregar `DbSet<DetalleFactura>`
- [x] Agregar `DbSet<ConfiguracionSRI>`
- [x] Configurar relaciones EF (Factura → DetalleFactura, Factura → Cliente, etc.)

### 2.2 Migraciones
- [x] Crear migración `AddFacturacionElectronica` — 6 tablas generadas
- [x] Aplicar migración en base de datos — `InitialCreate` + `AddFacturacionElectronica` aplicadas en DB `Facturacion`

### 2.3 Repositorios (`src/Infrastructure/Repositories/`)
- [x] `EmisorRepository.cs`
- [x] `ClienteRepository.cs`
- [x] `ProductoRepository.cs`
- [x] `FacturaRepository.cs` — queries por estado, fecha, cliente
- [x] `ConfiguracionSRIRepository.cs`

### 2.4 Módulo Emisor (CRUD)
- [x] DTOs: `EmisorDto.cs`, `EmisorCreateDto.cs`
- [x] Command: `CreateEmisorCommand.cs` + Handler
- [x] Command: `UpdateEmisorCommand.cs` + Handler
- [x] Query: `GetEmisorQuery.cs` + Handler
- [x] Validator: `CreateEmisorCommandValidator.cs` (RUC válido, series 3 dígitos)
- [x] Controller: `EmisorController.cs`

### 2.5 Módulo Clientes (CRUD)
- [x] DTOs: `ClienteDto.cs`, `ClienteCreateDto.cs`
- [x] Command: `CreateClienteCommand.cs` + Handler
- [x] Command: `UpdateClienteCommand.cs` + Handler
- [x] Query: `GetClientesQuery.cs` + Handler (paginado + búsqueda)
- [x] Query: `GetClienteByIdQuery.cs` + Handler
- [x] Validator: `CreateClienteCommandValidator.cs` (RUC/CI válido según tipo)
- [x] Controller: `ClientesController.cs`

### 2.6 Módulo Productos/Servicios (CRUD)
- [x] DTOs: `ProductoDto.cs`, `ProductoCreateDto.cs`
- [x] Command: `CreateProductoCommand.cs` + Handler
- [x] Command: `UpdateProductoCommand.cs` + Handler
- [x] Query: `GetProductosQuery.cs` + Handler (paginado + búsqueda)
- [x] Validator: `CreateProductoCommandValidator.cs` (precio > 0)
- [x] Controller: `ProductosController.cs`

- [x] ✅ **FASE 2 COMPLETADA** — build limpio, migración generada, pendiente: `dotnet ef database update`

---

## FASE 3 — Facturación core (sin integración SRI)
> Sistema funcional: crear y gestionar facturas. Sin envío al SRI aún.

### 3.1 Lógica de negocio Application
- [x] `TaxCalculatorService.cs` — calcular subtotal, IVA (0%/15%), total por línea y total factura
- [x] `FacturaNumeracionService.cs` — generar número secuencial 001-001-000000001 (por emisor)
- [x] DTOs: `FacturaDto.cs`, `FacturaCreateDto.cs`, `DetalleFacturaDto.cs`, `FacturaResumenDto.cs`

### 3.2 Commands Factura
- [x] `CreateFacturaCommand.cs` + Handler — crea factura en BORRADOR, calcula totales, genera clave acceso
- [x] `AnularFacturaCommand.cs` + Handler — BORRADOR/RECHAZADA → ANULADA (AUTORIZADA requiere NC)

### 3.3 Queries Factura
- [x] `GetFacturasQuery.cs` + Handler — paginado, filtro por estado/fecha/cliente
- [x] `GetFacturaByIdQuery.cs` + Handler — incluye detalles completos
- [x] `GenerarRideQuery.cs` + Handler

### 3.4 Validator
- [x] `CreateFacturaCommandValidator.cs` — cliente, detalles no vacíos, cantidad/precio > 0, descuento < subtotal

### 3.5 RIDE PDF (`src/Infrastructure/Services/`)
- [x] `RideService.cs` — encabezado emisor, código barras clave acceso, datos cliente, tabla detalles, totales por tarifa, autorización
- [x] Endpoint: `GET /api/facturas/{id}/ride` → devuelve PDF

### 3.6 Controller
- [x] `FacturasController.cs`
  - `GET  /api/facturas/LeerTodo` — paginado + filtros (estado, fecha, cliente)
  - `GET  /api/facturas/{id}` — detalle con detalles
  - `POST /api/facturas/Crear` — crear (BORRADOR)
  - `POST /api/facturas/{id}/anular`
  - `GET  /api/facturas/{id}/ride` — descargar PDF

- [x] ✅ **FASE 3 COMPLETADA** — build limpio, 0 errores

---

## FASE 4 — Integración SRI
> Conectar con el Servicio de Rentas Internas. Flujo: XML → Firma → Envío → Autorización.

### 4.1 Generación XML (`src/Infrastructure/Services/`)
- [x] `XmlFacturaService.cs` — XML SRI v1.0.0: infoTributaria, infoFactura, detalles con impuestos por tarifa, infoAdicional, pagos
- [x] Códigos SRI correctos: tipo doc 01, IVA código 2, codigoPorcentaje 4 (15% 2024+), tipoIdentificacion según tabla SRI

### 4.2 Firma Digital (`src/Infrastructure/Services/`)
- [x] `FirmaDigitalService.cs` — XAdES-BES con System.Security.Cryptography.Xml:
  - Carga cert X509Certificate2 desde base64 + password
  - Digest SHA-1 de comprobante, KeyInfo y SignedProperties (C14N)
  - Firma RSA-SHA1 de SignedInfo canonicalizado
  - Ensambla Signature completa y la inyecta en el XML

### 4.3 Módulo ConfiguracionSRI
- [x] DTOs: `ConfiguracionSRIDto.cs`, `ConfiguracionSRICreateDto.cs`
- [x] Command: `SaveConfiguracionSRICommand.cs` + Handler (valida cert antes de guardar, desactiva anterior)
- [x] Query: `GetConfiguracionSRIQuery.cs` + Handler
- [x] Controller: `ConfiguracionSRIController.cs`
  - `GET  /api/configuracionsri`
  - `POST /api/configuracionsri`
  - `POST /api/configuracionsri/upload-certificado` — upload archivo .p12

### 4.4 Servicio SOAP SRI (`src/Infrastructure/Services/`)
- [x] `SRIService.cs` — HttpClient SOAP manual:
  - `EnviarComprobanteAsync` → RecepcionComprobantesOffline (XML en base64)
  - `ConsultarAutorizacionAsync` → AutorizacionComprobantesOffline
  - URLs por ambiente (Pruebas/Producción)
- [x] `SriErrorTranslator.cs` — diccionario códigos SRI → mensajes legibles español

### 4.5 Commands Envío SRI
- [x] `EmitirFacturaCommand.cs` + Handler:
  1. Valida estado (no ANULADA/ya AUTORIZADA)
  2. Genera XML → Firma XAdES-BES → Envía SOAP
  3. Si recepción OK: estado ENVIADA, espera 2s
  4. Consulta autorización → AUTORIZADA o RECHAZADA + motivo
- [x] Endpoint: `POST /api/facturas/{id}/emitir`
- [x] HttpClient SRI registrado en Program.cs con timeout 30s

### 4.6 Manejo de errores SRI
- [x] SriErrorTranslator — traduce códigos 43,44,45,47,48,49,60,61,64,65,70 + AUTH-CERT-*
- [x] MotivoRechazo guardado en Factura

- [x] ✅ **FASE 4 COMPLETADA** — build limpio, 0 errores

---

## FASE 5 — Documentos adicionales

### 5.1 Notas de Crédito
- [x] Entidades: `NotaCredito.cs` + `DetalleNotaCredito.cs`
- [x] XML SRI tipo 04: infoNotaCredito (numDocModificado, motivo), detalles, totalConImpuestos
- [x] CreateNotaCreditoCommand — solo sobre facturas AUTORIZADAS, clave acceso tipo 04
- [x] EmitirNotaCreditoCommand — flujo: XML → firma → SOAP → autorización
- [x] Controller: `NotasCreditoController.cs`

- [x] ✅ **SUB-FASE 5.1 COMPLETADA**

### 5.2 Notas de Débito
- [x] Entidades: `NotaDebito.cs` + `MotivoNotaDebito.cs`
- [x] XML SRI tipo 05: infoNotaDebito (motivos, impuestos, pagos)
- [x] CreateNotaDebitoCommand — calcula IVA 15% sobre suma de motivos
- [x] EmitirNotaDebitoCommand
- [x] Controller: `NotasDebitoController.cs`

- [x] ✅ **SUB-FASE 5.2 COMPLETADA**

### 5.3 Retenciones
- [x] Entidades: `Retencion.cs` + `DetalleRetencion.cs` + enum `TipoImpuestoRetencion`
- [x] XML SRI tipo 07: infoCompRetencion (periodoFiscal, sujetoRetenido), impuestos IR/IVA
- [x] CreateRetencionCommand — valorRetenido = base × porcentaje / 100
- [x] EmitirRetencionCommand
- [x] Controller: `RetencionesController.cs`
- [x] Migración `AddDocumentosAdicionales` aplicada en DB

- [x] ✅ **SUB-FASE 5.3 COMPLETADA**

- [x] ✅ **FASE 5 COMPLETADA** — build 0 errores, 6 tablas nuevas

---

## FASE 6 — Reportes y exportación

### 6.1 Endpoints reportes tributarios
- [x] `GET /api/reportes/facturas-por-mes?anio=` — cantidad + monto + IVA por mes
- [x] `GET /api/reportes/iva-por-mes?anio=` — base 0%, base 15%, IVA, importe total por mes
- [x] `GET /api/reportes/facturas-por-estado` — conteo y monto por estado SRI
- [x] `GET /api/reportes/top-clientes?top=10&anio=` — top clientes por monto facturado

### 6.2 ATS (Anexo Transaccional Simplificado)
- [x] `AtsExcelService.cs` — 3 hojas: Resumen, Ventas (facturas), Retenciones
- [x] `GET /api/reportes/ats/{anio}/{mes}` → descarga Excel ATS (.xlsx)
- [x] `IReporteFacturacionRepository` en Domain — Clean Architecture respetada

### 6.3 XML masivo / reenvío
- [x] `GET /api/facturas/exportar-xml?desde=&hasta=` — ZIP con XMLs firmados del período

- [x] ✅ **FASE 6 COMPLETADA** — build 0 errores

---

## Estado general del proyecto

| Fase | Descripción | Estado |
|------|-------------|--------|
| 1 | Entidades Domain + validadores Ecuador | ✅ Completada |
| 2 | Infraestructura + datos maestros | ✅ Completada |
| 3 | Facturación core (sin SRI) | ✅ Completada |
| 4 | Integración SRI (XML + firma + SOAP) | ✅ Completada |
| 5 | Documentos adicionales (NC, ND, Ret.) | ✅ Completada |
| 6 | Reportes + ATS | ✅ Completada |

---

## Notas técnicas

### Clave de acceso (49 dígitos)
```
[fecha 8] [tipo doc 2] [ruc 13] [ambiente 1] [serie 6] [secuencial 9] [cod numerico 8] [tipo emision 1] [verificador 1]
```

### Ambientes SRI
```
Pruebas:    https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl
Producción: https://cel.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl
```

### IVA vigente Ecuador 2024+
```
Código 2 → 15% (tarifa general)
Código 0 → 0%
Código 6 → Exento
Código 7 → No Objeto de IVA
```

### Tipos de comprobante
```
01 → Factura
03 → Liquidación de compra
04 → Nota de Crédito
05 → Nota de Débito
06 → Guía de Remisión
07 → Comprobante de Retención
```
