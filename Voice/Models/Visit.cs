using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Voice.Models
{
    public class WavFile
    {
        [Key]
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Path { get; set; }
        public double Jitter { get; set; }
        public double Shimmer { get; set; }
        public double HNR { get; set; }
        public double Intensity { get; set; }
        public double FirstPitch { get; set; }
        public double PitchMassive { get; set; }

        public int VisitId { get; set; }
        public Visit Visit { get; set; }
    }
    public class Visit
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime DateTime { get; set; }
        public virtual ICollection<WavFile> Files { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public Visit()
        {
            Files = new List<WavFile>();
        }
    }
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string Sex { get; set; }

        public virtual ICollection<Visit> Visits { get; set; }
        public Patient()
        {
            Visits = new List<Visit>();
        }
    }
}