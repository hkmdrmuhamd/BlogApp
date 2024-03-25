using System.Security.Claims;
using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Post");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.Users.FirstOrDefault(x => x.Email == loginViewModel.Email && x.Password == loginViewModel.Password);
                if (user != null)
                {
                    var userClaims = new List<Claim>(); //Kullanıcı bilgilerini tutmak için bir list oluşturduk.
                    userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())); //Kullanıcı id'sini tutmak için
                    userClaims.Add(new Claim(ClaimTypes.Name, user.UserName ?? "")); //Kullanıcı userName'ini tutmak için
                    userClaims.Add(new Claim(ClaimTypes.GivenName, user.Name ?? "")); //Adını tutmak için
                    userClaims.Add(new Claim(ClaimTypes.UserData, user.Image ?? "")); //Resmini tutmak için

                    if (user.Email == "info@mhk.com")
                    {
                        userClaims.Add(new Claim(ClaimTypes.Role, "admin")); //Kullanıcı admin ise admin rolünü ver
                    }

                    var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme); //Kullanıcı bilgilerini tutan listeyi kullanarak bir kimlik oluşturduk.

                    var authProperties = new AuthenticationProperties //Ekstra özellikler eklemek için
                    {
                        IsPersistent = true //Beni hatırla seçeneği (girilen kullanıcıyı sürekli olarak hatırlar)
                    };
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); //Önceki oturumu kapat
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults
                        .AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    ); //Yeni oturumu aç

                    return RedirectToAction("Index", "Post");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
                }
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
            }
            return View(loginViewModel);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.UserName == registerViewModel.UserName || x.Email == registerViewModel.Email);
                if (user == null)
                {
                    if (imageFile != null)
                    {
                        var extension = Path.GetExtension(imageFile.FileName).ToLower();
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("", "Sadece jpg, jpeg ve png uzantılı doslayaları seçebilirsiniz.");
                        }
                        else
                        {
                            var randomFileName = $"{Guid.NewGuid().ToString()}{extension}";
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(stream);
                            }
                            registerViewModel.Image = randomFileName;
                            var newUser = new User
                            {
                                Name = registerViewModel.Name,
                                UserName = registerViewModel.UserName,
                                Email = registerViewModel.Email,
                                Password = registerViewModel.Password,
                                Image = registerViewModel.Image
                            };
                            _userRepository.CreateUser(newUser);

                            return RedirectToAction("Login");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lütfen bir resim seçiniz.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya email adresi kullanılmaktadır.");
                }
            }
            return View(registerViewModel);
        }
    }
}