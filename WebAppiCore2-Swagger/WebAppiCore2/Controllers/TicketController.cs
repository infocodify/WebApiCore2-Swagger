using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppiCore2.Models;

namespace WebAppiCore2.Controllers
{
    [Produces("application/json")]
    [Route("api/Ticket")]
    public class TicketController : Controller
    {
        private TicketContext _context;

        public TicketController(TicketContext context)
        {
            _context = context;

            if (_context.TicketItems.Count() == 0)
            {
                _context.TicketItems.Add(new TicketItem
                {
                    Concert = "Beyonce"
                });
                _context.SaveChanges();
            }
        }

        [HttpGet]
        public IEnumerable<TicketItem> GetAll()
        {
            return _context.TicketItems.AsNoTracking().ToList();
        }


        //[HttpGet("{id}", Name = "GetTicket")]  or the following

        [HttpGet("{id}")]
        public IActionResult GetBYId(long id)
        {
            var ticket = _context.TicketItems.FirstOrDefault(t => t.Id == id);

            if(ticket == null)
            {
                return NotFound(); //return 404
            }
            return new ObjectResult(ticket);//return 200
        }

        [HttpPost]
        public IActionResult Create([FromBody]TicketItem ticket)
        {
            if(ticket == null)
            {
                return BadRequest(); // return 400
            }

            _context.TicketItems.Add(ticket);
            _context.SaveChanges();

            //refer as a route
            return CreatedAtRoute("GetTicket", new { id = ticket.Id }, ticket);
        }

        //Update - or PUT HTTP Verbs
        //specify the route
        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TicketItem ticket)
        {
            if (ticket == null || ticket.Id != id)
            {
                return BadRequest();//400
            }

            var tic = _context.TicketItems.FirstOrDefault(t => t.Id == id);
            if (tic == null)
            {
                return NotFound();
            }

            tic.Concert = ticket.Concert;
            tic.Available = ticket.Available;
            tic.Artist = ticket.Artist;

            _context.TicketItems.Update(tic);
            _context.SaveChanges();

            return new NoContentResult();
        }

        //Delete method
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var tics = _context.TicketItems.FirstOrDefault(t => t.Id == id);
            if(tics == null)
            {
                return NotFound();
            }
            _context.TicketItems.Remove(tics);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}