using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
[Route("api/User")]
[ApiController]
public class UserController : ControllerBase
{
    public UserController(UserInfoDB db, GenerateJwt jwt)
    {
        userInfoDB = db;
        generateJwt = jwt;
    }
    private readonly UserInfoDB userInfoDB;
    private readonly GenerateJwt generateJwt;

    [HttpGet("Info")]
    [AllowAnonymous]
    public string GetInfo()
    {
        return "Hello YXReader";
    }

    [HttpPost("Register")]
    [AllowAnonymous]
    public IResult Register(UserInfo user)
    {
        user.UserID = userInfoDB.UserInfos.Count<UserInfo>() + 1;
        userInfoDB.Add<UserInfo>(user);
        userInfoDB.SaveChanges();
        return Results.Created("$/api/User/Register/{user.UserID}", user);
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public IResult Login(string userName, string password)
    {
        var query = from d in userInfoDB.UserInfos
                    where d.UserName == userName && d.Password == password
                    select d;
        if (query.Count<UserInfo>() != 0)
        {
            UserInfo user = query.First<UserInfo>();
            var result = generateJwt.GenerateEncodedTokenAsync(user);
            result.refresh_token = Guid.NewGuid().ToString();
            return Results.Ok<JwtTokenResult>(result);
        }
        else
        {
            return Results.NotFound("check userName and password");
        }
    }

    [HttpPost("ChangeInfo")]
    [Authorize]
    public IResult ChangeInfo(UserInfo user)
    {
        userInfoDB.Update<UserInfo>(user);
        userInfoDB.SaveChanges();
        return Results.Ok();
    }
}