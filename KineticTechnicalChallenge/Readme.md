Uso de herramientas de Inteligencia Artificial
Durante el desarrollo de este proyecto se utilizaron herramientas de IA para asistir en diversas tareas. A continuación, se detalla su uso de forma transparente, tal como requiere el enunciado.

Herramientas utilizadas
ChatGPT (OpenAI, modelo GPT-4o)

Prompt relevantes utilizados
"¿Cómo configurar SQLite en un proyecto ASP.NET Core?"

Resultado: Generación de estructura básica de DbContext, DbContextOptions, UseSqlite, y migraciones.

"¿Por qué EF Core necesita un IDesignTimeDbContextFactory?"

Resultado: Explicación teórica + ejemplo práctico para justificar su uso en el proyecto.

"Organizame el proyecto para un sistema de procesamiento de archivos con API REST y procesamiento en background"

Resultado: Sugerencia de estructura de carpetas y componentes.

Código generado o asistido por IA
Asistencia en la generación de:

DocumentContext.cs

DocumentContextFactory.cs

Configuración de SQLite con DbContextOptions

Estructura de carpetas del proyecto

Endpoints REST iniciales (/process/start, /process/status/{id}, etc.)

Modificaciones realizadas al código generado
Se adaptaron las rutas y namespaces a la solución KineticTechnicalChallenge.

Se ajustaron propiedades para cumplir con los requerimientos específicos del análisis de archivos.

Se corrigieron errores de compilación relacionados con paquetes NuGet y configuración de diseño (SetBasePath, ConfigurationBuilder, etc.).

Justificación del uso
El uso de IA permitió:

Agilizar el desarrollo inicial de la estructura del proyecto.

Reducir errores de configuración comunes en EF Core.

Obtener explicaciones detalladas de comportamientos internos de .NET y EF.

Acelerar el desarrollo de pruebas y documentación, manteniendo un enfoque pedagógico y profesional.

