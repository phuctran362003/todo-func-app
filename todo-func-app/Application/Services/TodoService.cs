using Application.Interfaces;
using Domain.DTOs.TodoDTOs;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Application.Services
{
    public class TodoService : ITodoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TodoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TodoItemDto> CreateAsync(CreateTodoItemDto createDto)
        {
            var todoItem = new TodoItem
            {
                id = Guid.NewGuid().ToString(),
                Title = createDto.Title,
                Description = createDto.Description,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TodoItems.AddAsync(todoItem);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(todoItem);
        }

        public async Task<TodoItemDto> GetByIdAsync(string id)
        {
            var todoItem = await _unitOfWork.TodoItems.GetByIdAsync(id);

            if (todoItem == null)
            {
                throw new KeyNotFoundException($"TodoItem with id {id} not found");
            }

            return MapToDto(todoItem);
        }

        public async Task<IEnumerable<TodoItemDto>> GetAllAsync()
        {
            var todoItems = await _unitOfWork.TodoItems.GetAllAsync();
            return todoItems.Select(MapToDto).ToList();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var todoItem = await _unitOfWork.TodoItems.GetByIdAsync(id);

            if (todoItem == null)
            {
                return false;
            }

            await _unitOfWork.TodoItems.HardRemoveAsync(todoItem);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<TodoItemDto> UpdateAsync(string id, UpdateTodoItemDto updateDto)
        {
            var todoItem = await _unitOfWork.TodoItems.GetByIdAsync(id);

            if (todoItem == null)
            {
                throw new KeyNotFoundException($"TodoItem with id {id} not found");
            }

            todoItem.Title = updateDto.Title;
            todoItem.Description = updateDto.Description;
            todoItem.IsCompleted = updateDto.IsCompleted;
            todoItem.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.TodoItems.Update(todoItem);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(todoItem);
        }

        private TodoItemDto MapToDto(TodoItem todoItem)
        {
            return new TodoItemDto
            {
                Id = todoItem.id,
                Title = todoItem.Title,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted,
                CreatedAt = todoItem.CreatedAt,
                UpdatedAt = todoItem.UpdatedAt,
                CreatedBy = todoItem.CreatedBy,
                UpdatedBy = todoItem.UpdatedBy
            };
        }
    }
}
