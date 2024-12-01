using Exo.WebApi.Contexts;
using Exo.WebApi.Repositories;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ExoContext, ExoContext>();
builder.Services.AddControllers();

//Forma de autenticação
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
//parametros de validacao do token.
.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //valida quem esta solicitando
        ValidateIssuer = true,
        //valida quem esta recebendo
        ValidateAudience = true,
        //define se o tempo de expiracao será validado
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("exoapi-chave-autenticado")),
        //valida o tempo de expiracao do token.
        ClockSkew = TimeSpan.FromSeconds(30),
        //nome do issuer, da origem.
        ValidIssuer = "exoapi.webapi",
        // nome do audience, para o destino.
        ValidAudience = "exoapi.webapi",

    };
});

builder.Services.AddTransient<ProjetoRepository, ProjetoRepository>();
builder.Services.AddTransient<UsuarioRepository, UsuarioRepository>();

var app = builder.Build();

app.UseRouting();
//habilita a autenticacao
app.UseAuthentication();

//habilita a autorizacai
app.UseAuthentication();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
