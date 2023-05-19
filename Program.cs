using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//注入jwt
builder.Services.AddScoped<GenerateJwt>();
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

#region jwt验证
var jwtConfig = new JwtConfig();
builder.Configuration.Bind("JwtConfig", jwtConfig);
builder.Services
    .AddAuthentication(option =>
    {
        //认证middleware配置
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //Token颁发机构
            ValidIssuer = jwtConfig.Issuer,
            //颁发给谁
            ValidAudience = jwtConfig.Audience,
            //这里的key要进行加密
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey!)),
            //是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
            ValidateLifetime = true,
        };
    });
#endregion






string connString = builder.Configuration["connString"]!;
builder.Services.AddDbContext<UserInfoDB>(opt => opt.UseMySql(connString, ServerVersion.AutoDetect(connString)));

var app = builder.Build();
app.Urls.Add("http://*:10001");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();

app.MapGet("/api", () => "怡心阅读 后端接口 WebAPI");
app.MapPost("/api/userRegister", (UserInfo user, UserInfoDB db) =>
{
    user.UserID = db.UserInfos.Count<UserInfo>() + 1;
    db.Add<UserInfo>(user);
    db.SaveChanges();
    return Results.Created("$/api/userRegister/{user.UserID}", user);
});
app.MapPost("/api/userLogin", (string userName, string password, UserInfoDB db, GenerateJwt generateJwt) =>
{
    var query = from d in db.UserInfos
                where d.UserName == userName && d.Password == password
                select d;
    if (query.Count<UserInfo>() != 0)
    {
        var refreshToken = Guid.NewGuid().ToString();
        var jwtTokenResult = generateJwt.GenerateEncodedTokenAsync(query.First<UserInfo>());
        jwtTokenResult.refresh_token = refreshToken;
        return Results.Ok<JwtTokenResult>(jwtTokenResult);
    }
    else
    {
        return Results.NotFound("check userName and password");
    }
});
app.MapPost("/api/userChangeInfo", (UserInfo user, UserInfoDB db) =>
{
    db.Update<UserInfo>(user);
    db.SaveChanges();

});
app.Run();
