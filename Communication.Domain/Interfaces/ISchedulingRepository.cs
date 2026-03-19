using Communication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Domain.Interfaces;

public interface ISchedulingRepository
{
    // Dá suporte ao seu "POST Agendar" (Salva no banco)
    Task AddAsync(Scheduling scheduling);

    // Dá suporte ao seu "GET Consultar" (Busca no banco por um ID)
    Task<Scheduling?> GetByIdAsync(Guid id);

    // Dá suporte ao seu "Cancelar" (Atualiza o registro no banco)
    Task UpdateAsync(Scheduling scheduling);
}

