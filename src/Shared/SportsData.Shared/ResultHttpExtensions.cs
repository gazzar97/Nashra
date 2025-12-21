using Microsoft.AspNetCore.Http;    

namespace SportsData.Shared
{
    public static class ResultHttpExtensions
    {
        public static IResult ToHttpResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Results.Ok(
                    Envelope<T>.Success(result.Value!));
            }

            return MapFailure<T>(result.Errors);
        }
        public static IResult ToHttpResult(this Result result)
        {
            if (result.IsSuccess)
            {
                return Results.NoContent();
            }

            return MapFailure<object>(result.Errors);
        }
       
        private static IResult MapFailure<T>(string[] errors)
        {
            // 🔹 You can improve this logic later without touching endpoints
            return Results.BadRequest(
                Envelope<T>.Failure(errors));
        }

    }
}
