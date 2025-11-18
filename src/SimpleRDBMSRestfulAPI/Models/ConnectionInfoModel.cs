using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using hexasync.infrastructure.domain;
using hexasync.query_dsl;
using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Libs;

[Table("connection_infos")]
[EntityType("CONNECTION_INFO")]
[DapperMapping]
public class ConnectionInfoModel
{
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [Required]
    [Column("db_type")]
    public DbType DbType { get; set; }
    [Column("connection_string")]
    public string ConnectionString { get; set; } = null!;
    [Column("created_at")]
    [Required]
    public long CreatedAt { get; set; }

    [Column("updated_at")]
    [Required]
    public long UpdatedAt { get; set; }

    public string? GetConnectionString()
    {
        return Convert.FromBase64String(ConnectionString).DecryptAES();
    }
}