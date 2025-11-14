using Domain.DTOs.TodoDTOs;

namespace Application.Interfaces
{
    public interface ITodoService
    {
        Task<TodoItemDto> CreateAsync(CreateTodoItemDto createDto);
        Task<TodoItemDto> GetByIdAsync(string id);
        Task<IEnumerable<TodoItemDto>> GetAllAsync();
        Task<TodoItemDto> UpdateAsync(string id, UpdateTodoItemDto updateDto);
        Task<bool> DeleteAsync(string id);
    }
}
