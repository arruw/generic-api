using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GenericController.API.Models
{
    public class Post : ApplicationEntity
    {
        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
