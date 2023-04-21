using CI_Platform_Entites.Models;
using System.ComponentModel.DataAnnotations;

namespace CI_Platform.Models.ViewModels
{
    public class MyProfilePageVM
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        public string EmployeeID { get; set; }

        public string Manager { get; set; }

        public string Title { get; set; }

        public string Department { get; set; }

        [Required]
        public string MyProfile { get; set; }

        public string WhyIVolunteer { get; set; }

        public string City { get; set; }

        public long CityId { get; set; }

        [Required]
        public string Country { get; set; }

        public long CountryId { get; set; }

        public string Availability { get; set; }

        public string LinkedIn { get; set; }

        public string MySkill { get; set; }

        public List<Skill> MySkills { get; set; }
    }
}
