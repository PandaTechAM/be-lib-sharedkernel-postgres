using GridifyExtensions.DbContextFunction;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Postgres.Demo;

public class MyDbContext : PostgresFunctions
{
   protected MyDbContext(DbContextOptions options) : base(options)
   {
   }
}