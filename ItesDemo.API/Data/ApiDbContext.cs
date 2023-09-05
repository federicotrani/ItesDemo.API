using ItesDemo.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ItesDemo.API.Data;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

}
