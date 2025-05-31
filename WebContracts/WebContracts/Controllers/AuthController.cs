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

        // Construtor do controlador: injeta o contexto do banco de dados
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // =============================
        // EXIBE A TELA DE LOGIN (GET)
        // =============================
        // Quando o usuário acessa a URL /Auth/Login, essa ação exibe a tela de login.
        // Se a sessão tiver expirado, uma mensagem de aviso será exibida.
        public IActionResult Login()
        {
            if (Request.Query["expired"] == "true")
            {
                ViewBag.Message = "Sua sessão expirou. Por favor, faça login novamente.";
            }

            return View();
        }

        // ===============================
        // PROCESSA O LOGIN DO USUÁRIO (POST)
        // ===============================
        // Essa ação recebe os dados do formulário de login, valida e realiza a autenticação.
        // Se o e-mail e a senha estiverem corretos, o usuário é autenticado com cookies.
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Verifica se os campos foram preenchidos corretamente
            if (!ModelState.IsValid)
                return View(model);

            // Busca o usuário no banco de dados pelo e-mail informado
            var user = _context.Users.FirstOrDefault(u => u.UserEmail == model.Email);

            // Se o usuário não for encontrado, retorna erro
            if (user == null)
            {
                ModelState.AddModelError("", "Credenciais inválidas.");
                return View(model);
            }

            // Verifica se a senha fornecida confere com o hash salvo no banco
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.UserPasswordHash, model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Credenciais inválidas.");
                return View(model);
            }

            // Se as credenciais forem válidas, cria os dados de autenticação (claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            // Cria a identidade do usuário com base nos claims
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Faz o login do usuário usando cookies (sessão persistente)
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Exibe mensagem de sucesso após o login
            TempData["Success"] = "Login realizado com sucesso!";
            return RedirectToAction("Index", "Home");
        }

        // ========================
        // ENCERRA A SESSÃO (LOGOUT)
        // ========================
        // Quando o usuário clica em "Sair", essa ação encerra a sessão e redireciona para o login.
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Você saiu do sistema com sucesso.";
            return RedirectToAction("Login");
        }

        // =============================
        // EXIBE A TELA DE CADASTRO (GET)
        // =============================
        // Quando o usuário acessa /Auth/Register, essa ação exibe o formulário de criação de conta.
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ========================================
        // PROCESSA O CADASTRO DE NOVO USUÁRIO (POST)
        // ========================================
        // Essa ação valida os dados enviados no formulário, verifica se o e-mail e o nome de usuário já existem,
        // cria um novo usuário e salva no banco de dados com a senha protegida por hash.
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            // Verifica se todos os campos foram preenchidos corretamente
            if (!ModelState.IsValid)
                return View(request);

            // Verifica se o e-mail já está cadastrado
            if (await _context.Users.AnyAsync(u => u.UserEmail == request.UserEmail))
            {
                ModelState.AddModelError("UserEmail", "Este e-mail já está em uso.");
                return View(request);
            }

            // Verifica se o nome de usuário já está em uso
            if (await _context.Users.AnyAsync(u => u.UserName == request.UserName))
            {
                ModelState.AddModelError("UserName", "Este nome de usuário já está em uso.");
                return View(request);
            }

            // Cria um novo usuário com a senha criptografada (hash)
            var hasher = new PasswordHasher<User>();

            var user = new User
            {
                UserName = request.UserName,
                UserEmail = request.UserEmail,
                UserPasswordHash = hasher.HashPassword(null, request.Password)
            };

            // Salva o novo usuário no banco
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Mensagem de sucesso após criação da conta
            TempData["Success"] = "Conta criada com sucesso! Agora você pode fazer login.";
            return RedirectToAction("Login");
        }
    }
}
