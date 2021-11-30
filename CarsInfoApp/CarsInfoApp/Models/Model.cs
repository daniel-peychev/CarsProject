using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace CarsInfoApp.Models
{
    public partial class Model
    {
        public Model()
        {
            Engines = new HashSet<Engine>();
        }

        public Guid Id { get; set; }

        public Guid MakeId { get; set; }

        [Required, MaxLength(64), Display(Name = "Model name")]
        public string ModelName { get; set; }

        [Required, Display(Name = "Start year")]
        public DateTime StartYear { get; set; }

        [Display(Name = "End year")]
        public DateTime? EndYear { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Display(Name = "Total made")]
        public int? TotalModelsMade { get; set; }

        [Display(Name = "Manufacturer")]
        public virtual Make Make { get; set; }

        public virtual ICollection<Engine> Engines { get; set; }
    }
}
