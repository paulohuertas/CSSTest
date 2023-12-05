using CSSTest.Data;
using CSSTest.Models;
using CSSTest.Models.Helper;
using Microsoft.AspNetCore.Mvc;
using CSSTest.DTO;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace CSSTest.Controllers.API
{
    [ApiController]
    [Route("[controller]")]
    public class FundAPIController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataHelper _dataHelper;

        public FundAPIController(ApplicationDbContext context, DataHelper dataHelper)
        {
            _context = context;
            _dataHelper = dataHelper;
        }

        [HttpPost("{funds}")]
        [Route("Fund/CreateFunds")]
        public IActionResult CreateFunds()
        {
            if (_dataHelper != null)
            {
                var funds = _dataHelper.CreateFunds(1, 5);

                return Ok(funds);
            }

            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetLatestRecordById(int id)
        {
            var fund = (from f in _context.Funds
                        join v in _context.Value on f.Id equals v.FundId
                        where f.Id == id
                        orderby v.ValueId descending
                        select new
                        {
                            f.Id,
                            f.Name,
                            f.Description,
                            v.ValueDate,
                            ValuePrice = v.ValueDouble,
                            ValueiD = v.ValueId
                        }).Take(1);

            if (fund == null) return BadRequest();

            return Ok(fund);
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
            if (_dataHelper != null)
            {
                IQueryable fundValueCSV = _dataHelper.ExportToExcel();

                return Ok(fundValueCSV);
            }

            return Ok();
        }
    }
}
