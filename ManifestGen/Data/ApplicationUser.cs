using Microsoft.AspNetCore.Identity;

namespace ManifestGen.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ICollection<UserFile>? Files { get; set; }
    }

    public class UserFile
    {
        public string? UserFileId { get; set; }

        public string? FileName { get; set; }


        public byte[]? ManifestData { get; set; }

        public string? ApplicationUserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }
    }

}
