using Exo.WebApi.Models;
using Exo.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Exo.WebApi.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioRepository _usuariosRepository;
    public UsuariosController(UsuarioRepository usuarioRepository)
    {
        _usuariosRepository = usuarioRepository;
    }
    //get -> /api/usuarios
    [HttpGet]
    public IActionResult Listar()
    {
        return Ok(_usuariosRepository.Listar());
    }

    //post -> /api/usuarios
    // [HttpPost]
    // public IActionResult Cadastrar(Usuario usuario)
    // {
    //     _usuariosRepository.Cadastrar(usuario);
    //     return StatusCode(201);
    //}

    // Novo código POST para auxiliar o método de login.
    public IActionResult Post(Usuario usuario)
    {
        Usuario usuarioBuscado = _usuariosRepository.Login(usuario.Email, usuario.Senha);
        if (usuarioBuscado == null)
        {
            return NotFound("E-mail ou senha inválidos!");
        }
        //Se o usuário for encontrado, segue a criação do Token.
        //Define os dados que serão fornecidos no token - Payload.
        var claims = new[]
        {
            //Armazena na claim o e-mail do usuário autenticado.
            new Claim(JwtRegisteredClaimNames.Email, usuarioBuscado.Email),
            //Armazena na claim o id do usuário autenticado.
            new Claim(JwtRegisteredClaimNames.Jti, usuarioBuscado.Id.ToString()),
        };
        //define a chave de acesso ao token.
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("exoapi-chave-autenticado"));

        //Define as crendicias do token.
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //Gera o token.
        var token = new JwtSecurityToken(
            issuer: "exoapi.webapi", //emissor do token
            audience: "exoapi.webapi", //Destinatário do token.
            claims: claims, //dados definidos acima
            expires: DateTime.Now.AddMinutes(30), //Tempo de expiração.
            signingCredentials: creds //credenciais do token
        );
        //Retorna ok com o token.
        return Ok(
            new { token = new JwtSecurityTokenHandler().WriteToken(token) }
        );
    }
    // Fim do novo código POST para auxiliar o método de Login.


    // get -> /api/usuarios/{id}
    [HttpGet("{id}")] // faz a busca pelo ID.
    public IActionResult BuscaPorId(int id)
    {
        Usuario usuario = _usuariosRepository.BuscaPorId(id);
        if (usuario == null)
        {
            return NotFound();
        }
        return Ok(usuario);
    }
    //put -> /api/usuarios/{id}
    //atualiza.
    [Authorize]
    [HttpPut("{id}")]
    public IActionResult Atualizar(int id, Usuario usuario)
    {
        _usuariosRepository.Atualizar(id, usuario);
        return StatusCode(204);
    }

    //delete -> /api/usuarios/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Deletar(int id)
    {
        try
        {
            _usuariosRepository.Deletar(id);
            return StatusCode(204);
        }
        catch (Exception e)
        {
            return BadRequest();
        }
    }
}
