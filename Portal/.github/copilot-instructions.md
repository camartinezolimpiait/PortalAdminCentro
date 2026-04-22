# GitHub Copilot - Instrucciones y Reglas de Uso
​
Este archivo define las reglas y recomendaciones para el uso de GitHub Copilot en toda la solución.
​
## Alcance
​
Estas reglas aplican para todos los proyectos y archivos dentro de la solución.
​
## Reglas Generales
​
1. **Propósito**: Utilizar Copilot para acelerar el desarrollo, sugerir código y mejorar la productividad.
2. **Revisión de Código**: Todo el código generado por Copilot debe ser revisado manualmente antes de integrarse al proyecto.
3. **Licencias y Copyright**: Verificar que las sugerencias de Copilot no incluyan código con licencias incompatibles o contenido protegido por derechos de autor.
4. **Buenas Prácticas**: Priorizar las mejores prácticas de desarrollo en .NET 8 y mantener la calidad del código.
5. **Seguridad**: Validar que el código sugerido no introduzca vulnerabilidades de seguridad.
6. **Documentación**: Documentar adecuadamente el código generado por Copilot, especialmente si se trata de lógica compleja o funciones críticas.
7. **Identificación de Métodos Generados**: Todo método generado por GitHub Copilot debe incluir un comentario al inicio que indique claramente que fue generado por Copilot. Ejemplo: `// Método generado por GitHub Copilot`
8. **Marcado de Fragmentos de Código**: Cuando se genere un fragmento de código con GitHub Copilot, se debe indicar el inicio y el fin del fragmento con comentarios claros, por ejemplo:
   // Inicio código generado por GitHub Copilot
   ...código...
   // Fin código generado por GitHub Copilot
9. **Documentación de Refactorizaciones y Optimizaciones**: Toda refactorización u optimización de código realizada por medio de GitHub Copilot debe incluir un comentario al inicio y al final del bloque indicando que fue realizada por Copilot. Ejemplo:
   // Inicio refactorización/optimización por GitHub Copilot
   ...código refactorizado...
   // Fin refactorización/optimización por GitHub Copilot
10. **Notación Pascal para Métodos**: Todo nombre de método generado por GitHub Copilot debe estar en notación Pascal (la primera letra de cada palabra en mayúscula, sin guiones bajos).
11. **Notación Pascal para Variables Globales**: Todas las variables globales (campos) en una clase deben estar en notación Pascal (la primera letra de cada palabra en mayúscula, sin guiones bajos).
12. **Notación Camel para Parámetros de Métodos**: Todos los parámetros de los métodos deben estar en notación camel (la primera letra en minúscula y las siguientes palabras con mayúscula inicial, sin guiones bajos).
​
## Configuración Recomendada
​
- Mantener Copilot actualizado a la última versión.
- Configurar Copilot para sugerencias contextuales y relevantes al stack .NET 8.
- Desactivar sugerencias automáticas en archivos sensibles o de configuración.
​
## Ejemplo de Uso Responsable
​
- Usar Copilot para generar funciones repetitivas, plantillas de clases, y ejemplos de pruebas unitarias.
- Evitar aceptar sugerencias sin comprender su funcionamiento.
​
---
​
Este archivo puede ser modificado para agregar reglas específicas según las necesidades del equipo o del proyecto.