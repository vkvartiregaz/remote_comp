using ComputationServer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Job> Operations { get; set; }
    }
}
