using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GenericController.API.Models
{
    public class Comment : ApplicationEntity
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public Guid PostId { get; set; }
        public Post Post { get; set; }
    }
}
