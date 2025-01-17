﻿using CryptoExchange.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoExchange.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Coin> Coins { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ExchangeCoin> ExchangeCoins { get; set; }
        public DbSet<Setting> Settings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasKey(x => new
                {
                    x.UserId,
                    x.CoinId
                });
            modelBuilder.Entity<Account>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Account>()
                .HasOne(x => x.Coin)
                .WithMany()
                .HasForeignKey(x => x.CoinId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Transaction>()
                .HasOne<Coin>()
                .WithMany()
                .HasForeignKey(x => x.CoinId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExchangeCoin>()
                .HasKey(x => x.CoinId);

            modelBuilder.Entity<Setting>()
                .HasKey(x => x.Key);
            modelBuilder.Entity<Setting>()
                .Property(x => x.Value)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
