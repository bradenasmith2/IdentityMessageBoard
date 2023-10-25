using IdentityMessageBoard.DataAccess;
using IdentityMessageBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace IdentityMessageBoard.Controllers
{
    public class MessagesController : Controller
    {
        private readonly MessageBoardContext _context;
        private readonly IEnumerable<ApplicationUser> Users;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessagesController(MessageBoardContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            Users = new List<ApplicationUser>(); Users = _context.Users;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var user = _context.ApplicationUsers.Find(_userManager.GetUserId(User));
            ViewData["CurrentUser"] = user;
            var messages = _context.Messages
                .Where(m => m.Author == user)
                .Include(m => m.Author)
                .OrderBy(m => m.ExpirationDate)
                .ToList()
                .Where(m => m.IsActive()); // LINQ Where(), not EF Where()

            

            return View(messages);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AllMessages()
        {
            var allMessages = new Dictionary<string, List<Message>>()
            {
                { "active" , new List<Message>() },
                { "expired", new List<Message>() }
            };

            foreach (var message in _context.Messages)
            {
                if (message.IsActive())
                {
                    allMessages["active"].Add(message);
                }
                else
                {
                    allMessages["expired"].Add(message);
                }
            }


            return View(allMessages);
        }

        [Authorize(Roles = "SuperUser,Admin")]
        [Route("/user/{userId}/messages/{msgId}/edit")]
        public IActionResult Edit(string msgId, string userId)//null validation needed
        {
            var msg = _context.Messages.Find(int.Parse(msgId));
            return View(msg);
        }

        [Authorize(Roles = "SuperUser,Admin")]
        [Route("/user/{userId}/messages/{msgId}/edit/submit")]//null validation needed
        public IActionResult Update(string msgId, string userId, Message msg)
        {
            var user = _context.Users.Find(userId);

            msg.Id = int.Parse(msgId);
            var oldMsg = _context.Messages.Find(int.Parse(msgId));

            oldMsg.Author = msg.Author;
            oldMsg.ExpirationDate = msg.ExpirationDate;
            oldMsg.Content = msg.Content;

            _context.Messages.Update(oldMsg);
            _context.SaveChanges();

            return Redirect($"/users/{user.Id}/messages");
        }

        [Authorize(Roles = "SuperUser,Admin")]
        [Route("/user/{userId}/messages/{msgId}/delete")]//null validation needed
        public IActionResult Delete(string msgId, string userId)
        {
            var user = _context.Users.Find(userId);
            var msg = _context.Messages.Find(msgId);
            _context.Remove(msg);
            _context.SaveChanges();

            return Redirect($"/users/{user.Id}");
        }

        [Authorize]
        public IActionResult New()
        {

            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(string userId, string content, int expiresIn)
        {
            var user = _context.ApplicationUsers.Find(userId);

            if(!ModelState.IsValid)
            {
                return NotFound();
            }

            _context.Messages.Add(
                new Message()
                {
                    Content = content,
                    ExpirationDate = DateTime.UtcNow.AddDays(expiresIn),
                    Author = user
                });

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
