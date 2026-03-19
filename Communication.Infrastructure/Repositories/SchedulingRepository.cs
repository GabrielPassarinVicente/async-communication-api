using Communication.Domain.Entities;
using Communication.Domain.Interfaces;
using Communication.Infrastructure.Data;

namespace Communication.Infrastructure.Repositories;

public class SchedulingRepository : ISchedulingRepository
{
    private readonly CommunicationDbContext _dbContext;

    // Aqui está a Injeção de Dependência a acontecer no construtor!
    public SchedulingRepository(CommunicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddAsync(Scheduling scheduling)
    {
        await _dbContext.Schedulings.AddAsync(scheduling);
        await _dbContext.SaveChangesAsync(); // Salva as mudanças no banco de dados
    }
    public async Task<Scheduling?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Schedulings.FindAsync(id);
    }
    public async Task UpdateAsync(Scheduling scheduling)
    {
        _dbContext.Schedulings.Update(scheduling);
        await _dbContext.SaveChangesAsync(); // Salva as mudanças no banco de dados
    }
}

