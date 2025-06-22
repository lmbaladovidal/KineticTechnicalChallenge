
# Kinetic Technical Challenge
## Descripci�n del Proyecto
Este proyecto es una aplicaci�n de consola en C# que permite analizar documentos de texto y generar un resumen de su contenido. 
Utiliza SQLite para almacenar los documentos y sus res�menes, y est� dise�ado para ser f�cil de usar, mantener y escalar.

## Desiciones Tecnicas
### Lenguaje y Plataforma
El proyecto est� desarrollado en C# utilizando .NET 8.0, lo que permite aprovechar las �ltimas caracter�sticas del lenguaje y la plataforma.
### Base de Datos
Se utiliza SQLite como base de datos para almacenar los documentos y sus res�menes. SQLite es una base de datos ligera y f�cil de usar, ideal para aplicaciones de consola y proyectos peque�os.
### Estructura del Proyecto
El proyecto est� estructurado en varias carpetas para organizar el c�digo de manera clara:
- **KineticTechnicalChallenge**: Contiene la soluci�n principal del proyecto.
- **KineticTechnicalChallenge.Core**: Contiene la l�gica de negocio y las entidades del dominio.
- **KineticTechnicalChallenge.Data**: Contiene la implementaci�n de acceso a datos y la configuraci�n de la base de datos.
- **KineticTechnicalChallenge.ConsoleApp**: Contiene la aplicaci�n de consola que interact�a con el usuario.
- **KineticTechnicalChallenge.Tests**: Contiene las pruebas unitarias para asegurar la calidad del c�digo.

## Instalaci�n y Ejecuci�n
### Requisitos Previos
- .NET 8.0 SDK instalado en tu m�quina.
- SQLite instalado o la capacidad de utilizar SQLite a trav�s de NuGet.
- Un editor de c�digo como Visual Studio o Visual Studio Code.
- Conexi�n a Internet para descargar dependencias y herramientas necesarias.
- Conocimientos b�sicos de C# y .NET.
- Conocimientos b�sicos de bases de datos y SQL.

### Pasos para la Instalaci�n
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
-Abrir la soluci�n en tu editor de c�digo.
-Aseg�rarse de que la cadena de conexi�n a la base de datos sea correcta. Se puede cambiar el nombre del archivo de la base de datos de ser necesario.
-Donde `InputFolder` es la carpeta donde se encuentran los archivos de texto a procesar y `BatchSize` es el n�mero de archivos que se procesar�n en cada lote.

### Ejecuci�n del Proyecto
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

- Ejecuta el siguiente comando para iniciar la aplicaci�n de consola:
- 
```bash
dotnet run --project KineticTechnicalChallenge.ConsoleApp
```

- La aplicaci�n solicitar� la ruta de la carpeta que contiene los archivos de texto a procesar. Aseg�rate de que los archivos est�n en el formato correcto (texto plano).
- La aplicaci�n procesar� los archivos en lotes, generando res�menes y almacen�ndolos en la base de datos SQLite.
- Puedes consultar los res�menes generados en la base de datos utilizando herramientas como DBeaber o cualquier cliente SQLite.
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
