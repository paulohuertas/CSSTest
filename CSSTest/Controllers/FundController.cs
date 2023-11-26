using CSSTest.Data;
using CSSTest.Models;
using CSSTest.Models.Helper;
using Microsoft.AspNetCore.Mvc;
using CSSTest.DTO;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CSSTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FundController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public FundController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("{funds}")]
        [Route("Fund/CreateFunds")]
        public IActionResult CreateFunds()
        {
            if(_configuration != null)
            {
                IConfiguration configuration = _configuration;
                ApplicationDbContext context = _context;

                DataHelper dataHelper = new DataHelper(configuration, context);
                var funds = dataHelper.CreateFunds(1, 5);

                foreach (Fund fund in funds)
                {
                    var found = _context.Funds.FirstOrDefault(f => f.Name == fund.Name);
                    if (found != null)
                    {
                        _context.Funds.Add(fund);
                        _context.SaveChanges();
                    }
                }
                return Ok(funds);
            }

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult EditFund(int id, [FromBody] UpdateFundDTO editFund)
        {
            Fund? fund = _context.Funds.Where(f => f.Id == id).FirstOrDefault();

            if (fund == null) return NotFound();

            if (fund != null)
            {
                fund.Name = editFund.Name;
                fund.Description = editFund.Description;
                _context.SaveChanges();
            }

            return Ok(fund);
        }

        [HttpGet]
        public IActionResult ExportToCSV()
        {
            if (_configuration != null)
            {
                IConfiguration configuration = _configuration;
                ApplicationDbContext context = _context;

                DataHelper dataHelper = new DataHelper(configuration, context);
                IQueryable fundValueCSV = dataHelper.ExportToExcel();

                return Ok(fundValueCSV);
            }

            return Ok();
        }
    }
}
