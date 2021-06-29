using System;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using MouseCoordinatesListener.EmailService;
using MouseCoordinatesListener.Models;

namespace MouseCoordinatesListener.Controllers
{
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly RepositoryContext _context;
        private readonly ILogger<EventController> _logger;
        private readonly IEmailSender _emailSender;
        
        public EventController(RepositoryContext context, 
            ILogger<EventController> logger,
            IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }
        
        [Authorize]
        [HttpGet("/api/allevent")]
        public IActionResult GetAllEvents()
        {
            return Ok(_context.Events.ToList());
        }

        [AllowAnonymous]
        [HttpPost("/api/email")]
        public async Task<IActionResult> SendMessageToEmail([FromBody] User user)
        {
            var firstOrDefault = _context.Users.FirstOrDefault(x => x.Name == user.Name);
            var eventCount = _context.Events.Count();
            
            if (firstOrDefault != null)
            {
                var message = new Message(new[] {firstOrDefault.Email}, "ТензоТехСервис",
                        $"Кол-во записей: {eventCount}" +
                    $" Почта: {firstOrDefault.Email}" +
                    $" Скайп: {firstOrDefault.Skype}" +
                    $" Ватсап: {firstOrDefault.WhatsApp}" +
                    $" Контакт: {firstOrDefault.PhoneNumber}" +
                    $" Имя: {firstOrDefault.Name}" +
                    $" Возраст: {firstOrDefault.Age}"
                    );
                await _emailSender.SendEmailAsync(message);
                return Ok();
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("/api/start")]
        public IActionResult StartButtonLog()
        {
            _logger.LogInformation($"Запуск записи событий в БД");
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("/api/stop")]
        public IActionResult StopButtonLog()
        {
            _logger.LogInformation($"Остановка записи событий в БД");
            return Ok();
        }
        
        [AllowAnonymous]
        [HttpPost("/api/eventreg")]
        public async Task<IActionResult> EventRegister([FromBody] Event eventRegister)
        {
            try
            {
                DateTime date = DateTime.Now;
                
                _context.Events.Add(new Event
                {
                    EventType = eventRegister.EventType,
                    Coords = eventRegister.Coords,
                    CreatedOn = date
                });
            
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpDelete("/api/delete")]
        public IActionResult DeleteEvent()
        {
             _context.Database.ExecuteSqlRaw("TRUNCATE TABLE mousecoord.events;");
             return Ok(new
             {
                 response = "Записи успешно удалены"
             });
        }
    }
}