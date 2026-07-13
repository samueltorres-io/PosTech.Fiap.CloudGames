using PosTech.Fiap.CloudGames.Application.Auth;
using PosTech.Fiap.CloudGames.Application.Auth.Dtos;
using PosTech.Fiap.CloudGames.Application.Users;
using PosTech.Fiap.CloudGames.Application.Users.Dtos;
using PosTech.Fiap.CloudGames.Domain.Exceptions;
using PosTech.Fiap.CloudGames.Infrastructure.Persistence;
using PosTech.Fiap.CloudGames.Infrastructure.Persistence.Repositories;
using PosTech.Fiap.CloudGames.Infrastructure.Security;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Reqnroll;
using Xunit;

namespace PosTech.Fiap.CloudGames.Bdd.Tests.Steps;

[Binding]
public sealed class AutenticacaoSteps : IDisposable
{
    private readonly CloudGamesDbContext _context;
    private readonly UserCommandService _userCommand;
    private readonly AuthService _authService;

    private UserResponse? _registered;
    private AuthResponse? _authResponse;
    private Exception? _caughtException;

    public AutenticacaoSteps()
    {
        var options = new DbContextOptionsBuilder<CloudGamesDbContext>()
            .UseInMemoryDatabase($"fcg-bdd-{Guid.NewGuid()}")
            .Options;
        _context = new CloudGamesDbContext(options);

        var users = new UserRepository(_context);
        var hasher = new BcryptPasswordHasher();
        var jwt = new JwtTokenGenerator(Options.Create(new JwtSettings
        {
            Issuer = "fcg-tests",
            Audience = "fcg-tests",
            SecretKey = "chave-de-teste-bdd-com-mais-de-32-bytes-1234567890",
            ExpirationMinutes = 60
        }));

        _userCommand = new UserCommandService(users, hasher, _context);
        _authService = new AuthService(users, hasher, jwt);
    }

    [Given(@"que não existe usuário cadastrado com o e-mail ""(.*)""")]
    public async Task DadoQueNaoExisteUsuario(string email)
    {
        (await _context.Users.AnyAsync(u => u.Email == PosTech.Fiap.CloudGames.Domain.ValueObjects.Email.Create(email)))
            .Should().BeFalse();
    }

    [Given(@"que existe um usuário ""(.*)"" com e-mail ""(.*)"" e senha ""(.*)""")]
    public async Task DadoQueExisteUsuario(string nome, string email, string senha)
    {
        _registered = await _userCommand.RegisterAsync(new RegisterUserRequest(nome, email, senha));
    }

    [When(@"eu cadastro o usuário ""(.*)"" com e-mail ""(.*)"" e senha ""(.*)""")]
    public async Task QuandoEuCadastro(string nome, string email, string senha)
    {
        _registered = await _userCommand.RegisterAsync(new RegisterUserRequest(nome, email, senha));
    }

    [When(@"eu tento cadastrar o usuário ""(.*)"" com e-mail ""(.*)"" e senha ""(.*)""")]
    public async Task QuandoEuTentoCadastrar(string nome, string email, string senha)
    {
        _caughtException = await Record.ExceptionAsync(() =>
            _userCommand.RegisterAsync(new RegisterUserRequest(nome, email, senha)));
    }

    [When(@"eu tento autenticar com e-mail ""(.*)"" e senha ""(.*)""")]
    public async Task QuandoEuTentoAutenticar(string email, string senha)
    {
        _caughtException = await Record.ExceptionAsync(async () =>
            _authResponse = await _authService.LoginAsync(new LoginRequest(email, senha)));
    }

    [Then(@"o cadastro é realizado com sucesso")]
    public void EntaoCadastroComSucesso()
    {
        _registered.Should().NotBeNull();
        _registered!.Active.Should().BeTrue();
    }

    [Then(@"o login com e-mail ""(.*)"" e senha ""(.*)"" retorna um token válido")]
    public async Task EntaoLoginRetornaToken(string email, string senha)
    {
        var response = await _authService.LoginAsync(new LoginRequest(email, senha));
        response.AccessToken.Should().NotBeNullOrWhiteSpace();
        response.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow);
    }

    [Then(@"o cadastro é rejeitado por senha inválida")]
    public void EntaoCadastroRejeitadoPorSenha()
    {
        _caughtException.Should().BeOfType<DomainException>();
    }

    [Then(@"a autenticação é negada")]
    public void EntaoAutenticacaoNegada()
    {
        _caughtException.Should().NotBeNull();
        _authResponse.Should().BeNull();
    }

    public void Dispose() => _context.Dispose();
}
