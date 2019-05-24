using System.Collections.Generic;

namespace Demo.Dialogs.UserProfile
{
    public class UserProfileState
    {
        public string Name { get; set; }
        public string Profession { get; set; }
        public string Email { get; set; }
        public List<string> ItemsToReview { get; set; } = new List<string>();
    }
}
