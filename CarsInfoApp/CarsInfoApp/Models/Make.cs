using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace CarsInfoApp.Models
{
    public partial class Make
    {
        public Make()
        {
            Models = new HashSet<Model>();
        }

        public Guid Id { get; set; }

        [Required, MaxLength(64)]
        public string Name { get; set; }

        [Required, Display(Name = "Establishment date"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EstablishmentDate { get; set; }

        [Display(Name = "Total cars made")]
        public int? TotalCarsMade { get; set; }

        [Display(Name = "Income")]
        public decimal? TotalIncome { get; set; }

        public virtual ICollection<Model> Models { get; set; }
    }
}
