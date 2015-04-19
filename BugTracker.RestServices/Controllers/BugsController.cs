namespace BugTracker.RestServices.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Web.Http;

    using BugTracker.Data;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using BugTracker.RestServices.Models;
    using BugTracker.Data.Models;
    using Newtonsoft.Json;

    [RoutePrefix("api")]
    public class BugsController : ApiController
    {
        private readonly BugTrackerDbContext db;

        public BugsController()
            : this(new BugTrackerDbContext())
        {
        }

        public BugsController(BugTrackerDbContext data)
        {
            this.db = data;
        }

        // GET: api/bugs
        [HttpGet]
        [Route("bugs")]
        public IHttpActionResult GetAllBugs()
        {
            var bugs = db.Bugs
                .OrderByDescending(c => c.DateCreated)
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    c.Status,
                    c.Author.UserName
                });

            return this.Ok(bugs);
        }

        // GET: api/bugs/{id}
        [HttpGet]
        [Route("bugs/{id:int}")]
        public IHttpActionResult GetBugDetailsById(int id)
        {
            var bug = db.Bugs
                .Include(b => b.Comments)
                .FirstOrDefault(b => b.Id == id);
            if (bug == null)
            {
                return this.NotFound();
            }

            return Ok(new
            {
                bug.Id,
                bug.Title,
                bug.Description,
                bug.Status,
                Author = bug.Author != null ? bug.Author.UserName : null,
                bug.DateCreated,
                Comments = bug.Comments
                .OrderByDescending(c => c.PublishDate)
                .Select(c => new 
                {
                    c.Id,
                    c.Text,
                    Author = c.Author != null ? c.Author.UserName : null,
                    DataCreated = c.PublishDate
                })
            });
        }

        // GET: /api/bugs/filter
        [HttpGet]
        [Route("bugs/filter")]
        public IHttpActionResult GetBugsByFilterParameters(
            [FromUri] string keyword = null, 
            [FromUri] string statuses = null,
            [FromUri] string author = null)
        {
            var bugs = db.Bugs
                .Where(b =>
                    (keyword != null ? b.Title.Contains(keyword) : true)
                    && b.Author.UserName == author
                    && (statuses != null ?
                        statuses.Contains(b.Status.ToString()) : 
                        true)
                ).OrderByDescending(b => b.DateCreated)
                .Select(b => new 
                {
                    b.Id,
                    b.Title,
                    b.Status,
                    Author = b.Author != null ? b.Author.UserName : null,
                    b.DateCreated
                });

            return Ok(bugs);
        }

        // POST: api/bugs
        [HttpPost]
        [Route("bugs")]
        public IHttpActionResult CreateBug(BugsBindingModel bugData)
        {
            if (bugData == null)
            {
                return BadRequest("Missing bug data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = null;

            if (User.Identity.IsAuthenticated)
            {
                user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            }

            var bug = new Bug() 
            { 
                Title = bugData.Title,
                Description = bugData.Description,
                Status = BugStatus.Open,
                Author = user,
                DateCreated = DateTime.Now
            };
            db.Bugs.Add(bug);
            db.SaveChanges();
            if (user != null)
            {
                return this.CreatedAtRoute(
                "DefaultApi",
                new { controller = "bugs", id = bug.Id },
                new { Id = bug.Id, Author = user.UserName, Message = "User bug submitted." });
            }
            return this.CreatedAtRoute(
                "DefaultApi",
                new { controller = "bugs", id = bug.Id },
                new {Id = bug.Id, Message = "Anonymous bug submitted."});
        }

        // PATCH: api/bugs/{id}
        [HttpPatch]
        [Route("bugs/{id:int}")]
        public IHttpActionResult EditBug(int id, BugsBindingModel bugData)
        {
            if (bugData == null)
            {
                return BadRequest("Missing bug data.");
            }

            var bug = db.Bugs.Find(id);
            if (bug == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (bugData.Title != null)
            {
                bug.Title = bugData.Title;
            }
            if (bugData.Description != null)
            {
                bug.Description = bugData.Description;
            }

            bug.Status = bugData.Status;
            db.SaveChanges();

            return this.Ok(
                new
                {
                    Message = "Bug #" + id + " patched."
                }
            );
        }

        // DELETE: api/bugs/{id}
        [HttpDelete]
        [Route("bugs/{id:int}")]
        public IHttpActionResult DeleteBug(int id)
        {
            Bug bug = db.Bugs.Find(id);
            if (bug == null)
            {
                return NotFound();
            }

            db.Bugs.Remove(bug);
            db.SaveChanges();

            return Ok(new
            {
                Message = "Bug #" + id + " deleted."
            });
        }

        // GET: Bugs
        /*public ActionResult Index()
        {
            return View();
        }*/
    }
}