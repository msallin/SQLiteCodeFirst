namespace SQLite.CodeFirst.MigrationConsole.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Int(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "Autoincrement",
                                    new AnnotationValues(oldValue: null, newValue: "True")
                                },
                            }),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Coaches", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.Name, name: "IX_Team_TeamsName");
            
            CreateTable(
                "dbo.Coaches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(maxLength: 50,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "Collate",
                                    new AnnotationValues(oldValue: null, newValue: "NoCase")
                                },
                            }),
                        LastName = c.String(maxLength: 50),
                        Street = c.String(maxLength: 100),
                        City = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "DATETIME('now')")
                                },
                            }),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TeamPlayer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        TeamId = c.Int(nullable: false),
                        FirstName = c.String(maxLength: 50,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "Collate",
                                    new AnnotationValues(oldValue: null, newValue: "NoCase")
                                },
                            }),
                        LastName = c.String(maxLength: 50),
                        Street = c.String(maxLength: 100),
                        City = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "DATETIME('now')")
                                },
                            }),
                        Mentor_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TeamPlayer", t => t.Mentor_Id)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.Number)
                .Index(t => new { t.Number, t.TeamId }, unique: true, name: "IX_TeamPlayer_NumberPerTeam")
                .Index(t => t.Mentor_Id);
            
            CreateTable(
                "dbo.Stadions",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Street = c.String(nullable: false, maxLength: 128),
                        City = c.String(nullable: false, maxLength: 128),
                        Order = c.Int(nullable: false),
                        Team_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Name, t.Street, t.City })
                .ForeignKey("dbo.Teams", t => t.Team_Id, cascadeDelete: true)
                .Index(t => new { t.Street, t.Name }, unique: true, name: "IX_Stadion_Main")
                .Index(t => t.Order, unique: true, name: "ReservedKeyWordTest")
                .Index(t => t.Team_Id);
            
            CreateTable(
                "dbo.Foos",
                c => new
                    {
                        FooId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        FooSelf1Id = c.Int(),
                        FooSelf2Id = c.Int(),
                        FooSelf3Id = c.Int(),
                    })
                .PrimaryKey(t => t.FooId)
                .ForeignKey("dbo.Foos", t => t.FooSelf1Id)
                .ForeignKey("dbo.Foos", t => t.FooSelf2Id)
                .ForeignKey("dbo.Foos", t => t.FooSelf3Id)
                .Index(t => t.FooSelf1Id)
                .Index(t => t.FooSelf2Id)
                .Index(t => t.FooSelf3Id);
            
            CreateTable(
                "dbo.FooSelves",
                c => new
                    {
                        FooSelfId = c.Int(nullable: false, identity: true),
                        FooId = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FooSelfId)
                .ForeignKey("dbo.Foos", t => t.FooId, cascadeDelete: true)
                .Index(t => t.FooId);
            
            CreateTable(
                "dbo.FooSteps",
                c => new
                    {
                        FooStepId = c.Int(nullable: false, identity: true),
                        FooId = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FooStepId)
                .ForeignKey("dbo.Foos", t => t.FooId, cascadeDelete: true)
                .Index(t => t.FooId);
            
            CreateTable(
                "dbo.FooCompositeKeys",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Version = c.String(nullable: false, maxLength: 20),
                        Name = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => new { t.Id, t.Version });
            
            CreateTable(
                "dbo.FooRelationshipAs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FooRelationshipBs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FooRelationshipAFooCompositeKeys",
                c => new
                    {
                        FooRelationshipA_Id = c.Int(nullable: false),
                        FooCompositeKey_Id = c.Int(nullable: false),
                        FooCompositeKey_Version = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => new { t.FooRelationshipA_Id, t.FooCompositeKey_Id, t.FooCompositeKey_Version })
                .ForeignKey("dbo.FooRelationshipAs", t => t.FooRelationshipA_Id, cascadeDelete: true)
                .ForeignKey("dbo.FooCompositeKeys", t => new { t.FooCompositeKey_Id, t.FooCompositeKey_Version }, cascadeDelete: true)
                .Index(t => t.FooRelationshipA_Id)
                .Index(t => new { t.FooCompositeKey_Id, t.FooCompositeKey_Version });
            
            CreateTable(
                "dbo.FooRelationshipBFooCompositeKeys",
                c => new
                    {
                        FooRelationshipB_Id = c.Int(nullable: false),
                        FooCompositeKey_Id = c.Int(nullable: false),
                        FooCompositeKey_Version = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => new { t.FooRelationshipB_Id, t.FooCompositeKey_Id, t.FooCompositeKey_Version })
                .ForeignKey("dbo.FooRelationshipBs", t => t.FooRelationshipB_Id, cascadeDelete: true)
                .ForeignKey("dbo.FooCompositeKeys", t => new { t.FooCompositeKey_Id, t.FooCompositeKey_Version }, cascadeDelete: true)
                .Index(t => t.FooRelationshipB_Id)
                .Index(t => new { t.FooCompositeKey_Id, t.FooCompositeKey_Version });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FooRelationshipBFooCompositeKeys", new[] { "FooCompositeKey_Id", "FooCompositeKey_Version" }, "dbo.FooCompositeKeys");
            DropForeignKey("dbo.FooRelationshipBFooCompositeKeys", "FooRelationshipB_Id", "dbo.FooRelationshipBs");
            DropForeignKey("dbo.FooRelationshipAFooCompositeKeys", new[] { "FooCompositeKey_Id", "FooCompositeKey_Version" }, "dbo.FooCompositeKeys");
            DropForeignKey("dbo.FooRelationshipAFooCompositeKeys", "FooRelationshipA_Id", "dbo.FooRelationshipAs");
            DropForeignKey("dbo.Foos", "FooSelf3Id", "dbo.Foos");
            DropForeignKey("dbo.Foos", "FooSelf2Id", "dbo.Foos");
            DropForeignKey("dbo.Foos", "FooSelf1Id", "dbo.Foos");
            DropForeignKey("dbo.FooSteps", "FooId", "dbo.Foos");
            DropForeignKey("dbo.FooSelves", "FooId", "dbo.Foos");
            DropForeignKey("dbo.Stadions", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.TeamPlayer", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.TeamPlayer", "Mentor_Id", "dbo.TeamPlayer");
            DropForeignKey("dbo.Teams", "Id", "dbo.Coaches");
            DropIndex("dbo.FooRelationshipBFooCompositeKeys", new[] { "FooCompositeKey_Id", "FooCompositeKey_Version" });
            DropIndex("dbo.FooRelationshipBFooCompositeKeys", new[] { "FooRelationshipB_Id" });
            DropIndex("dbo.FooRelationshipAFooCompositeKeys", new[] { "FooCompositeKey_Id", "FooCompositeKey_Version" });
            DropIndex("dbo.FooRelationshipAFooCompositeKeys", new[] { "FooRelationshipA_Id" });
            DropIndex("dbo.FooSteps", new[] { "FooId" });
            DropIndex("dbo.FooSelves", new[] { "FooId" });
            DropIndex("dbo.Foos", new[] { "FooSelf3Id" });
            DropIndex("dbo.Foos", new[] { "FooSelf2Id" });
            DropIndex("dbo.Foos", new[] { "FooSelf1Id" });
            DropIndex("dbo.Stadions", new[] { "Team_Id" });
            DropIndex("dbo.Stadions", "ReservedKeyWordTest");
            DropIndex("dbo.Stadions", "IX_Stadion_Main");
            DropIndex("dbo.TeamPlayer", new[] { "Mentor_Id" });
            DropIndex("dbo.TeamPlayer", "IX_TeamPlayer_NumberPerTeam");
            DropIndex("dbo.TeamPlayer", new[] { "Number" });
            DropIndex("dbo.Teams", "IX_Team_TeamsName");
            DropIndex("dbo.Teams", new[] { "Id" });
            DropTable("dbo.FooRelationshipBFooCompositeKeys");
            DropTable("dbo.FooRelationshipAFooCompositeKeys");
            DropTable("dbo.FooRelationshipBs");
            DropTable("dbo.FooRelationshipAs");
            DropTable("dbo.FooCompositeKeys");
            DropTable("dbo.FooSteps");
            DropTable("dbo.FooSelves");
            DropTable("dbo.Foos");
            DropTable("dbo.Stadions");
            DropTable("dbo.TeamPlayer",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "CreatedUtc",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "DATETIME('now')" },
                        }
                    },
                    {
                        "FirstName",
                        new Dictionary<string, object>
                        {
                            { "Collate", "NoCase" },
                        }
                    },
                });
            DropTable("dbo.Coaches",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "CreatedUtc",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "DATETIME('now')" },
                        }
                    },
                    {
                        "FirstName",
                        new Dictionary<string, object>
                        {
                            { "Collate", "NoCase" },
                        }
                    },
                });
            DropTable("dbo.Teams",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "Id",
                        new Dictionary<string, object>
                        {
                            { "Autoincrement", "True" },
                        }
                    },
                });
        }
    }
}
