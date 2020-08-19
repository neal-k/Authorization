using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CustomPolicyProvider.CustomAuthorizationPolicyProvider;

namespace Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        [Authorize(Policy = "Claim.DoB")]
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }

        [SecurityLevel(5)]
        public IActionResult SecretLevel()
        {
            return View("Secret");
        }

        [SecurityLevel(10)]
        public IActionResult SecretHigherLevel()
        {
            return View("Secret");
        }

        // Accessible by anyone
        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            // Grandma Makes claims about you
            var grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob"),
                new Claim(ClaimTypes.Email, "Bob@fmail.com"),
                new Claim(ClaimTypes.DateOfBirth, "11/11/2000"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(DynamicPolicies.SecurityLevel, "7"),
                new Claim("Grandma.Says", "Very nice boi.")
            };
            // Driver License claims
            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob K Foo"),
                new Claim("DrivingLicense", "A+")
            };
            // Because we trust grandma we will create an identity based on her claims
            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
            // Because we trust the government we will create identity based on it's claims
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Grandma Identity");
            // grandma is one of many that can be used for identification. Enter other identification as part of array here
            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity});

            // Create a context based on the principal which contains all the identities
            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DoStuff()
        {
            // we are doing stuff here 

            var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("Hello").Build();

            var authResult = await _authorizationService.AuthorizeAsync(User, customPolicy);

            if(authResult.Succeeded)
            {
                
            }

            return View("Index");
        }
    }
}