namespace CI_Platform.Models.ViewModels
{
    public class MissionListingVM
    {
         
       public int MissionCount { get; set; }
       //public Pager Pager { get; set; }
       public List<MissionVM> Missions { get; set; }
        
    }
}
