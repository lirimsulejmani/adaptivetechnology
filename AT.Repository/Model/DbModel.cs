namespace AT.Repository.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Collections.Generic;

    public partial class DbModel : DbContext
    {
        public DbModel()
            : base("name=DbEntities")
        {
        }

        public virtual DbSet<AdaptationTechnique> AdaptationTechniques { get; set; }
        public virtual DbSet<TextToReadHyperlink> TextToReadHyperlinks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<DbModel>(null);
        }
    }
}
