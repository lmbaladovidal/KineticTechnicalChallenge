
# Kinetic Technical Challenge
## Descripción del Proyecto
Este proyecto es una aplicación de consola en C# que permite analizar documentos de texto y generar un resumen de su contenido. 
Utiliza SQLite para almacenar los documentos y sus resúmenes, y está diseñado para ser fácil de usar, mantener y escalar.

## Desiciones Tecnicas
### Lenguaje y Plataforma
El proyecto está desarrollado en C# utilizando .NET 8.0, lo que permite aprovechar las últimas características del lenguaje y la plataforma.
### Base de Datos
Se utiliza SQLite como base de datos para almacenar los documentos y sus resúmenes. SQLite es una base de datos ligera y fácil de usar, ideal para aplicaciones de consola y proyectos pequeños.
### Estructura del Proyecto
El proyecto está estructurado en varias carpetas para organizar el código de manera clara:
- **KineticTechnicalChallenge**: Contiene la solución principal del proyecto.
- **KineticTechnicalChallenge.Core**: Contiene la lógica de negocio y las entidades del dominio.
- **KineticTechnicalChallenge.Data**: Contiene la implementación de acceso a datos y la configuración de la base de datos.
- **KineticTechnicalChallenge.ConsoleApp**: Contiene la aplicación de consola que interactúa con el usuario.
- **KineticTechnicalChallenge.Tests**: Contiene las pruebas unitarias para asegurar la calidad del código.

## Instalación y Ejecución
### Requisitos Previos
- .NET 8.0 SDK instalado en tu máquina.
- SQLite instalado o la capacidad de utilizar SQLite a través de NuGet.
- Un editor de código como Visual Studio o Visual Studio Code.
- Conexión a Internet para descargar dependencias y herramientas necesarias.
- Conocimientos básicos de C# y .NET.
- Conocimientos básicos de bases de datos y SQL.

### Pasos para la Instalación
- Clonar el repositorio del proyecto desde GitHub.
-Ajustar el appSettings.json
```json
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DocumentProcessor.db"
  },
  "TextProcessing": {
    "InputFolder": "./TextFiles",
    "BatchSize": 5
  },
```
-Abrir la solución en tu editor de código.
-Asegúrarse de que la cadena de conexión a la base de datos sea correcta. Se puede cambiar el nombre del archivo de la base de datos de ser necesario.
-Donde `InputFolder` es la carpeta donde se encuentran los archivos de texto a procesar y `BatchSize` es el número de archivos que se procesarán en cada lote.

### Ejecución del Proyecto
- Abrir una terminal y navegar a la carpeta del proyecto.
- Ejecuta el siguiente comando para restaurar las dependencias:
-
```bash
dotnet restore
```

- Ejecuta el siguiente comando para compilar el proyecto:
-
```bash
dotnet build
```

- Ejecuta el siguiente comando para iniciar la aplicación de consola:
- 
```bash
dotnet run --project KineticTechnicalChallenge.ConsoleApp
```

- La aplicación solicitará la ruta de la carpeta que contiene los archivos de texto a procesar. Asegúrate de que los archivos estén en el formato correcto (texto plano).
- La aplicación procesará los archivos en lotes, generando resúmenes y almacenándolos en la base de datos SQLite.
- Puedes consultar los resúmenes generados en la base de datos utilizando herramientas como DBeaber o cualquier cliente SQLite.
- Para ejecutar las pruebas unitarias, utiliza el siguiente comando:

```bash
dotnet test KineticTechnicalChallenge.Tests
```


## Uso de herramientas de Inteligencia Artificial

Durante el desarrollo de este proyecto se utilizaron herramientas de IA para asistir en diversas tareas. 

Herramientas utilizadas
ChatGPT (OpenAI, modelo GPT-4o)
https://chatgpt.com/share/68575326-9074-8013-92a3-5f230991ba8d
Claude Sonnet 4
https://claude.ai/share/a626a308-922a-4fce-91f4-c2bae842ebc6

La IA fue utilizada para acelerar el proceso de construccion del proyecto y obtener ejemplos de codigo 
dado que nunca realize un proyecto que analize documentos de texto creando sumarios y ademas, utilizando SQLite.

Si bien hay codigo generado por IA en la solucion es muy poco el cual permanece tal cual fue generado. Los codigos generados con ia
dicen en su cabecera "Gen With IA"
