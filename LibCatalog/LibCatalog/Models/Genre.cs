using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LibCatalog.Models
{
    public class Genre
    {

        [Key]
        public int GenreId { get; set; }

        [Display(Name = "Genre")]
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string GenreName { get; set; }

        public virtual ICollection<Book> Books { get; set; }

    }
}