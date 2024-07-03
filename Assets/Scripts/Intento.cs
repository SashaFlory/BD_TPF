using Postgrest.Models;
using Postgrest.Attributes;

public class intento : BaseModel
{
    [Column("id"), PrimaryKey]
    public int id { get; set; }

    [Column("username")]
    public int username { get; set; }

    [Column("category")]
    public int category { get; set; }

    [Column("score")]
    public int score { get; set; }
}