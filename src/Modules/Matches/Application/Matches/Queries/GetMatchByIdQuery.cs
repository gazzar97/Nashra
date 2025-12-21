using FluentValidation;
using MediatR;
using SportsData.Modules.Matches.Application.Matches.GetMatches;
using SportsData.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsData.Modules.Matches.Application.Matches.Queries
{
    public class GetMatchByIdQuery : IRequest<Result<MatchDto>>
    {
        public int matchId { get; set; }
    }
    public class GetMatchByIdQueryValidator : AbstractValidator<GetMatchByIdQuery>
    {
        public GetMatchByIdQueryValidator()
        {
            
            RuleFor(x => x.matchId).GreaterThan(0).WithMessage("Match ID must be greater than zero.");
        }
    }
    public class GetMatchByIdQueryHandler : IRequestHandler<GetMatchByIdQuery, Result<MatchDto>>
    {
        public Task<Result<MatchDto>> Handle(GetMatchByIdQuery request, CancellationToken cancellationToken)
        {
            var match = new MatchDto(Guid.NewGuid(), "Al Ahly vs Zamalek", "Scheduled");
            return Task.FromResult(Result<MatchDto>.Success(match));
        }
    }   
}
