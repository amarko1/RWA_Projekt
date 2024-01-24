using AutoMapper;
using DATA_LAYER.BLModels;
using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_LAYER.Models;
using System.Security.Claims;

namespace MVC_LAYER.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepo _userRepo;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(ILogger<UserController> logger, IUserRepo userRepo,ICountryRepository countryRepository, IMapper mapper , IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _userRepo = userRepo;
            _mapper = mapper;
            _countryRepository = countryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            var blUsers = _userRepo.GetAll();
            var vmUsers = _mapper.Map<IEnumerable<VMUser>>(blUsers);

            return View(vmUsers);
        }

        private void SetCountriesToView()
        {
            var countries = _countryRepository.GetAll();

            var selectListItems = countries.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).OrderBy(p => p.Text);

            ViewBag.CountrySelectList = new SelectList(selectListItems, "Value", "Text");
        }

        public IActionResult Register()
        {
            SetCountriesToView();

            return View();
        }

        [HttpPost]
        public IActionResult Register(VMRegister register)
        {
            if (!ModelState.IsValid)
                return View(register);

            var user = _userRepo.CreateUser(
                register.Username,
                register.FirstName,
                register.LastName,
                register.Email,
                register.Password,
                register.CountryOfResidenceId);

            return RedirectToAction("Login");
        }

        public IActionResult ValidateEmail(VMValidateEmail validateEmail)
        {
            if (!ModelState.IsValid)
                return View(validateEmail);

            // Confirm email, skip BL for simplicity
            _userRepo.ConfirmEmail(
                validateEmail.Email,
                validateEmail.SecurityToken);

            return RedirectToAction("Index");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(VMLogin login)
        {
            if (!ModelState.IsValid)
                return View(login);

            var user = _userRepo.GetConfirmedUser(
                login.Username,
                login.Password);

            if (user == null)
            {
                ModelState.AddModelError("Username", "Invalid username or password");
                return View(login);
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Email) };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties()).Wait();

            return RedirectToAction("CardView", "Video");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

            return RedirectToAction("CardView", "Video");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(VMChangePassword changePassword)
        {
            // Change user password, skip BL for simplicity
            _userRepo.ChangePassword(
                changePassword.Username,
                changePassword.NewPassword);

            return RedirectToAction("CardView", "Video");
        }

        //[HttpGet]
        //public IActionResult UserDetails()
        //{
        //    // Retrieve username from claims
        //    string username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

        //    if (username == null)
        //    {
        //        // Handle user not found
        //        return NotFound();
        //    }

        //    return View(username);
        //}

        [HttpGet]
        public IActionResult UserDetails()
        {
            // Retrieve username from claims
            string username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            if (username == null)
            {
                // Handle user not found
                return NotFound();
            }

            // No need to call _userRepo.Get(username) here
            var user = new BLUser { Username = username }; // You can create a BLUser object with just the username

            return View(user);
        }

    }
}
