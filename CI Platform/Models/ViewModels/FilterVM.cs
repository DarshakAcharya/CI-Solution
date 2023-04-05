namespace CI_Platform.Models.ViewModels
{
    public class FilterVM
    {
        public string? SearchInput { get; set; }
        public long[] CountryFilter { get; set; }

        public long[] CityFilter { get; set; }

        public long[] ThemesFilter { get; set; }

        public long[] SkillsFilter { get; set; }

         
    }
}
