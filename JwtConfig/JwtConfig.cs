using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// jwt配置
/// </summary>
public class JwtConfig : IOptions<JwtConfig>
{
    public JwtConfig Value => this;
    public string SecretKey { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int Expired { get; set; }
    public DateTime NotBefore => DateTime.UtcNow;
    public DateTime IssuedAt => DateTime.UtcNow;
    public DateTime Expiration => IssuedAt.AddMinutes(Expired);
    private SecurityKey SigningKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
    public SigningCredentials SigningCredentials =>
        new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256);
}

