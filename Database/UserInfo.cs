using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("user_info")]
public class UserInfo
{
    [Key]
    public int UserID { get; set; }
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public string? Email { get; set; }
    public string? PersonalInfo { get; set; }
}