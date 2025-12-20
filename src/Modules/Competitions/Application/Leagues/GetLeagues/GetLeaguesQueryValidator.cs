using FluentValidation;

namespace SportsData.Modules.Competitions.Application.Leagues.GetLeagues
{
    public class GetLeaguesQueryValidator : AbstractValidator<GetLeaguesQuery>
    {
        public GetLeaguesQueryValidator()
        {
            // Example rule: e.g. if we had filters
            // RuleFor(x => x.Country).NotEmpty(); 
            // Since query is empty currently, no rules needed but class exists for pattern proof.
        }
    }
}
