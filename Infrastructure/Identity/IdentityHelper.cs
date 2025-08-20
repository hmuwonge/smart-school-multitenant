using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class IdentityHelper
{
    public static List<string> GetIdentityResultErrorDescriptions(IdentityResult identityResult)
    {
        // return identityResult.Errors.Select(errror => errror.Description).ToList();
        var errorDescriptions = new List<string>();

        foreach (var error in identityResult.Errors)
        {
            errorDescriptions.Add(error.Description);
        }
        return errorDescriptions;
    }
}