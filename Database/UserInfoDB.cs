using Microsoft.EntityFrameworkCore;
public class UserInfoDB : DbContext
{
    public UserInfoDB(DbContextOptions<UserInfoDB> options) : base(options) { }
    public DbSet<UserInfo> UserInfos { get; set; } = null!;
}