using Microsoft.AspNetCore.Http;    

namespace SportsData.Shared
{
    public static class ResultHttpExtensions
    {
        public static IResult ToHttpResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return result.StatusCode switch
                {
                    201 => Results.Created(string.Empty, Envelope<T>.Success(result.Value!)),
                    204 => Results.NoContent(),
                    _ => Results.Ok(Envelope<T>.Success(result.Value!))
                };
            }

            return MapFailure<T>(result.Errors, result.StatusCode);
        }

        public static IResult ToHttpResult(this Result result)
        {
            if (result.IsSuccess)
            {
                return result.StatusCode == 204 ? Results.NoContent() : Results.Ok();
            }

            return MapFailure<object>(result.Errors, result.StatusCode);
        }
       
        private static IResult MapFailure<T>(string[] errors, int statusCode)
        {
            return statusCode switch
            {
                404 => Results.NotFound(Envelope<T>.Failure(errors)),
                409 => Results.Conflict(Envelope<T>.Failure(errors)),
                400 => Results.BadRequest(Envelope<T>.Failure(errors)),
                _ => Results.BadRequest(Envelope<T>.Failure(errors))
            };
        }
    }
}
