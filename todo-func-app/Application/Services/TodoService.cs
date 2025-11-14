using Application.Interfaces;
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

        // create todoitem
        // get todoitem
        // delete todoitem

    }
}
