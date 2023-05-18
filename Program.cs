var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.Urls.Add("http://*:10001");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapGet("/", () => "怡心阅读WebAPI");
app.MapGet("/test", () => "Test API 5.19");
app.Run();
