using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class HistorialCustodioRepository : GenericRepository<HistorialCustodio>, IHistorialCustodioRepository
{
    public HistorialCustodioRepository(ApplicationDbContext context) : base(context)
    {
    }
}
