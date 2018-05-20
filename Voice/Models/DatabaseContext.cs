﻿using System.Data.Entity;

namespace Voice.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() :
            base("DiplomaConnection")
        { }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<WavFile> WavFiles { get; set; }
    }
}