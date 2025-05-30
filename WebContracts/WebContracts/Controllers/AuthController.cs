using Microsoft.AspNetCore.Mvc;
using WebContracts.Models;
using WebContracts.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WebContracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace WebContracts.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        // Construtor que injeta o contexto do banco de dados
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // Exibe a tela de login (GET /Auth/Login)
        public IActionResult Login()
        {
            return View();
        }

        // Processa o login do usuário (POST /Auth/Login)
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Se os dados enviados não são válidos (ex: campos vazios), volta para a tela com os erros
            if (!ModelState.IsValid)
                return View(model);

            // Busca o usuário no banco com base no e-mail informado
            var user = _context.Users.FirstOrDefault(u => u.UserEmail == model.Email);

            // Se não encontrar o usuário, retorna erro genérico
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View(model);
            }

            // Verifica se a senha informada confere com o hash salvo no banco
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.UserPasswordHash, model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View(model);
            }

            // Se chegou aqui, as credenciais estão corretas
            // Define os "claims" que serão usados na autenticação (nome, ID etc.)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            // Cria uma identidade e um principal com esses claims
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Realiza o login via cookies (autenticação persistente no navegador)
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redireciona para a página inicial após o login
            return RedirectToAction("Index", "Home");
        }

        // Encerra a sessão do usuário (GET /Auth/Logout)
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Exibe o formulário de registro de novo usuário
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Processa o registro do novo usuário (POST /Auth/Register)
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            // Verifica se os dados estão válidos
            if (!ModelState.IsValid)
                return View(request);

            // Valida se o e-mail já está sendo usado por outro usuário
            if (await _context.Users.AnyAsync(u => u.UserEmail == request.UserEmail))
            {
                ModelState.AddModelError("UserEmail", "Email already in use.");
                return View(request);
            }

            // Valida se o nome de usuário já está sendo usado
            if (await _context.Users.AnyAsync(u => u.UserName == request.UserName))
            {
                ModelState.AddModelError("UserName", "Username already in use.");
                return View(request);
            }

            // Cria o hash da senha para não armazená-la em texto claro
            var hasher = new PasswordHasher<User>();

            // Cria o novo objeto de usuário com os dados informados
            var user = new User
            {
                UserName = request.UserName,
                UserEmail = request.UserEmail,
                UserPasswordHash = hasher.HashPassword(null, request.Password)
            };

            // Salva o novo usuário no banco
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Direciona para a tela de login após o cadastro
            return RedirectToAction("Login");
        }
    }
}
