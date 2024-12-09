using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using OdevDagitim.Models;
using OdevDagitim.Repositories;
using OdevDagitim.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OdevDagitim.Controllers
{
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly ClassRepository _classRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly INotyfService _notyf;

        public UserController(UserRepository userRepository, ClassRepository classRepository, IMapper mapper, IConfiguration config, INotyfService notyf)
        {
            _userRepository = userRepository;
            _classRepository = classRepository;
            _mapper = mapper;
            _config = config;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserModel model)
        {
           
                if (_userRepository.Where(s => s.Username == model.Username).Count() > 0)
                {
                    _notyf.Error("Girilen Kullanıcı Adı Kayıtlıdır!");
                    return View(model);
                }
                if (_userRepository.Where(s => s.Email == model.Email).Count() > 0)
                {
                    _notyf.Error("Girilen E-Posta Adresi Kayıtlıdır!");
                    return View(model);
                }
                var user = new User();
                user.Username = model.Username;
                user.Email = model.Email;
                user.PasswordHash =MD5Hash(model.PasswordHash);
                user.Role = "Ogrenci";
                
                user.Created = DateTime.Now;
                user.Updated = DateTime.Now;
                await _userRepository.AddAsync(user);
                _notyf.Success("Kullanıcı başarıyla kaydedildi.");
                return RedirectToAction("Login","User");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = _userRepository.Where(s => s.Username == model.UserName && s.PasswordHash == MD5Hash(model.Password)).FirstOrDefault();

            if (user == null)
            {
                _notyf.Error("Kullanıcı adı veya şifre hatalı!");
                return View(model);
            }

            // Kullanıcıyı doğruladıktan sonra Claims oluşturuyoruz
            var claims = new List<Claim>
                 {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
             };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Kullanıcıyı cookie ile kimlik doğrulama işlemi yapıyoruz
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddDays(3), // Cookie'nin geçerlilik süresi
                IsPersistent = true, // Eğer cookie'nin kalıcı olmasını istiyorsanız true yapın
                AllowRefresh = true // Yeniden giriş yapıldığında cookie'nin yenilenmesini sağlar
            });

            if (user.Role == "admin")
            {
                return RedirectToAction("Index","Admin");
            }

            _notyf.Success("Başarıyla giriş yapıldı.");
            return RedirectToAction("Index", "Home");
        }

        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _notyf.Success("Başarıyla çıkış yapıldı.");
            return RedirectToAction("Login", "User");
        }

        public string MD5Hash(string pass)
        {
            var salt = _config.GetValue<string>("AppSettings:MD5Salt");
            var password = pass + salt;
            var hashed = password.MD5();
            return hashed;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UserList()
        {
            var users = await _userRepository.GetUsersWithClassAsync();
            var viewModel = _mapper.Map<List<UserListViewModel>>(users);
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _notyf.Error("Kullanıcı bulunamadı.");
                return RedirectToAction(nameof(UserList));
            }

            var viewModel = _mapper.Map<UserListViewModel>(user);
            var classes = await _classRepository.GetAllAsync();
            ViewBag.Classes = new SelectList(classes, "Id", "ClassName");
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserListViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetByIdAsync(model.Id);
                if (user == null)
                {
                    _notyf.Error("Kullanıcı bulunamadı.");
                    return RedirectToAction(nameof(UserList));
                }

              
                user.Role = model.Role;
                user.ClassId = model.ClassId;
                user.Updated = DateTime.Now;

                await _userRepository.UpdateAsync(user);
                _notyf.Success("Kullanıcı başarıyla güncellendi.");
                return RedirectToAction(nameof(UserList));
            }

            var classes = await _classRepository.GetAllAsync();
            ViewBag.Classes = new SelectList(classes, "Id", "ClassName");
            return View(model);
        }
    }
}
