﻿namespace PressCenters.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PressCenters.Common;
    using PressCenters.Data.Common.Repositories;
    using PressCenters.Data.Models;
    using PressCenters.Services.Messaging;
    using PressCenters.Web.ViewModels.Contacts;

    public class ContactsController : BaseController
    {
        private const string RedirectedFromContactForm = "RedirectedFromContactForm";

        private readonly IRepository<ContactFormEntry> contactsRepository;

        private readonly IEmailSender emailSender;

        public ContactsController(IRepository<ContactFormEntry> contactsRepository, IEmailSender emailSender)
        {
            this.contactsRepository = contactsRepository;
            this.emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContactFormViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            // TODO: Extract to IP provider (service)
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();
            var contactFormEntry = new ContactFormEntry
                                   {
                                       Name = model.Name,
                                       Email = model.Email,
                                       Title = model.Title,
                                       Content = model.Content,
                                       Ip = ip,
                                   };
            await this.contactsRepository.AddAsync(contactFormEntry);
            await this.contactsRepository.SaveChangesAsync();

            var email = this.emailSender.EmailBuilder()
                .AddFromAddress(model.Email)
                .AddFromName(model.Name)
                .AddToAddress(GlobalConstants.SystemEmail)
                .AddSubject(model.Title)
                .AddHtmlContent(model.Content)
                .BuildEmail();
            await this.emailSender.SendEmailAsync(email);

            this.TempData[RedirectedFromContactForm] = true;

            return this.RedirectToAction("ThankYou");
        }

        public IActionResult ThankYou()
        {
            if (this.TempData[RedirectedFromContactForm] == null)
            {
                return this.NotFound();
            }

            return this.View();
        }
    }
}
