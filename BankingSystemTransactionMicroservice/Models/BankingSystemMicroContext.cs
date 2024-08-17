using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemTransactionMicroservice.Models;

public partial class BankingSystemMicroContext : DbContext
{
    public BankingSystemMicroContext()
    {
    }

    public BankingSystemMicroContext(DbContextOptions<BankingSystemMicroContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Recurrenttransaction> Recurrenttransactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=bankingsystemtransactionmicroservice;Username=postgres;Password=mypass03923367");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Recurrenttransaction>(entity =>
        {
            entity.HasKey(e => e.Recurrenttransactionidmicro).HasName("recurrenttransactions_pkey");

            entity.ToTable("recurrenttransactions");

            entity.Property(e => e.Recurrenttransactionidmicro).HasColumnName("recurrenttransactionidmicro");
            entity.Property(e => e.Accountid).HasColumnName("accountid");
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Branchid)
                .HasMaxLength(50)
                .HasColumnName("branchid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Frequency)
                .HasMaxLength(50)
                .HasColumnName("frequency");
            entity.Property(e => e.Nexttransactiondate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("nexttransactiondate");
            entity.Property(e => e.Recurrenttransactionid).HasColumnName("recurrenttransactionid");
            entity.Property(e => e.Transactiontype)
                .HasMaxLength(50)
                .HasColumnName("transactiontype");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
