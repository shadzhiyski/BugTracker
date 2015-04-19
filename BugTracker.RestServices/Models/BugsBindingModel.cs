namespace BugTracker.RestServices.Models
{
    using BugTracker.Data.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class BugsBindingModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public BugStatus Status { get; set; }
    }
}