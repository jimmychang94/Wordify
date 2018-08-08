using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wordify.Models;

namespace Wordify.Data
{
    public class DataDbContext : DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {

        }

        public DbSet<Note> Notes { get; set; }
        //public DbSet<Group> Groups { get; set; }
        //public DbSet<GroupMember> GroupMembers { get; set; }
        //public DbSet<GroupNote> GroupNotes { get; set; }
    }
}
