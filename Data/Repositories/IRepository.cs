using Microsoft.AspNetCore.Mvc;

   public interface IRepository<T> where T : class
   {
        // Get all entities
        public Task<ActionResult<IEnumerable<T>>> Get();

        // Get an entity by id
        public Task<ActionResult<T>> GetById(int id);

        // Create a new entity        
        public Task<ActionResult<T>> Create(T entity);

        // Update an entity
        public Task<IActionResult> Update(int id, T entity);

        // Delete an entity
        public Task<IActionResult> Delete(int id);
    }