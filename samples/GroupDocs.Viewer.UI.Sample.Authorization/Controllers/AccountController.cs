using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace GroupDocs.Viewer.UI.Sample.Authorization.Controllers;

// NOTE: the demo accounts stand in for your identity provider, e.g. ASP.NET Core Identity or OpenID Connect
public class AccountController : Controller
{
    private static readonly Dictionary<string, string[]> DemoAccounts = new()
    {
        ["viewer"] = new[] { ViewerAccess.Role },
        ["guest"] = Array.Empty<string>(),
    };

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string? userName, string? returnUrl = null)
    {
        if (userName == null || !DemoAccounts.TryGetValue(userName, out var roles))
            return BadRequest("Unknown account.");

        var claims = new List<Claim> { new(ClaimTypes.Name, userName) };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        // NOTE: the return URL comes from the query string, redirect to local URLs only
        return Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();
}
