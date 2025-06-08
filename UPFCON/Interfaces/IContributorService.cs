using UPFCON.Models;
using UPFCON.Requests;

namespace UPFCON.Interfaces;

public interface IContributorService
{
    public Task<List<Contribution>> CreateContributorsAsync(HttpContext httpContext, Paper paper,
        IList<ContributorDto> contributorDtos);
}