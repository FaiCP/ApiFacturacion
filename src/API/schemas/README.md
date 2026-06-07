# Esquemas XSD del SRI

Coloca aquí los esquemas oficiales del SRI para activar la validación XSD
previa a la firma (capa 2 de `ComprobanteValidator`).

`ComprobanteValidator` busca cada archivo por `codDoc`:

| codDoc | Documento              | Archivo esperado            |
|--------|------------------------|-----------------------------|
| 01     | Factura                | `factura.xsd`               |
| 04     | Nota de Crédito        | `notaCredito.xsd`           |
| 05     | Nota de Débito         | `notaDebito.xsd`            |
| 06     | Guía de Remisión       | `guiaRemision.xsd`          |
| 07     | Comprobante Retención  | `comprobanteRetencion.xsd`  |

## Comportamiento

- Si el archivo XSD existe → el XML se valida contra el esquema oficial.
- Si **no** existe → la capa XSD se omite silenciosamente; las reglas de
  negocio del SRI (clave de acceso, módulo 11, coherencia de campos) siguen
  aplicándose siempre.

## Dónde obtenerlos

Portal del SRI → Facturación Electrónica → Documentación técnica / Fichas
técnicas. Descarga los XSD de la versión que uses (ej. factura v2.1.0) y
renómbralos según la tabla anterior.

Los `.xsd` se copian automáticamente al directorio de salida (ver `API.csproj`),
por lo que estarán disponibles también en el contenedor Docker / Render.
