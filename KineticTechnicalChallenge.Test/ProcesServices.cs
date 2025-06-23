using KineticTechnicalChallenge.Core.Contract.Configuration;
using KineticTechnicalChallenge.Core.Contract.Enums;
using KineticTechnicalChallenge.Core.Contract.Interfaces;
using KineticTechnicalChallenge.Core.Data;
using KineticTechnicalChallenge.Core.Data.Models;
using KineticTechnicalChallenge.Services;
using KineticTechnicalChallenge.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Text.Json;

namespace KineticTechnicalChallenge.Tests.Services
{
    [TestFixture]
    public class ProcessServiceTests
    {
        private Mock<ILogger<ProcessService>> _mockLogger;
        private Mock<IOptions<TextProcessingSettings>> _mockSettings;
        private Mock<IAnalysisServices> _mockAnalysisServices;
        private Mock<IProcessStorage> _mockStorage;
        private Mock<IProcessQueue> _mockQueue;
        private ProcessService _processService;
        private DocumentContext _context;
        private TextProcessingSettings _settings;

        [SetUp]
        public void Setup()
        {
            // Crear contexto en memoria
            var options = new DbContextOptionsBuilder<DocumentContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DocumentContext(options);

            // Setup mocks
            _mockLogger = new Mock<ILogger<ProcessService>>();
            _mockAnalysisServices = new Mock<IAnalysisServices>();
            _mockStorage = new Mock<IProcessStorage>();
            _mockQueue = new Mock<IProcessQueue>();

            // Setup settings
            _settings = new TextProcessingSettings
            {
                BatchSize = 5,
                InputFolder = Path.GetTempPath()
            };
            _mockSettings = new Mock<IOptions<TextProcessingSettings>>();
            _mockSettings.Setup(x => x.Value).Returns(_settings);

            // Crear archivos temporales para testing
            HelpersTest.CreateTempTestFiles(_settings);

            _processService = new ProcessService(
                _context,
                _mockLogger.Object,
                _mockSettings.Object,
                _mockAnalysisServices.Object,
                _mockStorage.Object,
                _mockQueue.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
            HelpersTest.CleanupTempTestFiles(_settings);
        }

        #region GetProcessResultsAsync Tests

        [Test]
        public async Task GetProcessResultsAsync_ValidGuid_ReturnsProcessResponse()
        {
            // Arrange
            var processId = Guid.NewGuid();
            var result = new AnalysisResult
            {
                Id = Guid.NewGuid(),
                ProcessInfoId = processId,
                TotalWords = 100,
                TotalLines = 10,
                TotalCharacters = 500,
                MostFrequentWordsJson = JsonSerializer.Serialize(new List<string> { "test", "word" }),
                FilesProcessedJson = JsonSerializer.Serialize(new List<string> { "file1.txt" })
            };

            await _context.Results.AddAsync(result);
            await _context.SaveChangesAsync();

            // Act
            var response = await _processService.GetProcessResultsAsync(processId.ToString());

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Results.TotalWords, Is.EqualTo(100));
            Assert.That(response.Results.TotalLines, Is.EqualTo(10));
            Assert.That(response.Results.TotalCharacters, Is.EqualTo(500));
            Assert.That(response.Results.MostFrequentWords, Contains.Item("test"));
            Assert.That(response.Results.FilesProcessed, Contains.Item("file1.txt"));
        }

        [Test]
        public void GetProcessResultsAsync_InvalidGuid_ThrowsArgumentException()
        {
            // Arrange
            var invalidGuid = "invalid-guid";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
                _processService.GetProcessResultsAsync(invalidGuid));
            Assert.That(ex.Message, Does.Contain("Invalid GUID format"));
            Assert.That(ex.ParamName, Is.EqualTo("processGuid"));
        }

