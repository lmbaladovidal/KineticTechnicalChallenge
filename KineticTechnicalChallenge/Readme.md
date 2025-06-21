Uso de herramientas de Inteligencia Artificial
Durante el desarrollo de este proyecto se utilizaron herramientas de IA para asistir en diversas tareas. A continuaci�n, se detalla su uso de forma transparente, tal como requiere el enunciado.

Herramientas utilizadas
ChatGPT (OpenAI, modelo GPT-4o)

Prompt relevantes utilizados
"�C�mo configurar SQLite en un proyecto ASP.NET Core?"

Resultado: Generaci�n de estructura b�sica de DbContext, DbContextOptions, UseSqlite, y migraciones.

"�Por qu� EF Core necesita un IDesignTimeDbContextFactory?"

Resultado: Explicaci�n te�rica + ejemplo pr�ctico para justificar su uso en el proyecto.

"Organizame el proyecto para un sistema de procesamiento de archivos con API REST y procesamiento en background"

Resultado: Sugerencia de estructura de carpetas y componentes.

C�digo generado o asistido por IA
Asistencia en la generaci�n de:

DocumentContext.cs

DocumentContextFactory.cs

Configuraci�n de SQLite con DbContextOptions

Estructura de carpetas del proyecto

Endpoints REST iniciales (/process/start, /process/status/{id}, etc.)

Modificaciones realizadas al c�digo generado
Se adaptaron las rutas y namespaces a la soluci�n KineticTechnicalChallenge.

Se ajustaron propiedades para cumplir con los requerimientos espec�ficos del an�lisis de archivos.

Se corrigieron errores de compilaci�n relacionados con paquetes NuGet y configuraci�n de dise�o (SetBasePath, ConfigurationBuilder, etc.).

Justificaci�n del uso
El uso de IA permiti�:

Agilizar el desarrollo inicial de la estructura del proyecto.

Reducir errores de configuraci�n comunes en EF Core.

Obtener explicaciones detalladas de comportamientos internos de .NET y EF.

Acelerar el desarrollo de pruebas y documentaci�n, manteniendo un enfoque pedag�gico y profesional.

