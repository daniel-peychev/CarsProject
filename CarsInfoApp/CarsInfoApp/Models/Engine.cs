using System;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace CarsInfoApp.Models
{
    public partial class Engine
    {
        public Guid Id { get; set; }

        public Guid ModelId { get; set; }

        [Required, MaxLength(64), Display(Name = "Engine code")]
        public string EngineCode { get; set; }

        [Required, Display(Name = "Horse power")]
        public int HorsePower { get; set; }

        [Required]
        public int Torque { get; set; }

        public string Fuel { get; set; }

        [Display(Name = "Turbo")]
        public bool HasTurbo { get; set; }

        public virtual Model Model { get; set; }
    }
}
