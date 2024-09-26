using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ServerMarketBot.Entities;

namespace ServerMarketBot;

public class DatabaseContext : DbContext
{
	public DbSet<User> Users { get; set; }
	public DbSet<Application> Applications { get; set; }
	public DatabaseContext(DbContextOptions<DatabaseContext> opt) : base(opt)
	{
		//Database.EnsureDeleted();
		Database.EnsureCreated();
	}
}
