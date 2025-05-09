using System.Data.Entity;

namespace UPFCON.Models.Context;

public class UpfconContext : DbContext
{
    public required DbSet<BoardDirector> BoardMembers { get; set; }
}