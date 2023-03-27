using CI_Platform_Entites.Models;

namespace CI_Platform.Models.ViewModels
{
    public class MissionVM
    { 
            public long MissionId { get; set; }
            public string Title { get; set; }
            public string ShortDescription { get; set; }
            public string Description { get; set; }

            public string Organization { get; set; }

            public string OrganizationDetails { get; set; }

            public int Rating { get; set; }

            public int UserPrevRating { get; set; }

            public IQueryable? MissionComments { get; set; }

            public string ImgUrl { get; set; }

            public string Theme { get; set; }

            public List<User> RecommendationList { get; set; }

            public int progress { get; set; }

            public string GoalAim { get; set; }

            public List<User> RecentVolunteers { get; set; }

            public List<MissionDocument> MissionDocs { get; set; }

            public string missionType { get; set; }
            public bool isFavrouite { get; set; }

            public bool userApplied { get; set; }

            public string City { get; set; }

            public string StartDateEndDate { get; set; }

            public string StartDate { get; set; }

            public string EndDate { get; set; }

            public int NoOfSeatsLeft { get; set; }

            public string Deadline { get; set; }

            public DateTime createdAt { get; set; }
        }

    }

