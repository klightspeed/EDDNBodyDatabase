using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Diagnostics;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;
using EDDNBodyDatabase.XModels;
using System.Configuration;

namespace EDDNBodyDatabase.Models
{
    public class BodyDbContext : DbContext
    {
        public BodyDbContext() : base(ConfigurationManager.AppSettings["BodyDBConnection"] ?? "BodyDB.Mysql")
        {
        }

        static BodyDbContext()
        {
            Database.SetInitializer<BodyDbContext>(new MigrateDatabaseToLatestVersion<BodyDbContext, Migrations.Configuration>());
            //Database.SetInitializer<BodyDbContext>(new DropCreateDatabaseAlways<BodyDbContext>());
        }

        protected void Configure<T>(DbModelBuilder builder, Action<EntityTypeConfiguration<T>> action)
            where T : class
        {
            action(builder.Entity<T>());
        }

        protected void ConfigureNameIdMap<T>(DbModelBuilder builder)
            where T : class, INameIdMapTinyId
        {
            Configure<T>(builder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                m.Property(e => e.Name).IsRequired().HasMaxLength(128).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute { IsUnique = true }));
            });
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<ForeignKeyIndexConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            ConfigureNameIdMap<Atmosphere>(modelBuilder);
            ConfigureNameIdMap<AtmosphereComponent>(modelBuilder);
            ConfigureNameIdMap<AtmosphereType>(modelBuilder);
            ConfigureNameIdMap<BodyType>(modelBuilder);
            ConfigureNameIdMap<Luminosity>(modelBuilder);
            ConfigureNameIdMap<MaterialName>(modelBuilder);
            ConfigureNameIdMap<PlanetClass>(modelBuilder);
            ConfigureNameIdMap<ReserveLevel>(modelBuilder);
            ConfigureNameIdMap<RingClass>(modelBuilder);
            ConfigureNameIdMap<ScanType>(modelBuilder);
            ConfigureNameIdMap<Software>(modelBuilder);
            ConfigureNameIdMap<StarType>(modelBuilder);
            ConfigureNameIdMap<TerraformState>(modelBuilder);
            ConfigureNameIdMap<Volcanism>(modelBuilder);

            Configure<Region>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                m.Property(e => e.Name).IsRequired().HasMaxLength(128).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute { IsUnique = true }));
                m.Property(e => e.RegionAddress).IsOptional();
            });

            Configure<SoftwareVersion>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                m.HasRequired(e => e.SoftwareRef).WithMany().HasForeignKey(e => e.SoftwareId).WillCascadeOnDelete(false);
                m.Property(e => e.SoftwareId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Version", 1)));
                m.Property(e => e.Version).IsRequired().HasMaxLength(128).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Version", 2)));
                m.Ignore(e => e.SoftwareName);
            });

            Configure<System>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                m.Property(e => e.ModSystemAddress).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute()));
                m.Property(e => e.RegionId).IsRequired();
                m.Property(e => e.SizeClass).IsRequired();
                m.Property(e => e.Mid3).IsRequired();
                m.Property(e => e.Mid2).IsRequired();
                m.Property(e => e.Mid1b).IsRequired();
                m.Property(e => e.Mid1a).IsRequired();
                m.Property(e => e.Sequence).IsRequired();
                m.Property(e => e.EdsmId).IsRequired();
                m.Ignore(e => e.SystemAddress);
                m.Ignore(e => e.Region);
                m.Ignore(e => e.Name);
                m.Ignore(e => e.EdsmLastModified);
                m.HasOptional(e => e.SystemCustomName).WithRequired().WillCascadeOnDelete(false);
                m.HasRequired(e => e.RegionRef).WithMany().HasForeignKey(e => e.RegionId).WillCascadeOnDelete(false);
            });

            Configure<SystemCustomName>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.CustomName).IsRequired().HasMaxLength(128).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute()));
                m.Property(e => e.SystemAddress).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute()));
            });

            Configure<SystemBody>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                m.Property(e => e.SystemId).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("BodyPgName", 1) { IsUnique = true }));
                m.Property(e => e.Stars).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("BodyPgName", 2) { IsUnique = true }));
                m.Property(e => e.IsBelt).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("BodyPgName", 3) { IsUnique = true }));
                m.Property(e => e.Planet).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("BodyPgName", 4) { IsUnique = true }));
                m.Property(e => e.Moon1).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("BodyPgName", 5) { IsUnique = true }));
                m.Property(e => e.Moon2).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("BodyPgName", 6) { IsUnique = true }));
                m.Property(e => e.Moon3).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("BodyPgName", 7) { IsUnique = true }));
                m.Property(e => e.ScanBaseHash).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("BodyPgName", 8) { IsUnique = true }));
                m.Property(e => e.BodyID).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("BodyPgName", 9) { IsUnique = true }));
                m.Property(e => e.CustomNameId).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("BodyPgName", 10) { IsUnique = true }));
                m.HasOptional(e => e.CustomName).WithRequired().WillCascadeOnDelete(false);
                m.HasRequired(e => e.SystemRef).WithMany().HasForeignKey(e => e.SystemId).WillCascadeOnDelete(false);
            });

            Configure<BodyCustomName>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                m.Property(e => e.Name).IsRequired().HasMaxLength(128).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute { IsUnique = true }));
            });

            Configure<SystemBodyCustomName>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.SystemId).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                {
                    new IndexAttribute("CustomName", 1)
                }));
                m.Property(e => e.CustomName).IsRequired().HasMaxLength(128).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                {
                    new IndexAttribute(),
                    new IndexAttribute("CustomName", 2)
                }));
            });

            Configure<SystemBodyDuplicate>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.Name).IsRequired().HasMaxLength(255).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                {
                    new IndexAttribute("NameHash", 1)
                }));
                m.Property(e => e.ScanBaseHash).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                {
                    new IndexAttribute("NameHash", 2)
                }));
                m.HasRequired(e => e.Body).WithMany().HasForeignKey(e => e.DuplicateOfBodyId).WillCascadeOnDelete(false);
                m.HasRequired(e => e.Duplicate).WithOptional(t => t.DuplicateOf).WillCascadeOnDelete(false);
            });

            Configure<ParentSet>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                m.Ignore(e => e.Parent0Type);
                m.Ignore(e => e.Parent1Type);
                m.Ignore(e => e.Parent2Type);
                m.Ignore(e => e.Parent3Type);
                m.Ignore(e => e.Parent4Type);
                m.Ignore(e => e.Parent5Type);
                m.Ignore(e => e.Parent6Type);
                m.Ignore(e => e.Parent7Type);
                m.HasRequired(e => e.Parent0TypeRef).WithMany().HasForeignKey(e => e.Parent0TypeId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.Parent1TypeRef).WithMany().HasForeignKey(e => e.Parent1TypeId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.Parent2TypeRef).WithMany().HasForeignKey(e => e.Parent2TypeId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.Parent3TypeRef).WithMany().HasForeignKey(e => e.Parent3TypeId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.Parent4TypeRef).WithMany().HasForeignKey(e => e.Parent4TypeId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.Parent5TypeRef).WithMany().HasForeignKey(e => e.Parent5TypeId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.Parent6TypeRef).WithMany().HasForeignKey(e => e.Parent6TypeId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.Parent7TypeRef).WithMany().HasForeignKey(e => e.Parent7TypeId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.ParentRef).WithMany().HasForeignKey(e => e.ParentId).WillCascadeOnDelete(false);
                m.Property(e => e.Parent1BodyId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_ParentBody", 1)));
                m.Property(e => e.Parent2BodyId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_ParentBody", 2)));
                m.Property(e => e.Parent3BodyId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_ParentBody", 3)));
                m.Property(e => e.Parent4BodyId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_ParentBody", 4)));
                m.Property(e => e.Parent5BodyId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_ParentBody", 5)));
                m.Property(e => e.Parent6BodyId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_ParentBody", 6)));
                m.Property(e => e.Parent7BodyId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_ParentBody", 7)));
            });

            Configure<BodyScan>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                m.Property(e => e.SystemBodyId).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute()));
                m.HasRequired(e => e.SystemBody).WithMany().HasForeignKey(e => e.SystemBodyId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.Parents).WithMany().HasForeignKey(e => e.ParentSetId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.ReserveLevelRef).WithMany().HasForeignKey(e => e.ReserveLevelId).WillCascadeOnDelete(false);
                m.Ignore(e => e.ReserveLevel);
            });

            Configure<BodyScanStar>(modelBuilder, m =>
            {
                m.Ignore(e => e.Luminosity);
                m.Ignore(e => e.StarType);
                m.HasOptional(e => e.LuminosityRef).WithMany().HasForeignKey(e => e.LuminosityId).WillCascadeOnDelete(false);
                m.HasRequired(e => e.StarTypeRef).WithMany().HasForeignKey(e => e.StarTypeId).WillCascadeOnDelete(false);
                m.ToTable("BodyScanStars");
            });

            Configure<BodyScanPlanet>(modelBuilder, m =>
            {
                m.Ignore(e => e.PlanetClass);
                m.Ignore(e => e.Volcanism);
                m.HasOptional(e => e.Atmosphere).WithRequired();
                m.HasOptional(e => e.Materials).WithRequired();
                m.HasRequired(e => e.PlanetClassRef).WithMany().HasForeignKey(e => e.PlanetClassId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.VolcanismRef).WithMany().HasForeignKey(e => e.VolcanismId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.TerraformStateRef).WithMany().HasForeignKey(e => e.TerraformStateId).WillCascadeOnDelete(false);
                m.Ignore(e => e.TerraformState);
                m.ToTable("BodyScanPlanets");
            });

            Configure<BodyScanMaterials>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                m.Ignore(e => e.Material1);
                m.Ignore(e => e.Material2);
                m.Ignore(e => e.Material3);
                m.Ignore(e => e.Material4);
                m.Ignore(e => e.Material5);
                m.Ignore(e => e.Material6);
                m.HasRequired(e => e.Material1Ref).WithMany().HasForeignKey(e => e.Material1Id).WillCascadeOnDelete(false);
                m.HasRequired(e => e.Material2Ref).WithMany().HasForeignKey(e => e.Material2Id).WillCascadeOnDelete(false);
                m.HasRequired(e => e.Material3Ref).WithMany().HasForeignKey(e => e.Material3Id).WillCascadeOnDelete(false);
                m.HasRequired(e => e.Material4Ref).WithMany().HasForeignKey(e => e.Material4Id).WillCascadeOnDelete(false);
                m.HasRequired(e => e.Material5Ref).WithMany().HasForeignKey(e => e.Material5Id).WillCascadeOnDelete(false);
                m.HasRequired(e => e.Material6Ref).WithMany().HasForeignKey(e => e.Material6Id).WillCascadeOnDelete(false);
            });

            Configure<BodyScanAtmosphere>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                m.Ignore(e => e.Atmosphere);
                m.Ignore(e => e.AtmosphereComponent1);
                m.Ignore(e => e.AtmosphereComponent2);
                m.Ignore(e => e.AtmosphereComponent3);
                m.Ignore(e => e.AtmosphereType);
                m.HasOptional(e => e.AtmosphereComponent1Ref).WithMany().HasForeignKey(e => e.AtmosphereComponent1Id).WillCascadeOnDelete(false);
                m.HasOptional(e => e.AtmosphereComponent2Ref).WithMany().HasForeignKey(e => e.AtmosphereComponent2Id).WillCascadeOnDelete(false);
                m.HasOptional(e => e.AtmosphereComponent3Ref).WithMany().HasForeignKey(e => e.AtmosphereComponent3Id).WillCascadeOnDelete(false);
                m.HasRequired(e => e.AtmosphereRef).WithMany().HasForeignKey(e => e.AtmosphereId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.AtmosphereTypeRef).WithMany().HasForeignKey(e => e.AtmosphereTypeId).WillCascadeOnDelete(false);
            });

            Configure<BodyScanRing>(modelBuilder, m =>
            {
                m.HasKey(e => new { e.ScanId, e.RingNum });
                m.Ignore(e => e.Class);
                m.HasRequired(e => e.ClassRef).WithMany().HasForeignKey(e => e.ClassId).WillCascadeOnDelete(false);
                m.HasRequired(e => e.ScanRef).WithMany(t => t.Rings).HasForeignKey(e => e.ScanId).WillCascadeOnDelete(false);
                m.HasOptional(e => e.CustomName).WithRequired().WillCascadeOnDelete(false);
                m.Ignore(e => e.Name);
            });

            Configure<BodyScanRingCustomName>(modelBuilder, m =>
            {
                m.HasKey(e => new { e.ScanId, e.RingNum });
                m.Property(e => e.Name).IsRequired().HasMaxLength(255);
            });

            Configure<EDDNJournalScan>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                m.HasRequired(e => e.SoftwareVersion).WithMany().HasForeignKey(e => e.SoftwareVersionId).WillCascadeOnDelete(false);
                m.HasRequired(e => e.ScanData).WithMany().HasForeignKey(e => e.BodyScanId).WillCascadeOnDelete(false);
                m.Property(e => e.BodyScanId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute()));
                m.HasOptional(e => e.ScanTypeRef).WithMany().HasForeignKey(e => e.ScanTypeId).WillCascadeOnDelete(false);
                m.Ignore(e => e.GatewayTimestamp);
                m.Ignore(e => e.ScanTimestamp);
                m.Ignore(e => e.ScanType);
                m.Ignore(e => e.JsonExtra);
                m.HasOptional(e => e.JsonExtraRef).WithRequired().WillCascadeOnDelete(false);
            });

            Configure<EDDNJournalScanJsonExtra>(modelBuilder, m =>
            {
                m.HasKey(e => e.Id);
                m.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                m.Property(e => e.JsonExtra).IsRequired().HasMaxLength(1024);
            });
        }

        public void Init()
        {
            this.Database.Initialize(false);
        }

        public static void InitializeDatabase()
        {
            using (BodyDbContext ctx = new BodyDbContext())
            {
                ctx.Init();
            }
        }

        public int MsSqlInsertIdentity(DbCommand cmd)
        {
            return (int)cmd.ExecuteScalar();
        }

        public int MySqlInsertIdentity(DbCommand cmd)
        {
            cmd.ExecuteNonQuery();
            return (int)((MySql.Data.MySqlClient.MySqlCommand)cmd).LastInsertedId;
        }

        public Func<DbCommand, int> GetInsertIdentity(ref string cmdtext, ref string cmdtext2)
        {
            if (Database.Connection is global::System.Data.SqlClient.SqlConnection)
            {
                cmdtext += " OUTPUT INSERTED.Id";
                return MsSqlInsertIdentity;
            }
            else if (Database.Connection is global::MySql.Data.MySqlClient.MySqlConnection)
            {
                return MySqlInsertIdentity;
            }
            else
            {
                return null;
            }
        }

        public bool BoolValue(object val)
        {
            return val is sbyte ? ((sbyte)val != 0) : (bool)val;
        }

        public bool? BoolNullValue(object val)
        {
            return val is sbyte ? ((sbyte)val != 0) : val as bool?;
        }

        public List<System> GetSystemsByAddress(long sysaddr)
        {
            List<System> systems = new List<System>();

            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT s.Id, s.ModSystemAddress, s.X, s.Y, s.Z, s.RegionId, s.SizeClass, s.Mid3, s.Mid2, s.Mid1b, s.Mid1a, s.Sequence, n.SystemAddress, n.CustomName, s.EdsmId, s.EdsmLastModifiedSeconds " +
                    "FROM Systems s " +
                    "LEFT JOIN SystemCustomNames n ON n.Id = s.Id " +
                    "WHERE s.ModSystemAddress = @ModSystemAddress";
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction;
                var addrparam = cmd.AddParameter("@ModSystemAddress", DbType.Int64, System.SystemAddressToModSystemAddress(sysaddr));
                object[] cols = new object[16];

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        rdr.GetValues(cols);
                        System sys = new System
                        {
                            Id = (int)cols[0],
                            ModSystemAddress = (long)cols[1],
                            X = (int)cols[2],
                            Y = (int)cols[3],
                            Z = (int)cols[4],
                            RegionId = (short)cols[5],
                            SizeClass = (byte)cols[6],
                            Mid3 = (byte)cols[7],
                            Mid2 = (byte)cols[8],
                            Mid1b = (byte)cols[9],
                            Mid1a = (byte)cols[10],
                            Sequence = (short)cols[11],
                            EdsmId = (int)cols[14],
                            EdsmLastModifiedSeconds = (int)cols[15],
                        };

                        if (cols[13] != DBNull.Value)
                        {
                            sys.SystemCustomName = new SystemCustomName
                            {
                                Id = sys.Id,
                                SystemAddress = (long)cols[12],
                                CustomName = (string)cols[13]
                            };
                        }

                        systems.Add(sys);
                    }
                }
            }

            systems.Reverse();

            return systems;
        }

        public List<System> GetSystemsByCustomName(string name)
        {
            List<System> systems = new List<System>();

            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT s.Id, s.ModSystemAddress, s.X, s.Y, s.Z, s.RegionId, s.SizeClass, s.Mid3, s.Mid2, s.Mid1b, s.Mid1a, s.Sequence, n.SystemAddress, n.CustomName, s.EdsmId, s.EdsmLastModifiedSeconds " +
                    "FROM Systems s " +
                    "JOIN SystemCustomNames n ON n.Id = s.Id " +
                    "WHERE n.CustomName = @SystemName";
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction;
                cmd.AddParameter("@SystemName", DbType.String, name);
                object[] cols = new object[16];

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        rdr.GetValues(cols);
                        System sys = new System
                        {
                            Id = (int)cols[0],
                            ModSystemAddress = (long)cols[1],
                            X = (int)cols[2],
                            Y = (int)cols[3],
                            Z = (int)cols[4],
                            RegionId = (short)cols[5],
                            SizeClass = (byte)cols[6],
                            Mid3 = (byte)cols[7],
                            Mid2 = (byte)cols[8],
                            Mid1b = (byte)cols[9],
                            Mid1a = (byte)cols[10],
                            Sequence = (short)cols[11],
                            EdsmId = (int)cols[14],
                            EdsmLastModifiedSeconds = (int)cols[15],
                        };

                        sys.SystemCustomName = new SystemCustomName
                        {
                            Id = sys.Id,
                            SystemAddress = (long)cols[12],
                            CustomName = (string)cols[13]
                        };

                        systems.Add(sys);
                    }
                }
            }

            systems.Reverse();

            return systems;
        }

        public List<System> GetSystemsByEdsmId(int edsmid)
        {
            List<System> systems = new List<System>();

            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT s.Id, s.ModSystemAddress, s.X, s.Y, s.Z, s.RegionId, s.SizeClass, s.Mid3, s.Mid2, s.Mid1b, s.Mid1a, s.Sequence, n.SystemAddress, n.CustomName, s.EdsmId, s.EdsmLastModifiedSeconds " +
                    "FROM Systems s " +
                    "JOIN SystemCustomNames n ON n.Id = s.Id " +
                    "WHERE s.EdsmId = @EdsmId";
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction;
                cmd.AddParameter("@EdsmId", DbType.Int32, edsmid);
                object[] cols = new object[16];

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        rdr.GetValues(cols);
                        System sys = new System
                        {
                            Id = (int)cols[0],
                            ModSystemAddress = (long)cols[1],
                            X = (int)cols[2],
                            Y = (int)cols[3],
                            Z = (int)cols[4],
                            RegionId = (short)cols[5],
                            SizeClass = (byte)cols[6],
                            Mid3 = (byte)cols[7],
                            Mid2 = (byte)cols[8],
                            Mid1b = (byte)cols[9],
                            Mid1a = (byte)cols[10],
                            Sequence = (short)cols[11],
                            EdsmId = (int)cols[14],
                            EdsmLastModifiedSeconds = (int)cols[15],
                        };

                        if (cols[13] != DBNull.Value)
                        {
                            sys.SystemCustomName = new SystemCustomName
                            {
                                Id = sys.Id,
                                SystemAddress = (long)cols[12],
                                CustomName = (string)cols[13]
                            };
                        }

                        systems.Add(sys);
                    }
                }
            }

            systems.Reverse();

            return systems;
        }

        public List<System> FindSystemsByCoordinates(int x, int y, int z, int range = 0)
        {
            List<System> systems = new List<System>();

            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT s.Id, s.ModSystemAddress, s.X, s.Y, s.Z, s.RegionId, s.SizeClass, s.Mid3, s.Mid2, s.Mid1b, s.Mid1a, s.Sequence, n.SystemAddress, n.CustomName, s.EdsmId, s.EdsmLastModifiedSeconds " +
                    "FROM Systems s " +
                    "LEFT JOIN SystemCustomNames n ON n.Id = s.Id " +
                    "WHERE s.ModSystemAddress >= @ModSystemAddress AND s.ModSystemAddress < @ModSystemAddress + 65536";
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction;
                var sysaddrparam = cmd.AddParameter("@ModSystemAddress", DbType.Int64);

                object[] cols = new object[16];
                for (byte sizeclass = 0; sizeclass < 8; sizeclass++)
                {
                    long modsysaddr = System.ModSystemAddressFromCoords(x, y, z, sizeclass, 0);
                    sysaddrparam.Value = modsysaddr;

                    using (DbDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            rdr.GetValues(cols);
                            System sys = new System
                            {
                                Id = (int)cols[0],
                                ModSystemAddress = (long)cols[1],
                                X = (int)cols[2],
                                Y = (int)cols[3],
                                Z = (int)cols[4],
                                RegionId = (short)cols[5],
                                SizeClass = (byte)cols[6],
                                Mid3 = (byte)cols[7],
                                Mid2 = (byte)cols[8],
                                Mid1b = (byte)cols[9],
                                Mid1a = (byte)cols[10],
                                Sequence = (short)cols[11],
                                EdsmId = (int)cols[14],
                                EdsmLastModifiedSeconds = (int)cols[15],
                            };

                            if (cols[13] != DBNull.Value)
                            {
                                sys.SystemCustomName = new SystemCustomName
                                {
                                    Id = sys.Id,
                                    SystemAddress = (long)cols[12],
                                    CustomName = (string)cols[13]
                                };
                            }

                            if (sys.X >= x - range && sys.X <= x + range && sys.Y >= y - range && sys.Y <= y + range && sys.Z >= z - range && sys.Z <= z + range)
                            {
                                systems.Add(sys);
                            }
                        }
                    }
                }
            }

            systems.Reverse();

            return systems;
        }

        public List<SystemBody> GetSystemBodiesByPgName(int systemid, XBody pgbody)
        {
            List<SystemBody> bodies = new List<SystemBody>();

            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT b.Id, b.SystemId, b.BodyID, b.Stars, b.Planet, b.Moon1, b.Moon2, b.Moon3, b.IsBelt, n.BodyID, n.CustomName, b.ScanBaseHash, b.CustomNameId " +
                    "FROM SystemBodies b " +
                    "LEFT JOIN SystemBodyCustomNames n ON n.Id = b.Id " +
                    "WHERE b.SystemId = @SystemId AND b.Stars = @Stars AND b.IsBelt = @IsBelt AND b.Planet = @Planet AND b.Moon1 = @Moon1 AND b.Moon2 = @Moon2 AND b.Moon3 = @Moon3";
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction;
                cmd.AddParameter("@SystemId", DbType.Int32, systemid);
                cmd.AddParameter("@Stars", DbType.Byte, pgbody.Stars);
                cmd.AddParameter("@IsBelt", DbType.Boolean, pgbody.IsBelt);
                cmd.AddParameter("@Planet", DbType.Byte, pgbody.Planet);
                cmd.AddParameter("@Moon1", DbType.Byte, pgbody.Moon1);
                cmd.AddParameter("@Moon2", DbType.Byte, pgbody.Moon2);
                cmd.AddParameter("@Moon3", DbType.Byte, pgbody.Moon3);
                object[] vals = new object[13];

                using (DbDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        rdr.GetValues(vals);

                        var body = new SystemBody
                        {
                            Id = (int)vals[0],
                            SystemId = (int)vals[1],
                            BodyID = (short)vals[2],
                            Stars = (byte)vals[3],
                            Planet = (byte)vals[4],
                            Moon1 = (byte)vals[5],
                            Moon2 = (byte)vals[6],
                            Moon3 = (byte)vals[7],
                            IsBelt = BoolValue(vals[8]),
                            ScanBaseHash = (int)vals[11],
                            CustomNameId = (short)vals[12]
                        };

                        if (vals[9] != DBNull.Value)
                        {
                            body.CustomName = new SystemBodyCustomName
                            {
                                Id = (int)vals[0],
                                BodyID = (short)vals[9],
                                CustomName = (string)vals[10]
                            };
                        }

                        bodies.Add(body);
                    }
                }
            }

            bodies.Reverse();

            return bodies;
        }

        public List<SystemBody> GetSystemBodiesByCustomName(int systemid, string name)
        {
            List<SystemBody> bodies = new List<SystemBody>();

            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT b.Id, b.SystemId, b.BodyID, b.Stars, b.Planet, b.Moon1, b.Moon2, b.Moon3, b.IsBelt, n.BodyID, n.CustomName, b.ScanBaseHash, b.CustomNameId " +
                    "FROM SystemBodies b " +
                    "JOIN SystemBodyCustomNames n ON n.Id = b.Id " +
                    "WHERE b.SystemId = @SystemId AND n.CustomName = @Name";
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction;
                cmd.AddParameter("@SystemId", DbType.Int32, systemid);
                cmd.AddParameter("@Name", DbType.String, name);
                object[] vals = new object[13];

                using (DbDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        rdr.GetValues(vals);

                        var body = new SystemBody
                        {
                            Id = (int)vals[0],
                            SystemId = (int)vals[1],
                            BodyID = (short)vals[2],
                            Stars = (byte)vals[3],
                            Planet = (byte)vals[4],
                            Moon1 = (byte)vals[5],
                            Moon2 = (byte)vals[6],
                            Moon3 = (byte)vals[7],
                            IsBelt = BoolValue(vals[8]),
                            ScanBaseHash = (int)vals[11],
                            CustomNameId = (short)vals[12]
                        };

                        body.CustomName = new SystemBodyCustomName
                        {
                            Id = (int)vals[0],
                            BodyID = (short)vals[9],
                            CustomName = (string)vals[10]
                        };

                        bodies.Add(body);
                    }
                }
            }

            bodies.Reverse();

            return bodies;
        }

        public List<BodyScan> GetBodyScans(int bodyid)
        {
            List<BodyScan> scans = new List<BodyScan>();

            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT b.Id, b.SystemBodyId, b.ParentSetId, b.AxialTilt, b.Eccentricity, b.OrbitalInclination, b.Periapsis, b.SemiMajorAxis, b.OrbitalPeriod, " +
                           "b.Radius, b.RotationPeriod, b.SurfaceTemperature, b.HasOrbit, b.TidalLock, " +
                           "s.AbsoluteMagnitude, s.StellarMass, s.Age_MY, s.StarTypeId, s.LuminosityId, " +
                           "p.PlanetClassId, p.CompositionMetal, p.CompositionRock, p.CompositionIce, p.HasComposition, p.MassEM, p.SurfaceGravity, p.VolcanismId, p.VolcanismMinor, p.VolcanismMajor, p.IsLandable, " +
                           "a.SurfacePressure, a.AtmosphereComponent1Id, a.AtmosphereComponent1Amt, a.AtmosphereComponent2Id, a.AtmosphereComponent2Amt, a.AtmosphereComponent3Id, a.AtmosphereComponent3Amt, " +
                           "a.AtmosphereId, a.AtmosphereTypeId, a.AtmosphereHot, a.AtmosphereThin, a.AtmosphereThick, " +
                           "m.MaterialCarbon, m.MaterialIron, m.MaterialNickel, m.MaterialPhosphorus, m.MaterialSulphur, " +
                           "m.Material1Id, m.Material1Amt, m.Material2Id, m.Material2Amt, m.Material3Id, m.Material3Amt, m.Material4Id, m.Material4Amt, m.Material5Id, m.Material5Amt, m.Material6Id, m.Material6Amt, " +
                           "b.ReserveLevelId, " +
                           "r1.RingNum, r1n.Name, r1.ClassId, r1.InnerRad, r1.OuterRad, r1.MassMT, r1.IsBelt, " +
                           "r2.RingNum, r2n.Name, r2.ClassId, r2.InnerRad, r2.OuterRad, r2.MassMT, r2.IsBelt, " +
                           "r3.RingNum, r3n.Name, r3.ClassId, r3.InnerRad, r3.OuterRad, r3.MassMT, r3.IsBelt, " +
                           "r4.RingNum, r4n.Name, r4.ClassId, r4.InnerRad, r4.OuterRad, r4.MassMT, r4.IsBelt, " +
                           "b.ScanBaseHash " +
                    "FROM BodyScans b " +
                    "LEFT JOIN BodyScanStars s ON s.Id = b.Id " +
                    "LEFT JOIN BodyScanPlanets p ON p.Id = b.Id " +
                    "LEFT JOIN BodyScanAtmospheres a ON a.Id = b.Id " +
                    "LEFT JOIN BodyScanMaterials m ON m.Id = b.Id " +
                    "LEFT JOIN BodyScanRings r1 ON r1.ScanId = b.Id AND r1.RingNum = 1 " +
                    "LEFT JOIN BodyScanRingCustomNames r1n ON r1n.ScanId = b.Id AND r1n.RingNum = 1 " +
                    "LEFT JOIN BodyScanRings r2 ON r2.ScanId = b.Id AND r2.RingNum = 2 " +
                    "LEFT JOIN BodyScanRingCustomNames r2n ON r2n.ScanId = b.Id AND r2n.RingNum = 2 " +
                    "LEFT JOIN BodyScanRings r3 ON r3.ScanId = b.Id AND r3.RingNum = 3 " +
                    "LEFT JOIN BodyScanRingCustomNames r3n ON r3n.ScanId = b.Id AND r3n.RingNum = 3 " +
                    "LEFT JOIN BodyScanRings r4 ON r4.ScanId = b.Id AND r4.RingNum = 4 " +
                    "LEFT JOIN BodyScanRingCustomNames r4n ON r4n.ScanId = b.Id AND r4n.RingNum = 4 " +
                    "WHERE b.SystemBodyId = @SystemBodyId";
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction;
                cmd.AddParameter("@SystemBodyId", DbType.Int32, bodyid);
                object[] vals = new object[89];

                using (DbDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        rdr.GetValues(vals);

                        BodyScan body = null;

                        if (vals[17] != DBNull.Value)
                        {
                            BodyScanStar star = new BodyScanStar
                            {
                                AbsoluteMagnitude = (float)vals[14],
                                StellarMass = (float)vals[15],
                                Age_MY = (short)vals[16],
                                StarTypeId = (byte)vals[17],
                                LuminosityId = vals[18] as byte? ?? 0
                            };

                            body = star;
                        }
                        else if (vals[19] != DBNull.Value)
                        {
                            BodyScanPlanet planet = new BodyScanPlanet
                            {
                                PlanetClassId = (byte)vals[19],
                                CompositionMetal = (float)vals[20],
                                CompositionRock = (float)vals[21],
                                CompositionIce = (float)vals[22],
                                HasComposition = BoolValue(vals[23]),
                                MassEM = (float)vals[24],
                                SurfaceGravity = (float)vals[25],
                                VolcanismId = vals[26] as byte?,
                                VolcanismMinor = BoolValue(vals[27]),
                                VolcanismMajor = BoolValue(vals[28]),
                                IsLandable = BoolNullValue(vals[29])
                            };

                            if (vals[30] != DBNull.Value)
                            {
                                planet.Atmosphere = new BodyScanAtmosphere
                                {
                                    SurfacePressure = (float)vals[30],
                                    AtmosphereComponent1Id = vals[31] as byte?,
                                    AtmosphereComponent1Amt = (float)vals[32],
                                    AtmosphereComponent2Id = vals[33] as byte?,
                                    AtmosphereComponent2Amt = (float)vals[34],
                                    AtmosphereComponent3Id = vals[35] as byte?,
                                    AtmosphereComponent3Amt = (float)vals[36],
                                    AtmosphereId = (byte)vals[37],
                                    AtmosphereTypeId = vals[38] as byte?,
                                    AtmosphereHot = BoolValue(vals[39]),
                                    AtmosphereThin = BoolValue(vals[40]),
                                    AtmosphereThick = BoolValue(vals[41]),
                                };
                            }

                            if (vals[42] != DBNull.Value)
                            {
                                planet.Materials = new BodyScanMaterials
                                {
                                    MaterialCarbon = (float)vals[42],
                                    MaterialIron = (float)vals[43],
                                    MaterialNickel = (float)vals[44],
                                    MaterialPhosphorus = (float)vals[45],
                                    MaterialSulphur = (float)vals[46],
                                    Material1Id = (byte)vals[47],
                                    Material1Amt = (float)vals[48],
                                    Material2Id = (byte)vals[49],
                                    Material2Amt = (float)vals[50],
                                    Material3Id = (byte)vals[51],
                                    Material3Amt = (float)vals[52],
                                    Material4Id = (byte)vals[53],
                                    Material4Amt = (float)vals[54],
                                    Material5Id = (byte)vals[55],
                                    Material5Amt = (float)vals[56],
                                    Material6Id = (byte)vals[57],
                                    Material6Amt = (float)vals[58],
                                };
                            }

                            body = planet;
                        }
                        else
                        {
                            body = new BodyScan();
                        }

                        body.Id = (int)vals[0];
                        body.SystemBodyId = (int)vals[1];
                        body.ParentSetId = vals[2] as int?;
                        body.AxialTilt = vals[3] as float?;
                        body.Eccentricity = (float)vals[4];
                        body.OrbitalInclination = (float)vals[5];
                        body.Periapsis = (float)vals[6];
                        body.SemiMajorAxis = (float)vals[7];
                        body.OrbitalPeriod = (float)vals[8];
                        body.Radius = (float)vals[9];
                        body.RotationPeriod = (float)vals[10];
                        body.SurfaceTemperature = (float)vals[11];
                        body.HasOrbit = BoolValue(vals[12]);
                        body.TidalLock = BoolNullValue(vals[13]);
                        body.ReserveLevelId = vals[59] as byte?;
                        body.ScanBaseHash = (int)vals[88];
                        body.Rings = new List<BodyScanRing>();

                        if (vals[60] != DBNull.Value)
                        {
                            body.Rings.Add(new BodyScanRing
                            {
                                ScanId = (int)vals[0],
                                RingNum = 1,
                                Name = vals[61] as string,
                                ClassId = (byte)vals[62],
                                InnerRad = (float)vals[63],
                                OuterRad = (float)vals[64],
                                MassMT = (float)vals[65],
                                IsBelt = BoolValue(vals[66]),
                            });

                            if (vals[67] != DBNull.Value)
                            {
                                body.Rings.Add(new BodyScanRing
                                {
                                    ScanId = (int)vals[0],
                                    RingNum = 2,
                                    Name = vals[68] as string,
                                    ClassId = (byte)vals[69],
                                    InnerRad = (float)vals[70],
                                    OuterRad = (float)vals[71],
                                    MassMT = (float)vals[72],
                                    IsBelt = BoolValue(vals[73]),
                                });

                                if (vals[74] != DBNull.Value)
                                {
                                    body.Rings.Add(new BodyScanRing
                                    {
                                        ScanId = (int)vals[0],
                                        RingNum = 3,
                                        Name = vals[75] as string,
                                        ClassId = (byte)vals[76],
                                        InnerRad = (float)vals[77],
                                        OuterRad = (float)vals[78],
                                        MassMT = (float)vals[79],
                                        IsBelt = BoolValue(vals[80]),
                                    });

                                    if (vals[81] != DBNull.Value)
                                    {
                                        body.Rings.Add(new BodyScanRing
                                        {
                                            ScanId = (int)vals[0],
                                            RingNum = 4,
                                            Name = vals[82] as string,
                                            ClassId = (byte)vals[83],
                                            InnerRad = (float)vals[84],
                                            OuterRad = (float)vals[85],
                                            MassMT = (float)vals[86],
                                            IsBelt = BoolValue(vals[87]),
                                        });
                                    }
                                }
                            }
                        }

                        scans.Add(body);
                    }
                }
            }

            scans.Reverse();

            return scans;
        }

        public List<ParentSet> GetParentSets(XParentSet scan)
        {
            List<ParentSet> parentsets = new List<ParentSet>();

            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT Id, ParentId, Parent0TypeId, Parent1BodyId, Parent1TypeId, Parent2BodyId, Parent2TypeId, Parent3BodyId, Parent3TypeId, Parent4BodyId, Parent4TypeId, Parent5BodyId, Parent5TypeId, Parent6BodyId, Parent6TypeId, Parent7BodyId, Parent7TypeId " +
                    "FROM ParentSets " +
                    "WHERE Parent1BodyId = @Parent1ID AND Parent2BodyId = @Parent2ID AND Parent3BodyId = @Parent3ID AND Parent4BodyId = @Parent4ID AND Parent5BodyId = @Parent5ID AND Parent6BodyId = @Parent6ID AND Parent7BodyId = @Parent7ID";
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction;
                cmd.AddParameter("@Parent1ID", DbType.Int16, scan.Parent1BodyID);
                cmd.AddParameter("@Parent2ID", DbType.Int16, scan.Parent2BodyID);
                cmd.AddParameter("@Parent3ID", DbType.Int16, scan.Parent3BodyID);
                cmd.AddParameter("@Parent4ID", DbType.Int16, scan.Parent4BodyID);
                cmd.AddParameter("@Parent5ID", DbType.Int16, scan.Parent5BodyID);
                cmd.AddParameter("@Parent6ID", DbType.Int16, scan.Parent6BodyID);
                cmd.AddParameter("@Parent7ID", DbType.Int16, scan.Parent7BodyID);
                object[] vals = new object[17];

                using (DbDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        rdr.GetValues(vals);

                        ParentSet parentset = new ParentSet
                        {
                            Id = (int)vals[0],
                            ParentId = vals[1] as int?,
                            Parent0TypeId = (byte)vals[2],
                            Parent1BodyId = (short)vals[3],
                            Parent1TypeId = vals[4] as byte?,
                            Parent2BodyId = (short)vals[5],
                            Parent2TypeId = vals[6] as byte?,
                            Parent3BodyId = (short)vals[7],
                            Parent3TypeId = vals[8] as byte?,
                            Parent4BodyId = (short)vals[9],
                            Parent4TypeId = vals[10] as byte?,
                            Parent5BodyId = (short)vals[11],
                            Parent5TypeId = vals[12] as byte?,
                            Parent6BodyId = (short)vals[13],
                            Parent6TypeId = vals[14] as byte?,
                            Parent7BodyId = (short)vals[15],
                            Parent7TypeId = vals[16] as byte?,
                        };

                        parentsets.Add(parentset);
                    }
                }
            }

            return parentsets;

        }

        public List<EDDNJournalScan> GetEDDNScansByGatewayTimestamp(DateTime timestamp)
        {
            List<EDDNJournalScan> scans = new List<EDDNJournalScan>();
            long ticks = timestamp.Subtract(EDDNJournalScan.ScanTimeZero).Ticks;

            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT j.Id, BodyScanId, GatewayTimestamp, ScanTimestampSeconds, SoftwareVersionId, ScanTypeId, DistanceFromArrivalLS," +
                          " HasSystemAddress, HasBodyID, HasParents, HasComposition, HasAxialTilt, HasLuminosity, IsMaterialsDict," +
                          " IsBasicScan, IsPos3SigFig, HasAtmosphereType, HasAtmosphereComposition, JsonExtra " +
                    "FROM EDDNJournalScans j " +
                    "LEFT JOIN EDDNJournalScanJsonExtras je ON je.Id = j.Id " +
                    "WHERE j.GatewayTimestampTicks = @GatewayTimestampTicks";
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction;
                cmd.AddParameter("@GatewayTimestampTicks", DbType.Int64, ticks);
                object[] cols = new object[19];

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        rdr.GetValues(cols);
                        EDDNJournalScan js = new EDDNJournalScan
                        {
                            Id = (int)cols[0],
                            BodyScanId = (int)cols[1],
                            GatewayTimestampTicks = (long)cols[2],
                            ScanTimestampSeconds = (int)cols[3],
                            SoftwareVersionId = (short)cols[4],
                            ScanTypeId = cols[5] as byte?,
                            DistanceFromArrivalLS = (float)cols[6],
                            HasSystemAddress = BoolValue(cols[7]),
                            HasBodyID = BoolValue(cols[8]),
                            HasParents = BoolValue(cols[9]),
                            HasComposition = BoolValue(cols[10]),
                            HasAxialTilt = BoolValue(cols[11]),
                            HasLuminosity = BoolValue(cols[12]),
                            IsMaterialsDict = BoolValue(cols[13]),
                            IsBasicScan = BoolValue(cols[14]),
                            IsPos3SigFig = BoolValue(cols[15]),
                            HasAtmosphereType = BoolValue(cols[16]),
                            HasAtmosphereComposition = BoolValue(cols[17]),
                            JsonExtra = cols[18] as string,
                        };

                        scans.Add(js);
                    }
                }
            }

            scans.Reverse();

            return scans;
        }

        public List<EDDNJournalScan> GetEDDNScansByScanId(int scanid)
        {
            List<EDDNJournalScan> scans = new List<EDDNJournalScan>();

            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT j.Id, BodyScanId, GatewayTimestampTicks, ScanTimestampSeconds, SoftwareVersionId, ScanTypeId, DistanceFromArrivalLS," +
                          " HasSystemAddress, HasBodyID, HasParents, HasComposition, HasAxialTilt, HasLuminosity, IsMaterialsDict," +
                          " IsBasicScan, IsPos3SigFig, HasAtmosphereType, HasAtmosphereComposition, JsonExtra " +
                    "FROM EDDNJournalScans j " +
                    "LEFT JOIN EDDNJournalScanJsonExtras je ON je.Id = j.Id " +
                    "WHERE j.BodyScanId = @BodyScanId";
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction;
                cmd.AddParameter("@BodyScanId", DbType.Int32, scanid);
                object[] cols = new object[19];

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        rdr.GetValues(cols);
                        EDDNJournalScan js = new EDDNJournalScan
                        {
                            Id = (int)cols[0],
                            BodyScanId = (int)cols[1],
                            GatewayTimestampTicks = (long)cols[2],
                            ScanTimestampSeconds = (int)cols[3],
                            SoftwareVersionId = (short)cols[4],
                            ScanTypeId = cols[5] as byte?,
                            DistanceFromArrivalLS = (float)cols[6],
                            HasSystemAddress = BoolValue(cols[7]),
                            HasBodyID = BoolValue(cols[8]),
                            HasParents = BoolValue(cols[9]),
                            HasComposition = BoolValue(cols[10]),
                            HasAxialTilt = BoolValue(cols[11]),
                            HasLuminosity = BoolValue(cols[12]),
                            IsMaterialsDict = BoolValue(cols[13]),
                            IsBasicScan = BoolValue(cols[14]),
                            IsPos3SigFig = BoolValue(cols[15]),
                            HasAtmosphereType = BoolValue(cols[16]),
                            HasAtmosphereComposition = BoolValue(cols[17]),
                            JsonExtra = cols[18] as string,
                        };

                        scans.Add(js);
                    }
                }
            }

            scans.Reverse();

            return scans;
        }

        public IEnumerable<BodyScan> GetAllBodyScans(int start, int limit)
        {
            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            using (var cmd = Database.Connection.CreateCommand())
            {
                string cmdtext =
                    "SELECT b.Id, b.SystemBodyId, b.ParentSetId, b.AxialTilt, b.Eccentricity, b.OrbitalInclination, b.Periapsis, b.SemiMajorAxis, b.OrbitalPeriod, " +
                           "b.Radius, b.RotationPeriod, b.SurfaceTemperature, b.HasOrbit, b.TidalLock, " +
                           "s.AbsoluteMagnitude, s.StellarMass, s.Age_MY, s.StarTypeId, s.LuminosityId, " +
                           "p.PlanetClassId, p.CompositionMetal, p.CompositionRock, p.CompositionIce, p.HasComposition, p.MassEM, p.SurfaceGravity, p.VolcanismId, p.VolcanismMinor, p.VolcanismMajor, p.IsLandable, " +
                           "a.SurfacePressure, a.AtmosphereComponent1Id, a.AtmosphereComponent1Amt, a.AtmosphereComponent2Id, a.AtmosphereComponent2Amt, a.AtmosphereComponent3Id, a.AtmosphereComponent3Amt, " +
                           "a.AtmosphereId, a.AtmosphereTypeId, a.AtmosphereHot, a.AtmosphereThin, a.AtmosphereThick, " +
                           "m.MaterialCarbon, m.MaterialIron, m.MaterialNickel, m.MaterialPhosphorus, m.MaterialSulphur, " +
                           "m.Material1Id, m.Material1Amt, m.Material2Id, m.Material2Amt, m.Material3Id, m.Material3Amt, m.Material4Id, m.Material4Amt, m.Material5Id, m.Material5Amt, m.Material6Id, m.Material6Amt, " +
                           "b.ReserveLevelId, " +
                           "r1.RingNum, r1n.Name, r1.ClassId, r1.InnerRad, r1.OuterRad, r1.MassMT, r1.IsBelt, " +
                           "r2.RingNum, r2n.Name, r2.ClassId, r2.InnerRad, r2.OuterRad, r2.MassMT, r2.IsBelt, " +
                           "r3.RingNum, r3n.Name, r3.ClassId, r3.InnerRad, r3.OuterRad, r3.MassMT, r3.IsBelt, " +
                           "r4.RingNum, r4n.Name, r4.ClassId, r4.InnerRad, r4.OuterRad, r4.MassMT, r4.IsBelt, " +
                           "b.ScanBaseHash, " +
                           "sb.SystemId, sb.BodyID, sb.Stars, sb.IsBelt, sb.Planet, sb.Moon1, sb.Moon2, sb.Moon3, sb.CustomNameId " +
                    "FROM SystemBodies sb " +
                    "JOIN BodyScans b ON b.SystemBodyId = sb.Id " +
                    "LEFT JOIN BodyScanStars s ON s.Id = b.Id " +
                    "LEFT JOIN BodyScanPlanets p ON p.Id = b.Id " +
                    "LEFT JOIN BodyScanAtmospheres a ON a.Id = b.Id " +
                    "LEFT JOIN BodyScanMaterials m ON m.Id = b.Id " +
                    "LEFT JOIN BodyScanRings r1 ON r1.ScanId = b.Id AND r1.RingNum = 1 " +
                    "LEFT JOIN BodyScanRingCustomNames r1n ON r1n.ScanId = b.Id AND r1n.RingNum = 1 " +
                    "LEFT JOIN BodyScanRings r2 ON r2.ScanId = b.Id AND r2.RingNum = 2 " +
                    "LEFT JOIN BodyScanRingCustomNames r2n ON r2n.ScanId = b.Id AND r2n.RingNum = 2 " +
                    "LEFT JOIN BodyScanRings r3 ON r3.ScanId = b.Id AND r3.RingNum = 3 " +
                    "LEFT JOIN BodyScanRingCustomNames r3n ON r3n.ScanId = b.Id AND r3n.RingNum = 3 " +
                    "LEFT JOIN BodyScanRings r4 ON r4.ScanId = b.Id AND r4.RingNum = 4 " +
                    "LEFT JOIN BodyScanRingCustomNames r4n ON r4n.ScanId = b.Id AND r4n.RingNum = 4 " +
                    "WHERE b.Id > @Offset " +
                    "ORDER BY b.Id ASC ";

                if (cmd is global::System.Data.SqlClient.SqlCommand)
                {
                    cmdtext = cmdtext + "OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY";
                }
                else if (cmd is global::MySql.Data.MySqlClient.MySqlCommand)
                {
                    cmdtext = cmdtext + "LIMIT @Limit";
                }

                cmd.CommandText = cmdtext;
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = Database.CurrentTransaction?.UnderlyingTransaction;
                cmd.AddParameter("@Limit", DbType.Int32, limit);
                cmd.AddParameter("@Offset", DbType.Int32, start);
                object[] vals = new object[98];

                using (DbDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        rdr.GetValues(vals);

                        BodyScan body = null;

                        if (vals[17] != DBNull.Value)
                        {
                            BodyScanStar star = new BodyScanStar
                            {
                                AbsoluteMagnitude = (float)vals[14],
                                StellarMass = (float)vals[15],
                                Age_MY = (short)vals[16],
                                StarTypeId = (byte)vals[17],
                                LuminosityId = vals[18] as byte? ?? 0
                            };

                            body = star;
                        }
                        else if (vals[19] != DBNull.Value)
                        {
                            BodyScanPlanet planet = new BodyScanPlanet
                            {
                                PlanetClassId = (byte)vals[19],
                                CompositionMetal = (float)vals[20],
                                CompositionRock = (float)vals[21],
                                CompositionIce = (float)vals[22],
                                HasComposition = BoolValue(vals[23]),
                                MassEM = (float)vals[24],
                                SurfaceGravity = (float)vals[25],
                                VolcanismId = vals[26] as byte?,
                                VolcanismMinor = BoolValue(vals[27]),
                                VolcanismMajor = BoolValue(vals[28]),
                                IsLandable = BoolNullValue(vals[29])
                            };

                            if (vals[30] != DBNull.Value)
                            {
                                planet.Atmosphere = new BodyScanAtmosphere
                                {
                                    SurfacePressure = (float)vals[30],
                                    AtmosphereComponent1Id = vals[31] as byte?,
                                    AtmosphereComponent1Amt = (float)vals[32],
                                    AtmosphereComponent2Id = vals[33] as byte?,
                                    AtmosphereComponent2Amt = (float)vals[34],
                                    AtmosphereComponent3Id = vals[35] as byte?,
                                    AtmosphereComponent3Amt = (float)vals[36],
                                    AtmosphereId = (byte)vals[37],
                                    AtmosphereTypeId = vals[38] as byte?,
                                    AtmosphereHot = BoolValue(vals[39]),
                                    AtmosphereThin = BoolValue(vals[40]),
                                    AtmosphereThick = BoolValue(vals[41]),
                                };
                            }

                            if (vals[42] != DBNull.Value)
                            {
                                planet.Materials = new BodyScanMaterials
                                {
                                    MaterialCarbon = (float)vals[42],
                                    MaterialIron = (float)vals[43],
                                    MaterialNickel = (float)vals[44],
                                    MaterialPhosphorus = (float)vals[45],
                                    MaterialSulphur = (float)vals[46],
                                    Material1Id = (byte)vals[47],
                                    Material1Amt = (float)vals[48],
                                    Material2Id = (byte)vals[49],
                                    Material2Amt = (float)vals[50],
                                    Material3Id = (byte)vals[51],
                                    Material3Amt = (float)vals[52],
                                    Material4Id = (byte)vals[53],
                                    Material4Amt = (float)vals[54],
                                    Material5Id = (byte)vals[55],
                                    Material5Amt = (float)vals[56],
                                    Material6Id = (byte)vals[57],
                                    Material6Amt = (float)vals[58],
                                };
                            }

                            body = planet;
                        }
                        else
                        {
                            body = new BodyScan();
                        }

                        body.Id = (int)vals[0];
                        body.SystemBodyId = (int)vals[1];
                        body.ParentSetId = vals[2] as int?;
                        body.AxialTilt = vals[3] as float?;
                        body.Eccentricity = (float)vals[4];
                        body.OrbitalInclination = (float)vals[5];
                        body.Periapsis = (float)vals[6];
                        body.SemiMajorAxis = (float)vals[7];
                        body.OrbitalPeriod = (float)vals[8];
                        body.Radius = (float)vals[9];
                        body.RotationPeriod = (float)vals[10];
                        body.SurfaceTemperature = (float)vals[11];
                        body.HasOrbit = BoolValue(vals[12]);
                        body.TidalLock = BoolNullValue(vals[13]);
                        body.ReserveLevelId = vals[59] as byte?;
                        body.ScanBaseHash = (int)vals[88];
                        body.Rings = new List<BodyScanRing>();

                        if (vals[60] != DBNull.Value)
                        {
                            body.Rings.Add(new BodyScanRing
                            {
                                ScanId = (int)vals[0],
                                RingNum = 1,
                                Name = vals[61] as string,
                                ClassId = (byte)vals[62],
                                InnerRad = (float)vals[63],
                                OuterRad = (float)vals[64],
                                MassMT = (float)vals[65],
                                IsBelt = BoolValue(vals[66]),
                            });

                            if (vals[67] != DBNull.Value)
                            {
                                body.Rings.Add(new BodyScanRing
                                {
                                    ScanId = (int)vals[0],
                                    RingNum = 2,
                                    Name = vals[68] as string,
                                    ClassId = (byte)vals[69],
                                    InnerRad = (float)vals[70],
                                    OuterRad = (float)vals[71],
                                    MassMT = (float)vals[72],
                                    IsBelt = BoolValue(vals[73]),
                                });

                                if (vals[74] != DBNull.Value)
                                {
                                    body.Rings.Add(new BodyScanRing
                                    {
                                        ScanId = (int)vals[0],
                                        RingNum = 3,
                                        Name = vals[75] as string,
                                        ClassId = (byte)vals[76],
                                        InnerRad = (float)vals[77],
                                        OuterRad = (float)vals[78],
                                        MassMT = (float)vals[79],
                                        IsBelt = BoolValue(vals[80]),
                                    });

                                    if (vals[81] != DBNull.Value)
                                    {
                                        body.Rings.Add(new BodyScanRing
                                        {
                                            ScanId = (int)vals[0],
                                            RingNum = 4,
                                            Name = vals[82] as string,
                                            ClassId = (byte)vals[83],
                                            InnerRad = (float)vals[84],
                                            OuterRad = (float)vals[85],
                                            MassMT = (float)vals[86],
                                            IsBelt = BoolValue(vals[87]),
                                        });
                                    }
                                }
                            }
                        }

                        body.SystemBody = new SystemBody
                        {
                            Id = (int)vals[1],
                            SystemId = (int)vals[89],
                            BodyID = (short)vals[90],
                            Stars = (byte)vals[91],
                            IsBelt = BoolValue(vals[92]),
                            Planet = (byte)vals[93],
                            Moon1 = (byte)vals[94],
                            Moon2 = (byte)vals[95],
                            Moon3 = (byte)vals[96],
                            CustomNameId = (short)vals[97],
                            ScanBaseHash = (int)vals[88]
                        };

                        yield return body;
                    }
                }
            }
        }

        public void InsertSystems(IEnumerable<System> systems)
        {
            string syscmdtext =
                "INSERT INTO Systems (ModSystemAddress, X, Y, Z, RegionId, SizeClass, Mid3, Mid2, Mid1b, Mid1a, Sequence, EdsmId, EdsmLastModifiedSeconds)";
            string syscmdtext2 = " VALUES (@ModSystemAddress, @X, @Y, @Z, @RegionId, @SizeClass, @Mid3, @Mid2, @Mid1b, @Mid1a, @Sequence, @EdsmId, @EdsmLastModifiedSeconds)";

            Func<DbCommand, int> execsyscmd = GetInsertIdentity(ref syscmdtext, ref syscmdtext2);

            syscmdtext += syscmdtext2;

            if (execsyscmd == null)
            {
                Set<System>().AddRange(systems);
                SaveChanges();
                return;
            }

            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            using (var txn = Database.Connection.BeginTransaction())
            {
                DbCommand syscmd = null;
                DbCommand namecmd = null;

                try
                {
                    syscmd = Database.Connection.CreateCommand();
                    syscmd.Transaction = txn;

                    syscmd.CommandText = syscmdtext;
                    syscmd.CommandType = CommandType.Text;
                    var paramSysaddr = syscmd.AddParameter("@ModSystemAddress", DbType.Int64);
                    var paramX = syscmd.AddParameter("@X", DbType.Int32);
                    var paramY = syscmd.AddParameter("@Y", DbType.Int32);
                    var paramZ = syscmd.AddParameter("@Z", DbType.Int32);
                    var paramRegionId = syscmd.AddParameter("@RegionId", DbType.Int16);
                    var paramSizeClass = syscmd.AddParameter("@SizeClass", DbType.Byte);
                    var paramMid3 = syscmd.AddParameter("@Mid3", DbType.Byte);
                    var paramMid2 = syscmd.AddParameter("@Mid2", DbType.Byte);
                    var paramMid1b = syscmd.AddParameter("@Mid1b", DbType.Byte);
                    var paramMid1a = syscmd.AddParameter("@Mid1a", DbType.Byte);
                    var paramSequence = syscmd.AddParameter("@Sequence", DbType.Int16);
                    var paramEdsmId = syscmd.AddParameter("@EdsmId", DbType.Int32);
                    var paramEdsmLastModified = syscmd.AddParameter("@EdsmLastModifiedSeconds", DbType.Int32);

                    namecmd = Database.Connection.CreateCommand();
                    namecmd.Transaction = txn;

                    namecmd.CommandText = "INSERT INTO SystemCustomNames (Id, SystemAddress, CustomName) VALUES (@Id, @SystemAddress, @CustomName)";
                    var paramNameId = namecmd.AddParameter("@Id", DbType.Int32);
                    var paramNameSysAddr = namecmd.AddParameter("@SystemAddress", DbType.Int64);
                    var paramCustomName = namecmd.AddParameter("@CustomName", DbType.String);

                    foreach (var sys in systems)
                    {
                        paramSysaddr.Value = sys.ModSystemAddress;
                        paramX.Value = sys.X;
                        paramY.Value = sys.Y;
                        paramZ.Value = sys.Z;
                        paramRegionId.Value = sys.RegionId;
                        paramSizeClass.Value = sys.SizeClass;
                        paramMid3.Value = sys.Mid3;
                        paramMid2.Value = sys.Mid2;
                        paramMid1b.Value = sys.Mid1b;
                        paramMid1a.Value = sys.Mid1a;
                        paramSequence.Value = sys.Sequence;
                        paramEdsmId.Value = sys.EdsmId;
                        paramEdsmLastModified.Value = sys.EdsmLastModifiedSeconds;
                        sys.Id = execsyscmd(syscmd);

                        if (sys.SystemCustomName != null)
                        {
                            sys.SystemCustomName.Id = sys.Id;
                            paramNameId.Value = sys.SystemCustomName.Id;
                            paramNameSysAddr.Value = sys.SystemCustomName.SystemAddress;
                            paramCustomName.Value = sys.SystemCustomName.CustomName;
                            namecmd.ExecuteNonQuery();
                        }
                    }
                }
                finally
                {
                    if (syscmd != null)
                    {
                        syscmd.Dispose();
                    }

                    if (namecmd != null)
                    {
                        namecmd.Dispose();
                    }
                }

                txn.Commit();
            }
        }

        public System GetOrAddSystem(XSystem system, string customname = null, bool notrack = false, bool noadd = false, int? edsmid = null)
        {
            bool haspgsys = system.RegionId != 0;
            bool ispgname = customname == null;
            int _x = (int)Math.Floor((system.StarPosX + 49985) * 32 + 0.5);
            int _y = (int)Math.Floor((system.StarPosY + 40985) * 32 + 0.5);
            int _z = (int)Math.Floor((system.StarPosZ + 24105) * 32 + 0.5);
            long sysaddr = system.SystemAddress;
            SystemStruct ps = new SystemStruct
            {
                RegionId = system.RegionId,
                SizeClass = system.SizeClass,
                Mid1a = system.Mid1a,
                Mid1b = system.Mid1b,
                Mid2 = system.Mid2,
                Mid3 = system.Mid3,
                Sequence = system.Sequence,
            };

            if (sysaddr == 0 && ps.RegionId != 0)
            {
                sysaddr = ps.ToSystemAddress();
            }

            List<System> msys = new List<System>();

            if (sysaddr != 0)
            {
                foreach (System sys in GetSystemsByAddress(sysaddr))
                {
                    msys.Add(sys);
                    if (!String.Equals(sys.SystemCustomName?.CustomName, customname, StringComparison.InvariantCultureIgnoreCase) || sys.X != _x || sys.Y != _y || sys.Z != _z)
                    {
                        continue;
                    }

                    if (ps.RegionId != 0 && (ps.RegionId != sys.RegionId || ps.SizeClass != sys.SizeClass || ps.Mid1a != sys.Mid1a || ps.Mid1b != sys.Mid1b || ps.Mid2 != sys.Mid2 || ps.Mid3 != sys.Mid3 || ps.Sequence != ps.Sequence))
                    {
                        continue;
                    }

                    if (edsmid != null && sys.EdsmId != 0 && edsmid != sys.EdsmId)
                    {
                        continue;
                    }

                    if (notrack)
                    {
                        return sys;
                    }
                    else
                    {
                        var retsys = Set<System>().Find(sys.Id);
                        if (sys.SystemCustomName != null)
                        {
                            retsys.SystemCustomName = Set<SystemCustomName>().Find(sys.Id);
                        }
                        return retsys;
                    }
                }
            }
            else
            {
                if (!ispgname)
                {
                    foreach (System sys in GetSystemsByCustomName(customname))
                    {
                        msys.Add(sys);
                        if (Math.Abs(sys.X - _x) > 2 || Math.Abs(sys.Y - _y) > 2 || Math.Abs(sys.Z - _z) > 2)
                        {
                            continue;
                        }

                        sysaddr = sys.SystemAddress;
                        ps = new SystemStruct
                        {
                            RegionId = sys.RegionId,
                            SizeClass = sys.SizeClass,
                            Mid3 = sys.Mid3,
                            Mid2 = sys.Mid2,
                            Mid1b = sys.Mid1b,
                            Mid1a = sys.Mid1a,
                            Sequence = sys.Sequence
                        };

                        if (sys.X != _x || sys.Y != _y || sys.Z != _z)
                        {
                            continue;
                        }

                        if (edsmid != null && sys.EdsmId != 0 && edsmid != sys.EdsmId)
                        {
                            continue;
                        }

                        if (notrack)
                        {
                            return sys;
                        }
                        else
                        {
                            var retsys = Set<System>().Find(sys.Id);
                            if (sys.SystemCustomName != null)
                            {
                                Entry(retsys).Reference(e => e.SystemCustomName).Load();
                            }
                            return retsys;
                        }
                    }
                }
            }

            if (sysaddr == 0)
            {
                long? fsysaddr = null;
                bool multimatch = false;
                System match1 = null;
                System match2 = null;

                foreach (System sys in FindSystemsByCoordinates(_x, _y, _z))
                {
                    if (fsysaddr == null)
                    {
                        fsysaddr = sys.SystemAddress;
                        match1 = sys;
                    }
                    else if (fsysaddr != sys.SystemAddress)
                    {
                        multimatch = true;
                        match2 = sys;
                    }
                }

                if (multimatch)
                {
                    Trace.WriteLine($"Multiple system matches at coordinates ({system.StarPosX},{system.StarPosY},{system.StarPosZ}) - {match1.Name} ({match1.SystemAddress}) | {match2.Name} ({match2.SystemAddress})");
                }
                else if (fsysaddr != null)
                {
                    sysaddr = (long)fsysaddr;
                    ps = new SystemStruct
                    {
                        RegionId = match1.RegionId,
                        SizeClass = match1.SizeClass,
                        Mid3 = match1.Mid3,
                        Mid2 = match1.Mid2,
                        Mid1b = match1.Mid1b,
                        Mid1a = match1.Mid1a,
                        Sequence = match1.Sequence
                    };

                    if (ps.RegionId == 0)
                    {
                        ps = SystemStruct.FromSystemAddress(sysaddr);
                    }
                }
            }

            if (ps.RegionId == 0 && sysaddr != 0)
            {
                ps = SystemStruct.FromSystemAddress(sysaddr);
            }

            if (ps.RegionId == 0 || sysaddr == 0)
            {
                throw new InvalidOperationException($"Unknown system {customname}");
            }

            System ret = new System
            {
                SystemAddress = sysaddr,
                RegionId = ps.RegionId,
                SizeClass = ps.SizeClass,
                Mid3 = ps.Mid3,
                Mid2 = ps.Mid2,
                Mid1b = ps.Mid1b,
                Mid1a = ps.Mid1a,
                Sequence = ps.Sequence,
                SystemCustomName = ispgname || customname == null ? null : new SystemCustomName
                {
                    SystemAddress = sysaddr,
                    CustomName = customname
                },
                X = _x,
                Y = _y,
                Z = _z
            };

            if (!noadd)
            {
                Set<System>().Add(ret);
            }

            return ret;
        }

        public System GetOrAddSystem(string name, string pgname, long? sysaddr, float x, float y, float z, bool notrack = false, bool noadd = false, int? edsmid = null)
        {
            bool haspgsys = false;
            bool ispgname = false;
            int _x = (int)Math.Floor((x + 49985) * 32 + 0.5);
            int _y = (int)Math.Floor((y + 40985) * 32 + 0.5);
            int _z = (int)Math.Floor((z + 24105) * 32 + 0.5);

            ispgname = SystemStruct.TryParse(name, out SystemStruct ps);
            haspgsys = ispgname || SystemStruct.TryParse(pgname, out ps);

            if (ispgname)
            {
                pgname = name;
                name = null;
            }

            SystemStruct ps2 = sysaddr == null ? new SystemStruct() : SystemStruct.FromSystemAddress((long)sysaddr);

            if (!haspgsys)
            {
                ps = ps2;
            }
            else
            {
                long psysaddr = ps.ToSystemAddress();

                if (sysaddr != null && psysaddr != sysaddr)
                {
                    Trace.WriteLine($"System {name} ({pgname}) address {sysaddr} != {psysaddr}");
                }

                sysaddr = psysaddr;
            }

            XSystem sys = new XSystem
            {
                SystemAddress = sysaddr ?? 0,
                StarPosX = x,
                StarPosY = y,
                StarPosZ = z,
                RegionId = ps.RegionId,
                SizeClass = ps.SizeClass,
                Mid1a = ps.Mid1a,
                Mid1b = ps.Mid1b,
                Mid2 = ps.Mid2,
                Mid3 = ps.Mid3,
                Sequence = ps.Sequence
            };

            return GetOrAddSystem(sys, name, notrack, noadd, edsmid);
        }

        public SystemBody GetOrAddBody(XBody pgbody, System system, int scanbasehash, string customname = null, bool notrack = false, bool noadd = false, bool makenew = false, XScanClass scandata = null)
        {
            return GetOrAddBody(pgbody, system.Id, scanbasehash: scanbasehash, customname: customname, notrack: notrack, noadd: noadd, makenew: makenew, scandata: scandata);
        }

        public SystemBody GetOrAddBody(XBody pgbody, int sysid, int scanbasehash, string customname = null, bool notrack = false, bool noadd = false, bool makenew = false, XScanClass scandata = null)
        {
            Debug.Assert(sysid != 0);
            bool wantnamedbody = false;
            List<SystemBody> bodies = new List<SystemBody>();

            if (sysid != 0 && !makenew)
            {
                wantnamedbody = BodyDatabase.GetNamedBody(sysid, pgbody, scanbasehash, customname, out Models.SystemBody nbody);

                if (nbody != null)
                {
                    bodies.Add(nbody);

                    if (nbody.ScanBaseHash == scanbasehash)
                    {
                        if (notrack)
                        {
                            return nbody;
                        }
                        else
                        {
                            var retbody = Set<SystemBody>().Find(nbody.Id);
                            Entry(retbody).Reference(e => e.SystemRef).Load();
                            if (nbody.CustomName != null)
                            {
                                Entry(retbody.SystemRef).Reference(e => e.SystemCustomName).Load();
                            }
                            return retbody;
                        }
                    }
                }

                if (customname != null)
                {
                    bodies.AddRange(GetSystemBodiesByCustomName(sysid, customname).Where(b => pgbody.BodyID == -1 || b.BodyID == -1 || pgbody.BodyID == b.BodyID));
                }
                else
                {
                    bodies.AddRange(GetSystemBodiesByPgName(sysid, pgbody).Where(b => b.CustomNameId == 0 && (pgbody.BodyID == -1 || b.BodyID == -1 || pgbody.BodyID == b.BodyID)));
                }

                bodies = bodies.Distinct().ToList();

                foreach (SystemBody sysbody in bodies)
                {
                    if (sysbody.ScanBaseHash == scanbasehash && sysbody.CustomName?.CustomName == customname)
                    {
                        if (wantnamedbody)
                        {
                            BodyDatabase.AddNamedBody(sysid, customname, sysbody);
                        }

                        if (notrack)
                        {
                            return sysbody;
                        }
                        else
                        {
                            var retbody = Set<SystemBody>().Find(sysbody.Id);
                            Entry(retbody).Reference(e => e.SystemRef).Load();
                            if (sysbody.CustomName != null)
                            {
                                Entry(retbody.SystemRef).Reference(e => e.SystemCustomName).Load();
                            }
                            return retbody;
                        }
                    }
                }
            }

            bodies = bodies.Where(b => b.Id != 0).ToList();

            if (bodies.Count != 0)
            {
                var curscanbase = scandata?.ScanBase;
                var scans = bodies.SelectMany(b => GetBodyScans(b.Id)).ToList();
                var scanbases = scans.Select(s => XScanBase.From(s)).ToList();
                var sbhashes = scanbases.Select(s => s.GetHashCode()).ToList();

                if (bodies.Count == 1 && sbhashes.All(s => s == scanbasehash) && bodies[0].ScanBaseHash != scanbasehash && false)
                {
                    var xbody = bodies[0];

                    using (var cmd = Database.Connection.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE SystemBodies SET ScanBaseHash = @ScanBaseHash WHERE Id = @Id";
                        cmd.CommandType = CommandType.Text;
                        cmd.AddParameter("@ScanBaseHash", DbType.Int32, scanbasehash);
                        cmd.AddParameter("@Id", DbType.Int32, xbody.Id);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = Database.Connection.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE BodyScans SET ScanBaseHash = @ScanBaseHash WHERE SystemBodyId = @Id";
                        cmd.CommandType = CommandType.Text;
                        cmd.AddParameter("@ScanBaseHash", DbType.Int32, scanbasehash);
                        cmd.AddParameter("@Id", DbType.Int32, xbody.Id);
                        cmd.ExecuteNonQuery();
                    }

                    if (notrack)
                    {
                        return xbody;
                    }
                    else
                    {
                        var retbody = Set<SystemBody>().Find(xbody.Id);
                        Entry(retbody).Reference(e => e.SystemRef).Load();
                        if (xbody.CustomName != null)
                        {
                            Entry(retbody.SystemRef).Reference(e => e.SystemCustomName).Load();
                        }
                        return retbody;
                    }
                }

                var diffprops = scans.Select(s => s.GetDifferingProps(scandata, new List<BodyScan.CompareResult>())).ToList();
                var majdiffprops = diffprops.Select(s => s.Where(p => !p.IsMinorDifference && !p.IsTweakedProperty).ToList()).ToList();

                if (majdiffprops.All(p => p.Count == 1) || diffprops.All(p => p.Count < 6))
                {
                    //Debugger.Break();
                }
            }

            SystemBody body = new SystemBody
            {
                SystemId = sysid,
                BodyID = pgbody.BodyID,
                Stars = pgbody.Stars,
                Planet = pgbody.Planet,
                IsBelt = pgbody.IsBelt,
                Moon1 = pgbody.Moon1,
                Moon2 = pgbody.Moon2,
                Moon3 = pgbody.Moon3,
                ScanBaseHash = scanbasehash,
                CustomNameId = BodyDatabase.BodyCustomName.GetId(customname) ?? 0,
            };

            if (customname != null)
            {
                body.CustomName = new SystemBodyCustomName
                {
                    BodyID = pgbody.BodyID,
                    CustomName = customname
                };
            }

            if (wantnamedbody)
            {
                BodyDatabase.AddNamedBody(sysid, customname, body);
            }

            if (!noadd)
            {
                Set<SystemBody>().Add(body);
            }

            return body;
        }

        public SystemBody GetOrAddBody(string name, System system, int scanbasehash, int bodyid = -1, bool notrack = false, bool noadd = false, bool makenew = false, XScanClass scandata = null)
        {
            bool ispgbody = SystemBodyStruct.TryParse(name, system.Name, out SystemBodyStruct pgbody);
            return GetOrAddBody(XBody.From(in pgbody, (short)bodyid), system, scanbasehash: scanbasehash, customname: ispgbody ? null : name, notrack: notrack, noadd: noadd, makenew: makenew, scandata: scandata);
        }

        public ParentSet GetOrAddParentSet(XParentSet scan, bool notrack = false, bool noadd = false)
        {
            if (scan.Parent0BodyID != 0)
            {
                return null;
            }

            foreach (ParentSet set in GetParentSets(scan))
            {
                if (set.Equals(scan))
                {
                    if (notrack)
                    {
                        return set;
                    }
                    else
                    {
                        return Set<ParentSet>().Find(set.Id);
                    }
                }
            }

            ParentSet parentpset = null;

            if (scan.Parent1BodyID != 0)
            {
                var parent = scan;

                if (scan.Parent7BodyID != 0)
                {
                    parent.Parent7BodyID = 0;
                    parent.Parent7Type = null;
                }
                else if (scan.Parent6BodyID != 0)
                {
                    parent.Parent6BodyID = 0;
                    parent.Parent6Type = null;
                }
                else if (scan.Parent5BodyID != 0)
                {
                    parent.Parent5BodyID = 0;
                    parent.Parent5Type = null;
                }
                else if (scan.Parent4BodyID != 0)
                {
                    parent.Parent4BodyID = 0;
                    parent.Parent4Type = null;
                }
                else if (scan.Parent3BodyID != 0)
                {
                    parent.Parent3BodyID = 0;
                    parent.Parent3Type = null;
                }
                else if (scan.Parent2BodyID != 0)
                {
                    parent.Parent2BodyID = 0;
                    parent.Parent2Type = null;
                }
                else
                {
                    parent.Parent1BodyID = 0;
                    parent.Parent1Type = null;
                }

                parentpset = GetOrAddParentSet(parent);
            }

            ParentSet pset = new ParentSet(scan)
            {
                ParentRef = parentpset
            };

            if (!noadd)
            {
                Set<ParentSet>().Add(pset);
            }

            return pset;
        }

        public BodyScan GetOrAddScan(SystemBody sysbody, XScanClass scanstruct, bool notrack = false, bool noadd = false)
        {
            if (sysbody.Id != 0)
            {
                var scans = GetBodyScans(sysbody.Id).Where(b => b.ScanBaseHash == sysbody.ScanBaseHash).ToList();

                if (scans.Count != 0)
                {
                    foreach (BodyScan scan in scans)
                    {
                        if (scan.Equals(scanstruct))
                        {
                            if (notrack)
                            {
                                return scan;
                            }
                            else
                            {
                                return Set<BodyScan>().Find(scan.Id);
                            }
                        }
                    }

                    var diffprops = scans.Select(s => s.GetDifferingProps(scanstruct, new List<BodyScan.CompareResult>())).ToList();

                    bool istweak = diffprops.Count == 0;
                    List<EDDNJournalScan> headers = null;
                    List<DateTime> headerdates = null;
                    var curheader = scanstruct.Header;

                    for (int i = 0; i < diffprops.Count; i++)
                    {
                        var diff = diffprops[i];
                        var scan = scans[i];
                        var tweaks = diff.Where(p => !p.IsMinorDifference).ToList();

                        if (tweaks.Count != 0)
                        {
                            headers = GetEDDNScansByScanId(scan.Id).OrderBy(s => s.ScanTimestamp).ToList();
                            headerdates = headers.Select(h => h.ScanTimestamp).ToList();
                        }

                        if (diff.Count(p => !p.IsMinorDifference) <= 4 && diff.All(p => p.IsMinorDifference || p.IsTweakedProperty))
                        {
                            istweak = true;
                        }
                    }

                    for (int i = 0; i < diffprops.Count; i++)
                    {
                        foreach (var diff in diffprops[i])
                        {
                            Trace.WriteLine($"Scan {scanstruct.Header.GatewayTimestamp} differs from {scans[i].Id}: {diff.Property}: {diff.Left:G9} != {diff.Right:G9} ({diff.LSB} lsb, {diff.Delta} delta)");
                        }
                    }

                    if (!istweak)
                    {
                        //Debugger.Break();
                    }

                    /*
                    foreach (BodyScan scan in scans)
                    {
                        if (scan.Equals(scanstruct, 0.0000015F))
                        {
                            if (notrack)
                            {
                                return scan;
                            }
                            else
                            {
                                return Set<BodyScan>().Find(scan.Id);
                            }
                        }

                        Debugger.Break();
                    }
                     */
                }
            }

            BodyScan newscan = BodyScan.FromStruct(scanstruct, sysbody.Id == 0 ? sysbody : null, sysbody.Id);

            if (!noadd)
            {
                Set<BodyScan>().Add(newscan);
            }

            return newscan;
        }

        public EDDNJournalScan GetOrAddEDDNScan(XScanHeader jscanstruct, string jsonextra, int scanid, BodyScan scan, bool notrack = false, bool noadd = false)
        {
            foreach (var jscan in GetEDDNScansByScanId(jscanstruct.ParentId))
            {
                if (jscan.GatewayTimestamp == jscanstruct.GatewayTimestamp && jscan.DistanceFromArrivalLS == jscanstruct.DistanceFromArrivalLS)
                {
                    return jscan;
                }
            }

            EDDNJournalScan eddnscan = EDDNJournalScan.From(jscanstruct, scanid, scan, jsonextra);

            if (!noadd)
            {
                Set<EDDNJournalScan>().Add(eddnscan);
            }

            return eddnscan;
        }
    }
}
