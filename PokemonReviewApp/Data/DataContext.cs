using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Pokemon> Pokemon { get; set; }
        public DbSet<PokemonCategory> PokemonCategories { get; set; }
        public DbSet<PokemonOwner> PokemonOwners { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //many to many relationship'i entity framework daha deqiq basa dussun ki,
            //bu verilenler sayesinde asagidakilari islet

            modelBuilder.Entity<PokemonCategory>()
                        .HasKey(pc => new { pc.PokemonId, pc.CategoryId });
            //diqqetnen baxsaq one to many relationship gozukur, yeni butun butun pokemon'lar PokemonCategories'e aiddi
            //ve bunlar PokemonId'e nezeren save olunur
            modelBuilder.Entity<PokemonCategory>()
                        .HasOne(p => p.Pokemon)
                        .WithMany(pc => pc.PokemonCategories)   //burda pokemonCategory PokemonCategories'e deyisib yeni basa dusduyun kimidi
                        .HasForeignKey(p => p.PokemonId);
            modelBuilder.Entity<PokemonCategory>()
                        .HasOne(p => p.Category)
                        .WithMany(pc => pc.PokemonCategories)
                        .HasForeignKey(c => c.CategoryId);

            modelBuilder.Entity<PokemonOwner>()
                        .HasKey(po => new { po.PokemonId, po.OwnerId });
            modelBuilder.Entity<PokemonOwner>()
                        .HasOne(p => p.Pokemon)
                        .WithMany(po => po.PokemonOwners)
                        .HasForeignKey(p => p.PokemonId);
            modelBuilder.Entity<PokemonOwner>()
                        .HasOne(o => o.Owner)
                        .WithMany(po => po.PokemonOwners)
                        .HasForeignKey(oi => oi.OwnerId);

        }
    }
}
