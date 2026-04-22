# Backend - Instrucciones y Reglas de Uso

## Propósito

Este documento describe las mejores prácticas, convenciones y estándares utilizados en la aplicación backend de MiLicencia. El backend sigue los principios de Diseño Orientado al Dominio (DDD) y aplica una arquitectura por capas para garantizar la consistencia, mantenibilidad y escalabilidad del código.
Además, este archivo proporciona instrucciones y recomendaciones para el uso de GitHub Copilot en el desarrollo y mantenimiento de la solución. Está orientado a mantener la coherencia, calidad y seguridad del código, especialmente en el contexto de la API de Pines Olimpia.

---

## Technology Stack

- **Runtime environment:** .NET Core3.1 (C#8.0)
- **Type-safe development with strict mode:** C#8.0, con tipado estático y validaciones en tiempo de compilación.
- **Web application framework:** ASP.NET Core Web API
- **Modern ORM for database access:** Dapper (implementado en la capa Infrastructure para acceso a datos)

---

## Arquitectura y estructura de capas

### Domain-Driven Design (DDD)

Domain-Driven Design es una metodología que se centra en modelar el software de acuerdo con la lógica del negocio y el conocimiento del dominio. Al enfocar el desarrollo en una comprensión profunda del dominio, DDD facilita la creación de sistemas complejos.
**Beneficios:**
- **Mejor Comunicación**: romueve un lenguaje común entre desarrolladores y expertos del dominio, mejorando la comunicación y reduciendo errores de interpretación.
- **Modelos de Dominio Claros**: Ayuda a construir modelos que reflejan con precisión las reglas y procesos del negocio.
- **Alta Mantenibilidad**: Al dividir el sistema en subdominios, facilita el mantenimiento y la evolución del software.

La solución sigue una arquitectura en capas:

- **MiLicenciaCotizador.Domain**: Define las entidades, modelos, enumeraciones y contratos (interfaces) de negocio.
    - Los **modelos** definen las entidades clave del negocio (Candidato, Posición, Solicitud, Entrevista, etc.).
    - Las **interfaces de repositorio** definen los contratos de acceso a los datos.
    - Lógica de negocio pura sin dependencias externas.
- **MiLicenciaCotizador.Application**: Implementa la lógica de negocio, servicios de aplicación y orquestación entre las entidades y repositorios.
    - Los **servicios** contienen la lógica de negocio y la orquestación.
    - **Validator** se encarga de la validación de la entrada.
    - Los **servicios** utilizan repositorios de la capa de Dominio.
- **MiLicenciaCotizador.Infrastructure**: Implementa la persistencia, acceso a datos y servicios externos, siguiendo los contratos definidos en Domain y utilizados por Application.
    - **Dapper ORM** gestiona las operaciones con la base de datos.
    - Las **implementaciones de repositorio** (a través de Dapper) cumplen con las interfaces del dominio.
- **MiLicenciaCotizador (Web/API)**: Expone los endpoints, realiza la inyección de dependencias y gestiona la interacción con los servicios de la capa Application.
    - Los **controladores** gestionan las solicitudes/respuestas HTTP.
    - Las **rutas** definen los puntos finales de la API.
    - Los **controladores** utilizan servicios de la capa de Aplicación.

**Regla principal:**
> Todo endpoint nuevo debe interactuar únicamente con servicios definidos en la capa Application, los cuales a su vez utilizan entidades y contratos de la capa Domain y la infraestructura de persistencia definida en Infrastructure. No se debe acceder directamente a entidades, repositorios o servicios de Infrastructure desde los controladores.

---

## Database & ORM

- **Relational database:** SQL Server (acceso a través de la capa Infrastructure)
- **Type-safe database client:** Dapper (provee acceso seguro y tipado a la base de datos desde C#)
- **Database migration tool:** Las migraciones y cambios de esquema se gestionan manualmente o mediante scripts SQL, ya que Dapper no incluye un sistema de migraciones integrado.

## Estándares de codificación

- **Lenguaje principal:** C#8.0
- **Framework objetivo:** .NET Core3.1
- **Estilo:** Seguir las convenciones de nomenclatura de .NET (PascalCase para clases y métodos, camelCase para variables locales y parámetros).
- **Documentación:** Usar comentarios XML para métodos públicos y controladores.
- **Manejo de errores:** Implementar bloques `try-catch` en todos los endpoints públicos. Registrar los errores usando el repositorio de pasarela (`_gatewayRepository.RegisterExceptionLog`).

- ## Buenas prácticas

- **Inyección de dependencias:** Utilizar el constructor para inyectar servicios y repositorios.
- **Serialización:** Usar `Global.SerializeJson` y `JsonConvert` para la serialización/deserialización de objetos.
- **Validaciones:** Validar los datos de entrada antes de procesar la lógica de negocio.
- **Auditoría:** Registrar acciones relevantes con `gatewayRepository.RegisterAuditLog` cuando corresponda.
- **Respuestas:** Estandarizar las respuestas de los endpoints usando DTOs y objetos de respuesta definidos en la solución.
- **Separación de responsabilidades:** La lógica de negocio debe residir en la capa Application. Los controladores solo gestionan la entrada/salida y delegan la lógica.
- **Persistencia y servicios externos:** Toda interacción con bases de datos o servicios externos debe realizarse a través de Infrastructure, nunca directamente desde el controlador ni desde Application sin usar los contratos definidos en Domain.

## 💡 Principios de Diseño Orientado al Dominio (DDD)

### Entidades (*Entities*)

Las **Entidades** son objetos con una **identidad distintiva** que persiste a lo largo del tiempo.

**Características Clave:**
* **Identidad**: Se definen por un identificador único global (ID), no por sus atributos.
* **Mutabilidad**: Sus atributos pueden cambiar, pero su ID permanece constante.
* **Ejemplos**: `MessageEntity` (Mensaje), `Position` (Puesto), `Interview` (Entrevista).

---

## Estructura recomendada para nuevos endpoints

1. **Atributos de ruta y método HTTP**  
   Ejemplo: `[HttpPost("nombreEndpoint")]`

2. **Manejo de dependencias**  
   Usar servicios inyectados por constructor (servicios de la capa Application).

3. **Validación de entrada**  
   Validar el modelo recibido antes de procesar.

4. **Delegación de lógica**  
   Llamar a un método del servicio de Application para procesar la solicitud.

5. **Manejo de errores**  
   Implementar `try-catch` y registrar errores.

6. **Respuesta estándar**  
   Retornar DTOs definidos en la capa Domain.

---

# Configuración Recomendada

- Mantener Copilot actualizado a la última versión.
- Configurar Copilot para sugerencias contextuales y relevantes al stack .NET Core3.1.
- Desactivar sugerencias automáticas en archivos sensibles o de configuración.

## Ejemplo de Uso Responsable

- Usar Copilot para generar funciones repetitivas, plantillas de clases, y ejemplos de pruebas unitarias.
- Evitar aceptar sugerencias sin comprender su funcionamiento.


## Ejemplo de endpoint recomendado

```csharp
/// <summary>
/// Obtiene el estado de un pin por su número.
/// </summary>
/// <param name="request">Modelo con el número de pin a consultar.</param>
/// <returns>Estado del pin consultado.</returns>
[HttpPost("ObtenerEstadoPin")]
public async Task<ResponseDTO<EstadoPinDTO>> ObtenerEstadoPin([FromBody] ConsultaEstadoPinRequest request)
{
    try
    {
        // Validación de entrada
        if (request == null || string.IsNullOrWhiteSpace(request.NumeroPin))
            throw new ArgumentException("El número de pin es obligatorio.", nameof(request));

        // Lógica de negocio delegada a Application
        var estadoPin = await _estadoPinService.ObtenerEstadoPinAsync(request.NumeroPin);

        // Auditoría
        await _gatewayRepository.RegisterAuditLog("ObtenerEstadoPin", null, estadoPin, DateTime.Now, null, null, null, "", "0");

        // Respuesta estándar
        return new ResponseDTO<EstadoPinDTO>
        {
            Codigo = 0,
            Respuesta = "OK",
            Data = estadoPin
        };
    }
    catch (Exception ex)
    {
        string requestJson = Global.SerializeJson(request);
        await _gatewayRepository.RegisterExceptionLog("ObtenerEstadoPin", ex, requestJson);
        return new ResponseDTO<EstadoPinDTO>
        {
            Codigo = -2,
            Respuesta = "Ha ocurrido un error. Por favor vuelva a intentarlo más tarde"
        };
    }
}
```

---

## Ejemplo de endpoint de conexión con ApiGatewayOcelot

```csharp
/// <summary>
/// Obtiene la lista de centros marketplace desde el ApiGatewayOcelot.
/// </summary>
/// <returns>Lista de centros marketplace.</returns>
[HttpGet("ObtenerCentrosMarketplace")]
public async Task<ResponseDTO<List<CenterMarketplace>>> ObtenerCentrosMarketplace()
{
    try
    {
        // Lógica de negocio delegada a Application
        var centros = await _sigCrcServices.GetMarketplaceCenters();

        // Auditoría
         // Auditoría
        await _gatewayRepository.RegisterAuditLog("ObtenerEstadoPin", null, estadoPin, DateTime.Now, null, null, null, "", "0");

        // Respuesta estándar
        return new ResponseDTO<List<CenterMarketplace>>
        {
            Codigo = 0,
            Respuesta = "OK",
            Data = centros
        };
    }
    catch (Exception ex)
    {
        string requestJson = Global.SerializeJson(request);
        await _gatewayRepository.RegisterExceptionLog("ObtenerEstadoPin", ex, requestJson);
        return new ResponseDTO<List<CenterMarketplace>>
        {
            Codigo = -2,
            Respuesta = "Ha ocurrido un error. Por favor vuelva a intentarlo más tarde"
        };
    }
}
```

---

## Seguridad

- No exponer información sensible en los logs ni en las respuestas.
- Validar y sanitizar todos los datos recibidos por los endpoints.
- Usar DTOs para limitar la información expuesta.

---

## Otros

- Mantener la coherencia en la estructura de carpetas y nombres de archivos.
- Actualizar este archivo si se realizan cambios significativos en la arquitectura o estándares.

---