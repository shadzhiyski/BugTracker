namespace BugTracker.RestServices.Controllers
{
    using BugTracker.Data;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Data.Entity;
    using BugTracker.RestServices.Models;
    using BugTracker.Data.Models;
    using System;

    [RoutePrefix("api")]
    public class CommentsController : ApiController
    {
        private readonly BugTrackerDbContext db;

        public CommentsController()
            : this(new BugTrackerDbContext())
        {
        }

        public CommentsController(BugTrackerDbContext data)
        {
            this.db = data;
        }

        // GET: api/comments
        [HttpGet]
        [Route("comments")]
        public IHttpActionResult GetAllComments()
        {
            var comments = db.Comments
                .Include(c => c.Bug)
                .OrderByDescending(c => c.PublishDate)
                .Select(c => new
                {
                    c.Id,
                    c.Text,
                    Author = c.Author != null ? c.Author.UserName : null,
                    DateCreated = c.PublishDate,
                    BugId = c.Bug.Id,
                    BugTitle = c.Bug.Title
                });

            return this.Ok(comments);
        }

        // GET: api/bugs/{id}/comments
        [HttpGet]
        [Route("bugs/{id:int}/comments")]
        public IHttpActionResult GetCommentsByBugId(int id)
        {
            var bug = db.Bugs.Find(id);
            if (bug == null)
            {
                return this.NotFound();
            }

            var comments = db.Comments
                .Where(c => c.Bug.Id == id)
                .OrderByDescending(c => c.PublishDate)
                .Select(c => new 
                {
                    c.Id,
                    c.Text,
                    Author = c.Author != null ? c.Author.UserName : null,
                    DateCreated = c.PublishDate
                });
            
            return Ok(comments);
        }

        // POST: api/bugs/{id}/comments
        [HttpPost]
        [Route("bugs/{id:int}/comments")]
        public IHttpActionResult CreateComment(int id, CommentBindingModel commentData)
        {
            var bug = db.Bugs.Find(id);
            if(bug == null) 
            {
                return NotFound();
            }

            if (commentData == null)
            {
                return BadRequest("Missing comment data.");
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

            var comment = new Comment() 
            { 
                Text = commentData.Text,
                Author = user,
                PublishDate = DateTime.Now,
                Bug = bug
            };
            db.Comments.Add(comment);
            db.SaveChanges();

            if (user != null)
            {
                return this.Ok(
                new { Id = comment.Id, Author = user.UserName, Message = "User comment added for bug #" + bug.Id });
            }

            return this.Ok(
                new {Id = comment.Id, Message = "Added anonymous comment for bug #" + bug.Id});
        }
    }
}