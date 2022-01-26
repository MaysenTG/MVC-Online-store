namespace Lab_Example.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FullProducts : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductImageMappings", "ImageNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductImageMappings", "ImageNumber", c => c.String());
        }
    }
}
