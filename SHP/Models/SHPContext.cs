using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Web.Security;
using System.Configuration;

namespace SHP.Models
{
    public class SHPContext : DbContext
    {

        //private object po;
#if DEBUG
        public SHPContext() : base("SHP")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

#else
        public MMSContext() : base(Crypto.AESGCM.SimpleDecryptWithPassword(ConfigurationManager.ConnectionStrings["MMS"].ConnectionString, "4p3xT00lsGroup"))
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }
#endif

        #region Security

        public DbSet<User> User { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Answer> Answer { get; set; }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region Config Keys

            /*Primary Key User*/
            modelBuilder.Entity<User>().HasKey(u => new { u.UserId });

            /*Primary Key Question*/
            modelBuilder.Entity<Question>().HasKey(q => new { q.QuestionId });

            /*Primary Key Answer*/
            modelBuilder.Entity<Answer>().HasKey(a => new { a.QuestionId, a.UserId });

            /*Relation 1 to * User and Answer*/
            modelBuilder.Entity<Answer>().HasRequired<User>(a => a.User)
                .WithMany(u => u.AnswerList)
                .HasForeignKey(a => a.UserId).WillCascadeOnDelete(false);

            /*Relation 1 to * Question and Answer*/
            modelBuilder.Entity<Answer>().HasRequired<Question>(a => a.Question)
                .WithMany(q => q.AnswerList)
                .HasForeignKey(a => a.QuestionId).WillCascadeOnDelete(false);

            #endregion
        }
    }
}