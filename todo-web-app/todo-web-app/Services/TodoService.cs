using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using todo_web_app.Models;

namespace todo_web_app.Services
{
    public class TodoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TodoService> _logger;
        private string ApiBaseUrl => $"{_configuration["ApiSettings:TodoFunctionAppBaseUrl"]}/api";

        public TodoService(HttpClient httpClient, IConfiguration configuration, ILogger<TodoService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _logger.LogInformation($"TodoService initialized. API Base URL: {ApiBaseUrl}");
        }

        /// <summary>
        /// Lấy tất cả todos
        /// </summary>
        public async Task<List<TodoItem>> GetAllTodosAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all todos...");
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<TodoItem>>>(
                    $"{ApiBaseUrl}/todos");
                
                var result = response?.Value?.Data ?? new List<TodoItem>();
                _logger.LogInformation($"Successfully fetched {result.Count} todos");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching todos: {ex.Message}, Stack: {ex.StackTrace}");
                return new List<TodoItem>();
            }
        }

        /// <summary>
        /// Lấy 1 todo theo ID
        /// </summary>
        public async Task<TodoItem?> GetTodoByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Fetching todo ID: {id}");
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<TodoItem>>(
                    $"{ApiBaseUrl}/todos/{id}");
                
                var result = response?.Value?.Data;
                _logger.LogInformation($"Successfully fetched todo ID: {id}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching todo: {ex.Message}, Stack: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Tạo todo mới
        /// </summary>
        public async Task<TodoItem?> CreateTodoAsync(CreateTodoItemDto createDto)
        {
            try
            {
                _logger.LogInformation($"Creating todo: Title='{createDto.Title}'");
                var response = await _httpClient.PostAsJsonAsync(
                    $"{ApiBaseUrl}/todos", createDto);
                
                _logger.LogInformation($"Create response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Create response content: {jsonContent}");
                    var result = JsonSerializer.Deserialize<ApiResponse<TodoItem>>(jsonContent);
                    _logger.LogInformation($"Todo created successfully with ID: {result?.Value?.Data?.Id}");
                    return result?.Value?.Data;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Create failed: {response.StatusCode} - {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating todo: {ex.Message}, Stack: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Cập nhật todo
        /// </summary>
        public async Task<TodoItem?> UpdateTodoAsync(string id, UpdateTodoItemDto updateDto)
        {
            try
            {
                _logger.LogInformation($"Updating todo ID: {id}, Title: '{updateDto.Title}', IsCompleted: {updateDto.IsCompleted}");
                var response = await _httpClient.PutAsJsonAsync(
                    $"{ApiBaseUrl}/todos/{id}", updateDto);
                
                _logger.LogInformation($"Update response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<TodoItem>>(jsonContent);
                    _logger.LogInformation($"Todo updated successfully");
                    return result?.Value?.Data;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Update failed: {response.StatusCode} - {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating todo: {ex.Message}, Stack: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Xóa todo
        /// </summary>
        public async Task<bool> DeleteTodoAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Deleting todo ID: {id}");
                var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/todos/{id}");
                _logger.LogInformation($"Delete response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Todo deleted successfully");
                    return true;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Delete failed: {response.StatusCode} - {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting todo: {ex.Message}, Stack: {ex.StackTrace}");
                return false;
            }
        }
    }
}
