using Microsoft.EntityFrameworkCore;

namespace MazeServiceScraper.Infrastructure.Database
{
	public class MazeDbContext : DbContext
	{
		public DbSet<Show> Shows { get; set; }
		public DbSet<Cast> Casts { get; set; }

		public MazeDbContext(DbContextOptions<MazeDbContext> options) : base(options)
		{

		}
	}
}