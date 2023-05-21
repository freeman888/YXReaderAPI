using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            }
                        },
                    new string[] { }
                    }
                });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT授权(数据将在请求头中进行传输) 在下方输入Bearer {token} 即可，注意两者之间有空格",
        Name = "Authorization",//jwt默认的参数名称
        In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer",

    });
});

builder.Services.AddScoped<GenerateJwt>();
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
var jwt = new JwtConfig();
builder.Configuration.Bind("JwtConfig", jwt);
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //Token颁发机构
        ValidIssuer = jwt.Issuer,
        //颁发给谁
        ValidAudience = jwt.Audience,
        //这里的key要进行加密
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
        //是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
        ValidateLifetime = true,
    };
});

string connString = builder.Configuration["connString"]!;
builder.Services.AddDbContext<UserInfoDB>(opt => opt.UseMySql(connString, ServerVersion.AutoDetect(connString)));

var app = builder.Build();
app.Urls.Add("http://*:10001");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
