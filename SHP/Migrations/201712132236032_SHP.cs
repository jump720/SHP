namespace SHP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SHPM : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.User",
            //    c => new
            //        {
            //            UserId = c.String(nullable: false, maxLength: 5),
            //            UserName = c.String(nullable: false, maxLength: 100),
            //            UserMail = c.String(nullable: false, maxLength: 100),
            //            UserCountry = c.String(nullable: false, maxLength: 20),
            //            UserCity = c.String(nullable: false, maxLength: 20),
            //            UserTitle = c.String(nullable: false, maxLength: 50),
            //            UserOffice = c.String(nullable: false, maxLength: 4),
            //        })
            //    .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
           // DropTable("dbo.User");
        }
    }
}
