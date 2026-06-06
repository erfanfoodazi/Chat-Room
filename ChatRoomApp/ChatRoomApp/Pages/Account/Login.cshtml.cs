using Application.Users.UseCases.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChatRoomApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IMediator _mediator;

        public LoginModel(SignInManager<User> signInManager, IMediator mediator)
        {
            _signInManager = signInManager;
            _mediator = mediator;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var User = await _mediator.Send(new GetUserByEmailQuery { Email = Email });
            if (User == null)
                return Page();

            var Username = User.UserName;

            var result = await _signInManager.PasswordSignInAsync(
                Username,
                Password,
                isPersistent: true,
                lockoutOnFailure: true);

            if (result.Succeeded)
                return Redirect("/chat");

            if (result.IsLockedOut)
                ErrorMessage = "Account is locked. Try again later.";
            else
                ErrorMessage = "Invalid username or password.";

            return Page();
        }
    }
}