namespace EchoRoom.Database.Infrastructure.BaseRepository;

public interface IBaseRepository<T> where T : class
{
    protected internal EchoRoomContext Context { get; }

    public async Task Add(T item)
        => await Context.Set<T>().AddAsync(item);

    public async Task<T?> GetItemWIthIncludes(
        Expression<Func<T, bool>>? predicateExpression = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = Context.Set<T>();


        if (includes is not null && includes.Length is not 0)
            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);

        if (predicateExpression is not null)
            query = query.Where(predicateExpression);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ICollection<T>> GetItems(
        Expression<Func<T, bool>>? predicateExpression = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = Context.Set<T>();

        return predicateExpression is not null
            ? await query.Where(predicateExpression).ToListAsync(cancellationToken)
            : await query.ToListAsync(cancellationToken);
    }

    public async Task<TResult?> GetMappedItem<TResult>(
        Expression<Func<T, TResult>> selectExpression,
        Expression<Func<T, bool>>? predicateExpression = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = Context.Set<T>();

        return predicateExpression is not null
            ? await query.Where(predicateExpression).Select(selectExpression).FirstOrDefaultAsync(cancellationToken)
            : await query.Select(selectExpression).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ICollection<TResult>> GetMappedItems<TResult>(
        Expression<Func<T, TResult>> selectExpression,
        Expression<Func<T, bool>>? predicateExpression = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = Context.Set<T>();

        return predicateExpression is not null
            ? await query.Where(predicateExpression).Select(selectExpression).ToListAsync(cancellationToken)
            : await query.Select(selectExpression).ToListAsync(cancellationToken);
    }

    public async Task<bool> Any(Expression<Func<T, bool>> predicateExpression, CancellationToken cancellationToken = default)
        => await Context.Set<T>().AnyAsync(predicateExpression, cancellationToken);
}