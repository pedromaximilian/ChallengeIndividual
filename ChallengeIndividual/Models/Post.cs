using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ChallengeIndividual.Models
{
    public partial class Post
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter Post Title")]
        [Display(Name = "Post Title")]
        [StringLength(Int32.MaxValue)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Please enter Post Content")]
        [Display(Name = "Content")]
        [StringLength(Int32.MaxValue)]
        public string Article { get; set; }


        [Display(Name = "Category")]
        public int? CategoryId { get; set; }


        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }



        [Display(Name = "Image")]
        public string Image { get; set; }
        public virtual Category Category { get; set; }
    }
}
