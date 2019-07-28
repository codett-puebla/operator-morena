using operator_morena.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace operator_morena.Connection
{
    public class ConnectionDB : DbContext
    {
        public ConnectionDB(): base(Properties.Settings.Default.Connection){}

        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Operator> Operators { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
    }
}
