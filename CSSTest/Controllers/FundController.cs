using CSSTest.Data;
using CSSTest.DTO;
using CSSTest.Models;
using CSSTest.Models.Helper;
using Microsoft.AspNetCore.Mvc;

namespace CSSTest.Controllers
{
    public class FundController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataHelper _dataHelper;
        public FundController(ApplicationDbContext context, DataHelper dataHelper)
        {
            _context = context;
            _dataHelper = dataHelper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(FundDTO fundDTO)
        {
            if (ModelState.IsValid)
            {
                Fund fund = new Fund
                {
                    Id = fundDTO.Id,
                    Name = fundDTO.Name,
                    Description = fundDTO.Description,
                };

                _context.Funds.Add(fund);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View();

        }
    }
}
