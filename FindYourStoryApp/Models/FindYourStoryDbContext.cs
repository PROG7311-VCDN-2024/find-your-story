using Microsoft.EntityFrameworkCore;

namespace FindYourStoryApp.Models;

public partial class FindYourStoryDbContext : DbContext
{
    public FindYourStoryDbContext()
    {
    }

    public FindYourStoryDbContext(DbContextOptions<FindYourStoryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ShippingAddress> ShippingAddresses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(configurationRoot.GetConnectionString("Find_Your_Story_DB"));
        }

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD797B7977DE5");

            entity.ToTable("Cart");

            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cart__ProductID__72C60C4A");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cart__UserID__71D1E811");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF471F5313");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ShippingAddressId).HasColumnName("ShippingAddressID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerID");

            entity.HasOne(d => d.ShippingAddress).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ShippingAddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ShippingAddressID");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6CDA295A051");

            entity.ToTable("Product");

            entity.Property(e => e.Author).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AB8540479");

            entity.HasIndex(e => e.RoleType, "UQ__Roles__5D0A2E94E3542F2D").IsUnique();

            entity.Property(e => e.RoleType).HasMaxLength(50);
        });

        modelBuilder.Entity<ShippingAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Shipping__091C2A1BB1ACE06E");

            entity.ToTable("ShippingAddress");

            entity.Property(e => e.AddressId).HasColumnName("AddressID");
            entity.Property(e => e.AddressLine1).HasMaxLength(255);
            entity.Property(e => e.AddressLine2).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.State).HasMaxLength(100);

            entity.HasOne(d => d.Customer).WithMany(p => p.ShippingAddresses)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerID_ShippingAddresses");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACF46A0BF4");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4EADFED6F").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirebaseUid)
                .HasMaxLength(100)
                .HasColumnName("FirebaseUID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
