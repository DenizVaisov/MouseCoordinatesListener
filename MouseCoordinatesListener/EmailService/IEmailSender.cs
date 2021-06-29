﻿using System.Threading.Tasks;

namespace MouseCoordinatesListener.EmailService
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
        Task SendEmailAsync(Message message);
    }
}