namespace BugTracker.RestServices.Models
{
    using BugTracker.Data.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CommentBindingModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [MinLength(1)]
        public string Text { get; set; }
    }
}