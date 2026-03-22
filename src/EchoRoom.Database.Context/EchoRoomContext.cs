namespace EchoRoom.Database.Context;

public sealed class EchoRoomContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Balance> Balances { get; set; }

    public EchoRoomContext()
    {
        
    }

    public EchoRoomContext(DbContextOptions<EchoRoomContext> options) 
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EchoRoomContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}