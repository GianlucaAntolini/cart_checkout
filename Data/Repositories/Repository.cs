using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public abstract class Repository<T> : ControllerBase, IRepository<T> where T : class, IEntity
{
    protected readonly DbContext _context;
    protected DbSet<T> dbSet;
    private readonly IUnitOfwork _unitOfWork;

    public Repository(IUnitOfwork unitOfwork)
    {
        _unitOfWork = unitOfwork;
        dbSet = _unitOfWork.Context.Set<T>();
    }

    //Get Request
    public async Task<ActionResult<IEnumerable<T>>> Get()
    {
        var data = await dbSet.ToListAsync();
        return Ok(data);
    }

    //Get by Id Request
    public async Task<ActionResult<T>> GetById(int id)
    {
        var data = await dbSet.FindAsync(id);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(data);
    }

    public async Task<ActionResult<int>> GetFirstAvailableId()
    {
        var data = await dbSet.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        if (data == null)
        {
            return Ok(1);
        }

        return Ok(data.Id + 1);
    }


    //Create Request
    public async Task<ActionResult<T>> Create(T entity)
    {
        dbSet.Add(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity;
    }

    //Update Request
    public async Task<IActionResult> Update(int id, T entity)
    {
        if (id != entity.Id)
        {
            return BadRequest();
        }




        var existingEntity = await dbSet.FindAsync(id);
        if (existingEntity == null)
        {
            return NotFound();
        }

        _unitOfWork.Context.Entry(existingEntity).CurrentValues.SetValues(entity);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }

        return NoContent();
    }

    //Delete Request
    public async Task<IActionResult> Delete(int id)
    {
        var data = await dbSet.FindAsync(id);
        if (data == null)
        {
            return NotFound();
        }

        dbSet.Remove(data);
        await _unitOfWork.SaveChangesAsync();
        return NoContent();
    }
}