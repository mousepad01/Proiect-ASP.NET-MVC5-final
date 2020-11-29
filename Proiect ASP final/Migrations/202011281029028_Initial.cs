namespace Proiect_ASP_final.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Adrese",
                c => new
                    {
                        idAdresa = c.Int(nullable: false, identity: true),
                        tara = c.String(nullable: false, maxLength: 25),
                        oras = c.String(nullable: false, maxLength: 100),
                        strada = c.String(nullable: false, maxLength: 100),
                        nr = c.Int(nullable: false),
                        idUtilizator = c.Int(nullable: false),
                        mentiune = c.String(),
                    })
                .PrimaryKey(t => t.idAdresa);
            
            CreateTable(
                "dbo.Categorii",
                c => new
                    {
                        idCategorie = c.Int(nullable: false, identity: true),
                        titlu = c.String(nullable: false, maxLength: 100),
                        descriere = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.idCategorie);
            
            CreateTable(
                "dbo.CategoriiProduse",
                c => new
                    {
                        idProdusCategorie = c.Int(nullable: false, identity: true),
                        idProdus = c.Int(nullable: false),
                        idCategorie = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idProdusCategorie)
                .ForeignKey("dbo.Categorii", t => t.idCategorie, cascadeDelete: true)
                .ForeignKey("dbo.Produse", t => t.idProdus, cascadeDelete: true)
                .Index(t => t.idProdus)
                .Index(t => t.idCategorie);
            
            CreateTable(
                "dbo.Produse",
                c => new
                    {
                        idProdus = c.Int(nullable: false, identity: true),
                        idOwner = c.Int(nullable: false),
                        titlu = c.String(nullable: false, maxLength: 100),
                        descriere = c.String(nullable: false),
                        imagePath = c.String(),
                        pret = c.Int(nullable: false),
                        dataAdaugare = c.DateTime(nullable: false),
                        cantitate = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idProdus);
            
            CreateTable(
                "dbo.ProduseRatinguri",
                c => new
                    {
                        prodRating = c.Int(nullable: false, identity: true),
                        idProdus = c.Int(nullable: false),
                        idUtilizator = c.Int(nullable: false),
                        dataReview = c.DateTime(nullable: false),
                        rating = c.Int(nullable: false),
                        descriere = c.String(maxLength: 1024),
                    })
                .PrimaryKey(t => t.prodRating)
                .ForeignKey("dbo.Produse", t => t.idProdus, cascadeDelete: true)
                .Index(t => t.idProdus);
            
            CreateTable(
                "dbo.Comenzi",
                c => new
                    {
                        idComanda = c.Int(nullable: false, identity: true),
                        dataPlasare = c.DateTime(nullable: false),
                        idAdresa = c.Int(nullable: false),
                        idUtilizator = c.Int(nullable: false),
                        dataFinalizare = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.idComanda)
                .ForeignKey("dbo.Adrese", t => t.idAdresa, cascadeDelete: true)
                .Index(t => t.idAdresa);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comenzi", "idAdresa", "dbo.Adrese");
            DropForeignKey("dbo.ProduseRatinguri", "idProdus", "dbo.Produse");
            DropForeignKey("dbo.CategoriiProduse", "idProdus", "dbo.Produse");
            DropForeignKey("dbo.CategoriiProduse", "idCategorie", "dbo.Categorii");
            DropIndex("dbo.Comenzi", new[] { "idAdresa" });
            DropIndex("dbo.ProduseRatinguri", new[] { "idProdus" });
            DropIndex("dbo.CategoriiProduse", new[] { "idCategorie" });
            DropIndex("dbo.CategoriiProduse", new[] { "idProdus" });
            DropTable("dbo.Comenzi");
            DropTable("dbo.ProduseRatinguri");
            DropTable("dbo.Produse");
            DropTable("dbo.CategoriiProduse");
            DropTable("dbo.Categorii");
            DropTable("dbo.Adrese");
        }
    }
}
