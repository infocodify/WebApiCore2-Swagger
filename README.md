# WebApiCore2-Swagger
Web Api Crud operations and Swagger 

1.  Create Web Api Core 2 Project and install Swashbuckle.AspNetCore using Nuget Package

2.  Define the use In Memory Database and enable Swagger in your Startup.cs file 
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using WebAppiCore2.Models;

namespace WebAppiCore2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            //Use In Memory Database
            services.AddDbContext<TicketContext>(opt => opt.UseInMemoryDatabase("TicketList"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //put swagger in http pipeline
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();
        }
    }
}
```
3.  Create TiketItem class in the Models folder to initialize in memory database
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebAppiCore2.Models
{
    public class TicketItem
    {
        public long Id { get; set; }
        public string Concert { get; set; }
        public string Artist { get; set; }
        public bool Available { get; set; }

    }

    public class TicketContext : DbContext
    {
        public TicketContext(DbContextOptions<TicketContext> options): base(options)
        {

        }

        public DbSet<TicketItem> TicketItems { get; set; }
    }
}
```

4.  In the Controller folder create CRUD (Post, Read, Update, Delete) operations
```csharp
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
```

5.  Donwload Poastman API (https://www.getpostman.com/) and test CRUD Methods \
    a.  http://localhost:portnumber/api/ticket \
    b. Open Postman copy-paste the link and test the CRUD methods
    
6.  You can also use Swagger to test Web Api CRUD methods \
    a.  http://localhost:portnumber/swagger/index.html
