using UPFCON.Models;
using UPFCON.Requests;

namespace UPFCON.Interfaces;

public interface IContributorService
{
    public Task CreateContributorsAsync(HttpContext httpContext, Paper paper,
        IList<ContributorDto> contributorDtos);
}