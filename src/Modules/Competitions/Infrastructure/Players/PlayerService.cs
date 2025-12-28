using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Competitions.Application.Players.Services;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Infrastructure.Players
{
    public class PlayerService : IPlayerService
    {
        private readonly CompetitionsDbContext _dbContext;

        public PlayerService(CompetitionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<PlayerDto>> GetPlayersAsync(Guid? teamId, Guid? seasonId, string? position, string? nationality, int page, int pageSize)
        {
            var query = _dbContext.Players.AsNoTracking().AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(position))
            {
                query = query.Where(p => p.Position == position);
            }

            if (!string.IsNullOrWhiteSpace(nationality))
            {
                query = query.Where(p => p.Nationality == nationality);
            }

            // If team or season filter is provided, join with TeamPlayerSeason
            if (teamId.HasValue || seasonId.HasValue)
            {
                var tpsQuery = _dbContext.TeamPlayerSeasons.AsNoTracking().AsQueryable();

                if (teamId.HasValue)
                {
                    tpsQuery = tpsQuery.Where(tps => tps.TeamId == teamId.Value);
                }

                if (seasonId.HasValue)
                {
                    tpsQuery = tpsQuery.Where(tps => tps.SeasonId == seasonId.Value);
                }

                var playerIds = tpsQuery.Select(tps => tps.PlayerId).Distinct();
                query = query.Where(p => playerIds.Contains(p.Id));

                // Join to get team and shirt number info
                var projectedQuery = from player in query
                                     join tps in _dbContext.TeamPlayerSeasons on player.Id equals tps.PlayerId
                                     join team in _dbContext.Teams on tps.TeamId equals team.Id
                                     where (!teamId.HasValue || tps.TeamId == teamId.Value) &&
                                           (!seasonId.HasValue || tps.SeasonId == seasonId.Value)
                                     select new PlayerDto(
                                         player.Id,
                                         player.Name,
                                         player.DateOfBirth,
                                         player.Nationality,
                                         player.Position,
                                         player.Height,
                                         player.Weight,
                                         tps.ShirtNumber,
                                         team.Name);

                return await projectedQuery.Distinct().ToPagedListAsync(page, pageSize);
            }

            // No team/season filter - return players without team context
            var simpleQuery = query.Select(p => new PlayerDto(
                p.Id,
                p.Name,
                p.DateOfBirth,
                p.Nationality,
                p.Position,
                p.Height,
                p.Weight,
                null,
                null));

            return await simpleQuery.ToPagedListAsync(page, pageSize);
        }

        public async Task<PlayerDetailDto?> GetPlayerByIdAsync(Guid playerId)
        {
            var player = await _dbContext.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (player == null)
                return null;

            // Get current team (most recent assignment without end date)
            var currentTeamAssignment = await (from tps in _dbContext.TeamPlayerSeasons
                                               join team in _dbContext.Teams on tps.TeamId equals team.Id
                                               join season in _dbContext.Seasons on tps.SeasonId equals season.Id
                                               where tps.PlayerId == playerId && tps.EndDate == null
                                               orderby tps.StartDate descending
                                               select new PlayerCurrentTeamDto(
                                                   team.Id,
                                                   team.Name,
                                                   season.Id,
                                                   season.Year,
                                                   tps.ShirtNumber,
                                                   tps.StartDate))
                                              .FirstOrDefaultAsync();

            // Get career history
            var careerHistory = await (from tps in _dbContext.TeamPlayerSeasons
                                       join team in _dbContext.Teams on tps.TeamId equals team.Id
                                       join season in _dbContext.Seasons on tps.SeasonId equals season.Id
                                       where tps.PlayerId == playerId
                                       orderby tps.StartDate descending
                                       select new PlayerCareerEntryDto(
                                           team.Id,
                                           team.Name,
                                           season.Id,
                                           season.Year,
                                           tps.ShirtNumber,
                                           tps.StartDate,
                                           tps.EndDate))
                                      .ToListAsync();

            return new PlayerDetailDto(
                player.Id,
                player.Name,
                player.DateOfBirth,
                player.Nationality,
                player.Position,
                player.Height,
                player.Weight,
                currentTeamAssignment,
                careerHistory);
        }

        public async Task<PagedList<PlayerDto>> GetPlayersByTeamAsync(Guid teamId, Guid seasonId, int page, int pageSize)
        {
            var query = from player in _dbContext.Players.AsNoTracking()
                        join tps in _dbContext.TeamPlayerSeasons on player.Id equals tps.PlayerId
                        join team in _dbContext.Teams on tps.TeamId equals team.Id
                        where tps.TeamId == teamId && tps.SeasonId == seasonId
                        orderby tps.ShirtNumber
                        select new PlayerDto(
                            player.Id,
                            player.Name,
                            player.DateOfBirth,
                            player.Nationality,
                            player.Position,
                            player.Height,
                            player.Weight,
                            tps.ShirtNumber,
                            team.Name);

            return await query.ToPagedListAsync(page, pageSize);
        }
    }
}
