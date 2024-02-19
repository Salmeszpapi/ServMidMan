using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using ServMidMan.Data;
using ServMidMan.Helper;
using ServMidMan.Models;

namespace ServMidMan.Controllers
{
	public class ChatController : Controller
	{
        private ILogger<HomeController> _logger;
        private DataProviderContext _dataProvider;

        public ChatController(ILogger<HomeController> logger, DataProviderContext dataProviderContext)
        {
            _logger = logger;
            _dataProvider = dataProviderContext;
        }
        public IActionResult Index(int? id)
		{
            if (!SiteGuardian.CheckSession(HttpContext))
            {
                return RedirectToAction("Welcome", "Authentication");
            }
            ViewData["typeOfUser"] = HttpContext.Session.GetString("UserType");
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            ViewData["MyId"] = userId;
            ViewData["NoExistingMessages"] = true;
            ChatWithPerson chatWithPerson = new ChatWithPerson();

            var usersWithMessages = _dataProvider.Users
    .Where(user => _dataProvider.ChatHistory.Any(chat =>
        (chat.SenderId == user.Id && chat.ReceiverID == userId) ||
        (chat.SenderId == userId && chat.ReceiverID == user.Id)))
    .ToList();
            chatWithPerson.AllUsers = usersWithMessages;
            if(id != null)
            {
                chatWithPerson.AllUsers.Insert(0, _dataProvider.Users.Where(x => x.Id == id).FirstOrDefault());
                chatWithPerson.Messages = _dataProvider.ChatHistory
                                                        .Where(c => c.SenderId == userId && c.ReceiverID == id)
                                                        .ToList();
                chatWithPerson.Partner = _dataProvider.Users.Where(x=>x.Id == id).FirstOrDefault();
            }
            else
            {
                chatWithPerson.Messages = _dataProvider.ChatHistory
                            .Where(c => c.SenderId == userId || c.ReceiverID == userId)
                            .ToList();
            }

            if (chatWithPerson.Messages.Count == 0)
            {
                return View(chatWithPerson);
            }
            ViewData["NoExistingMessages"] = false;

            if(id == null)
            {
                id = chatWithPerson.AllUsers[0].Id;
            }

            var partner = _dataProvider.Users.Where(c => c.Id == id).FirstOrDefault();
            chatWithPerson.Partner = partner;
           ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            return View(chatWithPerson);
		}
        [HttpPost]
        public ActionResult SendMessage(string message, string receiverName)
        {
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            // Process the message
            User receiverUser = _dataProvider.Users.Where(x=>x.Name == receiverName).FirstOrDefault();
            Chat chat = new Chat()
            {
                Massege = message,
                ReceiverID = receiverUser.Id,
                SenderId = userId,
                SendTime = DateTime.Now,
            };
            _dataProvider.Add(chat);
            _dataProvider.SaveChanges();
            // Construct the response data
            var responseData = new
            {
                time = DateTime.Now.ToString("h:mm tt"), // Format the time as needed
                message = message // Use the received message in the response
            };

            return Json(responseData);
        }
        public IActionResult GetConversation(string partnerName)
        {
            ViewData["typeOfUser"] = HttpContext.Session.GetString("UserType");
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var partner = _dataProvider.Users.FirstOrDefault(u => u.Name == partnerName);

            if (partner == null)
            {
                return BadRequest("Partner not found.");
            }
            var conversation = _dataProvider.ChatHistory
                .Where(c => (c.SenderId == userId && c.ReceiverID == partner.Id) ||
                            (c.SenderId == partner.Id && c.ReceiverID == userId))
                .OrderBy(c => c.SendTime)
                .Select(c => new
                {
                    sendTime = c.SendTime.ToString("h:mm tt"),
                    message = c.Massege,
                    sender = c.SenderId
                })
                .ToList();
            if(conversation != null)
            {
                ViewData["NoExistingMessages"] = false;
            }
            return Json(conversation);
        }

        public IActionResult Test()
        {
            return View();
        }
	}
}
