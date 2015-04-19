namespace BugTracker.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Comment
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public virtual User Author { get; set; }

        public DateTime PublishDate { get; set; }

        [Required]
        public virtual Bug Bug { get; set; }
    }
}
