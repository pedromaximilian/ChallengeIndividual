using System;
using System.Collections.Generic;

#nullable disable

namespace ChallengeIndividual.Models
{
    public partial class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Article { get; set; }
        public string Image { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}
