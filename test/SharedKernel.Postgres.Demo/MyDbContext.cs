using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Postgres.Demo;

public class MyDbContext : DbContext
{
   protected MyDbContext(DbContextOptions options) : base(options)
   {
   }
}