using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlogApp.Data.Abstract;
using BlogApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

                    if (user.Email == "info@mkh.com")
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
    }
}