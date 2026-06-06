# Plan de API reutilizable v1

## Objetivo
Convertir el backend actual de facturación electrónica en una API pública versionada, estable y consumible por otros proyectos, especialmente otros backends.

## Estado actual
- La API ya existe en `src/API` y usa Clean Architecture, CQRS, MediatR, JWT, Swagger y respuestas tipo `ApiResponse<T>`.
- Las rutas actuales no están versionadas de forma explícita.
- Los errores se devuelven con un formato distinto al resto de respuestas.
- La documentación pública existe, pero no define un contrato de integración estable para terceros.

## Alcance de implementación
- Publicar todas las rutas externas bajo `api/v1`.
- Mantener controladores delgados y dejar la lógica en `Application` e `Infrastructure`.
- Unificar el formato de respuesta de éxito y error.
- Ajustar Swagger/OpenAPI para reflejar el contrato público.
- Actualizar pruebas de integración para usar las rutas y el contrato nuevos.

## Cambios previstos
1. Rutas y versionado
   - Prefijo común `api/v1` para todos los controladores públicos.
   - Ruta de login alineada con el mismo esquema versionado.
2. Contrato HTTP
   - `ApiResponse<T>` como envoltorio estándar.
   - Errores con el mismo esquema JSON que el resto de la API.
   - Códigos HTTP consistentes para validación, autenticación y not-found.
3. Documentación
   - Swagger apuntando al contrato v1.
   - Descripciones mínimas de autenticación, paginación y errores.
4. Pruebas
   - Ajustar pruebas de integración a las rutas v1.
   - Verificar login, autorización y al menos un endpoint protegido.

## Criterios de aceptación
- [x] Un consumidor externo puede descubrir la API desde Swagger y usarla sin leer el código.
- [x] Todas las rutas públicas responden bajo `api/v1`.
- [x] Las respuestas de error y éxito comparten un formato estable.
- [x] Las pruebas pasan con las rutas nuevas.

## Suposiciones
- No se requiere compatibilidad hacia atrás con las rutas actuales.
- El consumo principal será entre servicios, no solo desde frontend.
- La base funcional de negocio no cambia; solo el contrato público y la exposición HTTP.

---

## ✅ COMPLETADO

| Ítem | Detalle | Estado |
|------|---------|--------|
| Rutas versionadas | Todos los controladores usan `api/v1/...` | ✅ |
| Login versionado | `POST /api/v1/login` | ✅ |
| `ApiResponse<T>` éxito | Todos los endpoints devuelven `ApiResponse<T>` | ✅ |
| `ApiResponse<T>` errores | `GlobalExceptionHandlerMiddleware` usa mismo esquema | ✅ |
| HTTP codes consistentes | 400/401/404/500 correctos | ✅ |
| Swagger título/descripción | Refleja facturación electrónica Ecuador + auth flow + paginación + errores | ✅ |
| Pruebas de integración | 6 tests: login, 401, endpoint protegido (clientes CRUD) | ✅ |
| Build 0 errores | | ✅ |
| 9/9 tests pasan | 3 unit + 6 integration | ✅ |
