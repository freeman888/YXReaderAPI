using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
/// <summary>
/// jwt配置
/// </summary>
public class JwtConfig : IOptions<JwtConfig>
{
    public JwtConfig Value => this;
    public string? SecretKey { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public int Expired { get; set; }
    public DateTime NotBefore => DateTime.UtcNow;
    public DateTime IssuedAt => DateTime.UtcNow;
    public DateTime Expiration => IssuedAt.AddMinutes(Expired);
    private SecurityKey SigningKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
    public SigningCredentials SigningCredentials =>
        new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256);
}

public class GenerateJwt
{
    private readonly JwtConfig _jwtConfig;
    public GenerateJwt(IOptions<JwtConfig> jwtConfig)
    {
        _jwtConfig = jwtConfig.Value;
    }
    /// <summary>
    /// 生成token
    /// </summary>
    /// <param name="sub"></param>
    /// <param name="customClaims">携带的用户信息</param>
    /// <returns></returns>
    public JwtTokenResult GenerateEncodedTokenAsync(UserInfo userInfo)
    {
        //创建用户身份标识，可按需要添加更多信息
        var user_info = new List<Claim>{
                new Claim("userid", userInfo.UserID.ToString()),
                new Claim("username", userInfo.UserName),
            };
        //创建令牌
        var jwt = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: user_info,
            notBefore: _jwtConfig.NotBefore,
            expires: _jwtConfig.Expiration,
            signingCredentials: _jwtConfig.SigningCredentials);
        string access_token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return new JwtTokenResult()
        {
            access_token = access_token,
            expires_in = _jwtConfig.Expired * 60,
            token_type = JwtBearerDefaults.AuthenticationScheme,
            user = userInfo
        };
    }
}

public class JwtTokenResult
{
    public string access_token { get; set; } = "";
    public string refresh_token { get; set; } = "";
    /// <summary>
    /// 过期时间(单位秒)
    /// </summary>
    public int expires_in { get; set; }
    public string token_type { get; set; } = "";
    public UserInfo user { get; set; } = null!;
}


