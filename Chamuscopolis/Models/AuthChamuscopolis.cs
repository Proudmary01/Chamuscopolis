namespace Chamuscopolis.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AuthChamuscopolis : DbContext
    {
        public AuthChamuscopolis()
            : base("name=AuthChamuscopolis")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
