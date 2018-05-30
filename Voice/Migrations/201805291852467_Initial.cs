namespace Voice.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Surname = c.String(nullable: false),
                        Age = c.Int(nullable: false),
                        Sex = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Visits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTime = c.DateTime(nullable: false),
                        PatientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.PatientId);
            
            CreateTable(
                "dbo.WavFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Path = c.String(),
                        Jitter = c.Double(nullable: false),
                        Shimmer = c.Double(nullable: false),
                        HNR = c.Double(nullable: false),
                        Intensity = c.Double(nullable: false),
                        FirstPitch = c.Double(nullable: false),
                        PitchMassive = c.Double(nullable: false),
                        VisitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Visits", t => t.VisitId, cascadeDelete: true)
                .Index(t => t.VisitId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Role = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Visits", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.WavFiles", "VisitId", "dbo.Visits");
            DropIndex("dbo.WavFiles", new[] { "VisitId" });
            DropIndex("dbo.Visits", new[] { "PatientId" });
            DropTable("dbo.Users");
            DropTable("dbo.WavFiles");
            DropTable("dbo.Visits");
            DropTable("dbo.Patients");
        }
    }
}