        [Test]
        public void GetProcessResultsAsync_ProcessNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var nonExistentGuid = Guid.NewGuid().ToString();

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
                _processService.GetProcessResultsAsync(nonExistentGuid));
            Assert.That(ex.Message, Does.Contain("Process Result not found"));
        }

        [Test]
        public void GetProcessResultsAsync_NullGuid_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() =>
                _processService.GetProcessResultsAsync(null));
        }

        [Test]
        public void GetProcessResultsAsync_EmptyGuid_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() =>
                _processService.GetProcessResultsAsync(string.Empty));
        }

        #endregion

        #region GetProcessStatusAsync Tests

        [Test]
        public async Task GetProcessStatusAsync_ValidGuid_ReturnsProcessStatus()
        {
            // Arrange
            var processId = Guid.NewGuid();
            var process = new ProcessInfo
            {
                Id = processId,
                Status = ProcessStatus.Running,
                StartedAt = DateTime.UtcNow,
                TotalFiles = 5,
                ProcessedFiles = 2,
                FilesJson = JsonSerializer.Serialize("test1,test2,test3")
            };

            await _context.Processes.AddAsync(process);
            await _context.SaveChangesAsync();

            // Act
            var status = await _processService.GetProcessStatusAsync(processId.ToString());

            // Assert
            Assert.That(status, Is.EqualTo(ProcessStatus.Running));
        }

        [Test]
        public void GetProcessStatusAsync_InvalidGuid_ThrowsArgumentException()
        {
            // Arrange
            var invalidGuid = "invalid-guid";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
                _processService.GetProcessStatusAsync(invalidGuid));
            Assert.That(ex.Message, Does.Contain("Invalid GUID format"));
        }

        [Test]
        public void GetProcessStatusAsync_ProcessNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var nonExistentGuid = Guid.NewGuid().ToString();

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
                _processService.GetProcessStatusAsync(nonExistentGuid));
            Assert.That(ex.Message, Does.Contain("Process not found"));
        }

        #endregion

        #region ListProcessesAsync Tests

        [Test]
        public async Task ListProcessesAsync_WithProcesses_ReturnsProcessList()
        {
            // Arrange
            var processId = Guid.NewGuid();
            var result = new AnalysisResult
            {
                Id = Guid.NewGuid(),
                ProcessInfoId = processId,
                TotalWords = 50,
                TotalLines = 5,
                TotalCharacters = 250,
                MostFrequentWordsJson = JsonSerializer.Serialize(new List<string> { "hello" }),
                FilesProcessedJson = JsonSerializer.Serialize(new List<string> { "test.txt" })
            };

            var process = new ProcessInfo
            {
                Id = processId,
                Status = ProcessStatus.Completed,
                StartedAt = DateTime.UtcNow,
                EstimatedCompletion = DateTime.UtcNow.AddMinutes(5),
                TotalFiles = 1,
                ProcessedFiles = 1,
                Results = result,
                FilesJson = JsonSerializer.Serialize("test1,test2,test3")

            };

            await _context.Processes.AddAsync(process);
            await _context.Results.AddAsync(result);
            await _context.SaveChangesAsync();

            // Act
            var processes = await _processService.ListProcessesAsync();

            // Assert
            Assert.That(processes, Is.Not.Null);
            Assert.That(processes.Count, Is.EqualTo(1));
            Assert.That(processes[0].ProcessInfoDTO.Guid, Is.EqualTo(processId));
            Assert.That(processes[0].ProcessInfoDTO.Status, Is.EqualTo(ProcessStatus.Completed));
            Assert.That(processes[0].Results.TotalWords, Is.EqualTo(50));
        }

        [Test]
        public async Task ListProcessesAsync_NoProcesses_ReturnsEmptyList()
        {
            // Act & Assert
            var processes = await _processService.ListProcessesAsync();
            Assert.That(processes, Is.Not.Null);
            Assert.That(processes.Count, Is.EqualTo(0));
        }

        #endregion

        #region StartProcessAsync Tests

        [Test]
        public async Task StartProcessAsync_WithFiles_CreatesProcesses()
        {
            // Act
            var processes = await _processService.StartProcessAsync();

            // Assert
            Assert.That(processes, Is.Not.Null);
            Assert.That(processes.Count, Is.GreaterThan(0));

            foreach (var process in processes)
            {
                Assert.That(process.ProcessInfoDTO.Guid, Is.Not.EqualTo(Guid.Empty));
                Assert.That(process.ProcessInfoDTO.Status, Is.EqualTo(ProcessStatus.Pending));
                Assert.That(process.ProcessInfoDTO.StartedAt, Is.LessThanOrEqualTo(DateTime.UtcNow));
            }

            // Verificar que se encolaron los procesos
            _mockQueue.Verify(q => q.Enqueue(It.IsAny<Guid>()), Times.AtLeast(1));
        }

        [Test]
        public async Task StartProcessAsync_MoreThan5Files_CreatesBatches()
        {
            // Arrange - Crear más archivos temporales
            HelpersTest.CreateAdditionalTempFiles(_settings, 7); // Total 14 archivos

            // Act
            var processes = await _processService.StartProcessAsync();

            // Assert
            Assert.That(processes.Count, Is.EqualTo(3)); // 2 batches: 5 + 5

            foreach (var process in processes)
            {
                Assert.That(process.ProcessInfoDTO.ProgressInfo.TotalFiles, Is.LessThanOrEqualTo(5));
            }
        }

        #endregion

        #region StopProcessAsync Tests

        [Test]
        public async Task StopProcessAsync_RunningProcess_StopsProcess()
        {
            // Arrange
            var processId = Guid.NewGuid();
            var result = new AnalysisResult
            {
                Id = Guid.NewGuid(),
                ProcessInfoId = processId,
                TotalWords = 0,
                TotalLines = 0,
                TotalCharacters = 0,
                MostFrequentWordsJson = JsonSerializer.Serialize(new List<string>()),
                FilesProcessedJson = JsonSerializer.Serialize(new List<string>())
            };

            var process = new ProcessInfo
            {
                Id = processId,
                Status = ProcessStatus.Running,
                StartedAt = DateTime.UtcNow,
                TotalFiles = 1,
                ProcessedFiles = 0,
                Results = result,
                FilesJson = JsonSerializer.Serialize("test1,test2,test3")
            };

            await _context.Processes.AddAsync(process);
            await _context.Results.AddAsync(result);
            await _context.SaveChangesAsync();

            // Act
            var response = await _processService.StopProcessAsync(processId.ToString());

            // Assert
            Assert.That(response.ProcessInfoDTO.Status, Is.EqualTo(ProcessStatus.Stopped));

            var updatedProcess = await _context.Processes.FindAsync(processId);
            Assert.That(updatedProcess.Status, Is.EqualTo(ProcessStatus.Stopped));
        }

        [Test]
        public void StopProcessAsync_InvalidGuid_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() =>
                _processService.StopProcessAsync("invalid-guid"));
        }

        [Test]
        public void StopProcessAsync_ProcessNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var nonExistentGuid = Guid.NewGuid().ToString();

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
                _processService.StopProcessAsync(nonExistentGuid));
            Assert.That(ex.Message, Does.Contain("Process not found or not in running state"));
        }

        [Test]
        public async Task StopProcessAsync_CompletedProcess_ThrowsInvalidOperationException()
        {
            // Arrange
            var processId = Guid.NewGuid();
            var process = new ProcessInfo
            {
                Id = processId,
                Status = ProcessStatus.Completed,
                StartedAt = DateTime.UtcNow,
                TotalFiles = 1,
                ProcessedFiles = 1,
                FilesJson = JsonSerializer.Serialize("test1,test2,test3")
            };

            await _context.Processes.AddAsync(process);
            await _context.SaveChangesAsync();

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
                _processService.StopProcessAsync(processId.ToString()));
            Assert.That(ex.Message, Does.Contain("Process not found or not in running state"));
        }

        #endregion

        #region ContinueProcessAsync Tests

        [Test]
        public async Task ContinueProcessAsync_StoppedProcess_ContinuesProcess()
        {
            // Arrange
            var processId = Guid.NewGuid();
            var result = new AnalysisResult
            {
                Id = Guid.NewGuid(),
                ProcessInfoId = processId,
                TotalWords = 25,
                TotalLines = 3,
                TotalCharacters = 125,
                MostFrequentWordsJson = JsonSerializer.Serialize(new List<string> { "partial" }),
                FilesProcessedJson = JsonSerializer.Serialize(new List<string> { "file1.txt" })
            };

            var process = new ProcessInfo
            {
                Id = processId,
                Status = ProcessStatus.Stopped,
                StartedAt = DateTime.UtcNow,
                TotalFiles = 2,
                ProcessedFiles = 1,
                Results = result,
                FilesJson = JsonSerializer.Serialize("test1,test2,test3")
            };

            await _context.Processes.AddAsync(process);
            await _context.Results.AddAsync(result);
            await _context.SaveChangesAsync();

            // Act
            var response = await _processService.ContinueProcessAsync(processId.ToString());

            // Assert
            Assert.That(response.ProcessInfoDTO.Status, Is.EqualTo(ProcessStatus.Pending));

            var updatedProcess = await _context.Processes.FindAsync(processId);
            Assert.That(updatedProcess.Status, Is.EqualTo(ProcessStatus.Pending));

            _mockQueue.Verify(q => q.Enqueue(processId), Times.Once);
        }

        [Test]
        public void ContinueProcessAsync_InvalidGuid_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() =>
                _processService.ContinueProcessAsync("invalid-guid"));
        }

        [Test]
        public void ContinueProcessAsync_RunningProcess_ThrowsInvalidOperationException()
        {
            // Arrange
            var processId = Guid.NewGuid();
            var result = new AnalysisResult
            {
                Id = Guid.NewGuid(),
                ProcessInfoId = processId,
                TotalWords = 25,
                TotalLines = 3,
                TotalCharacters = 125,
                MostFrequentWordsJson = JsonSerializer.Serialize(new List<string> { "partial" }),
                FilesProcessedJson = JsonSerializer.Serialize(new List<string> { "file1.txt" })
            };

            var process = new ProcessInfo
            {
                Id = processId,
                Status = ProcessStatus.Running,
                StartedAt = DateTime.UtcNow,
                TotalFiles = 2,
                ProcessedFiles = 1,
                Results = result,
                FilesJson = JsonSerializer.Serialize("test1,test2,test3")
            };

            _context.Processes.Add(process);
            _context.SaveChanges();

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
                _processService.ContinueProcessAsync(processId.ToString()));
            Assert.That(ex.Message, Does.Contain("Process not found or not in running state"));
        }

        #endregion

        #region Edge Cases and Error Handling Tests

        [Test]
        public void Constructor_NullContext_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ProcessService(
                null, _mockLogger.Object, _mockSettings.Object,
                _mockAnalysisServices.Object, _mockStorage.Object, _mockQueue.Object));
        }

        [Test]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ProcessService(
                _context, null, _mockSettings.Object,
                _mockAnalysisServices.Object, _mockStorage.Object, _mockQueue.Object));
        }

        [Test]
        public void Constructor_NullSettings_ThrowsArgumentNullException()
        {
            // Arrange
            var mockNullSettings = new Mock<IOptions<TextProcessingSettings>>();
            mockNullSettings.Setup(x => x.Value).Returns((TextProcessingSettings)null);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ProcessService(
                _context, _mockLogger.Object, mockNullSettings.Object,
                _mockAnalysisServices.Object, _mockStorage.Object, _mockQueue.Object));
        }

        [Test]
        public async Task GetProcessResultsAsync_CorruptedJson_HandlesGracefully()
        {
            // Arrange
            var processId = Guid.NewGuid();
            var result = new AnalysisResult
            {
                Id = Guid.NewGuid(),
                ProcessInfoId = processId,
                TotalWords = 100,
                TotalLines = 10,
                TotalCharacters = 500,
                MostFrequentWordsJson = "invalid-json",
                FilesProcessedJson = "invalid-json"
            };

            await _context.Results.AddAsync(result);
            await _context.SaveChangesAsync();

            // Act
            var response = await _processService.GetProcessResultsAsync(processId.ToString());

            // Assert
            Assert.That(response.Results.MostFrequentWords, Is.Not.Null);
            Assert.That(response.Results.FilesProcessed, Is.Not.Null);
            Assert.That(response.Results.MostFrequentWords.Count, Is.EqualTo(0));
            Assert.That(response.Results.FilesProcessed.Count, Is.EqualTo(0));
        }
    }

    #endregion
}