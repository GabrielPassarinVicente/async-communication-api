using Communication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Infrastructure.Data;

public class CommunicationDbContext : DbContext
{
    // O construtor recebe as opções de conexão (como a string de conexão do MySQL)
    public CommunicationDbContext(DbContextOptions<CommunicationDbContext> options) : base(options)
    {
    }

    public DbSet<Scheduling> Schedulings { get; set; } // Representa a tabela de agendamentos no banco de dados
}

