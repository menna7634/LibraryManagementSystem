using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class ContactFormController : Controller
    {
        private readonly IContactFormRepository _contactFormRepository;

        public ContactFormController(IContactFormRepository contactFormRepository)
        {
            _contactFormRepository = contactFormRepository;
        }

        public async Task<IActionResult> GetContacts(int pageNumber = 1, int pageSize = 10, string searchEmail="", bool orderByNewest = true  , DateTime? searchDate=default)
        {
            var result = await _contactFormRepository.GetContactFormsAsync(pageNumber, pageSize, searchEmail, orderByNewest, searchDate);
          
            ViewData["searchEmail"] = searchEmail;
            ViewData["searchDate"] = searchDate;
            ViewData["orderByNewest"] = orderByNewest;
            ViewData["pageNumber"] = pageNumber;
            ViewData["pageSize"] = pageSize;
          

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _contactFormRepository.DeleteContactFormAsync(id);

            return RedirectToAction("GetContacts");
        }

    }
}
