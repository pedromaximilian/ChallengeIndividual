using System;
using System.Collections.Generic;

#nullable disable

namespace ChallengeIndividual.Models
{
    public partial class Category
    {


        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<Post> Posts { get; set; }
    }
}
