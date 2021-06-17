using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengeIndividual.Models.ViewModels
{
    public class PostViewModel
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
        public IFormFile Image { get; set; }
        public virtual Category Category { get; set; }
    }
}
