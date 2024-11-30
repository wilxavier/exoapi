using Exo.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace Exo.WebApi.Contexts
{
    public class ExoContext : DbContext
    {
        public ExoContext()
        {
        }
        public ExoContext(DbContextOptions<ExoContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //essa string de conexao depente da sua maquina.
                optionsBuilder.UseSqlServer("Server=HP\\SQLEXPRESS;" + "Database=ExoApi;Trusted_Connection=True;");
            }
        }
        public DbSet<Projeto> Projetos { get; set; }// será criado um model projeto
        public DbSet<Usuario> Usuarios { get; set; }
    }
}