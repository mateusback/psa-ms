using Microsoft.EntityFrameworkCore;
using psa_legacy_sis.Domain;
using psa_legacy_sis.Domain.Repositories;

namespace psa_legacy_sis.Database.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;

    public CustomerRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _db.Set<Customer>().AddAsync(customer, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public async Task<Customer?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.Set<Customer>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _db.Set<Customer>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return list;
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _db.Set<Customer>().Update(customer);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var stub = new Customer { Id = id };
        _db.Entry(stub).State = EntityState.Deleted;
        await _db.SaveChangesAsync(cancellationToken);
    }
}