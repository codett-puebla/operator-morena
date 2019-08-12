using System.Data.Entity;
using operator_morena.Models;
using operator_morena.Properties;

namespace operator_morena.Connection
{
    public class ConnectionDB : DbContext
    {
        public ConnectionDB() : base(Settings.Default.Connection)
        {
        }
        //public ConnectionDB() : base(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="+ AppDomain.CurrentDomain.BaseDirectory + @"DataBase\dbMorena.mdf;Integrated Security=True;Connect Timeout=30") { }

        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Operator> Operators { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
    }
}