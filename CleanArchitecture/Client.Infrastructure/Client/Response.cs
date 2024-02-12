using Client.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Client.Infrastructure.Client
{
    public class Response<T> : IDataOutput
    {
        public T? Data { get; private set; }

        public ValidationProblemDetails? ValidationErrors { get; private set; }

        public ProblemDetails? ServerError { get; private set; }

        public int Status { get; private set; }

        public IEnumerable<HeaderData> ResponseHeaders { get; private set; }

        public bool ContainsError { get; private set; }

        private readonly int[]? successfulStatuses;

        public object? GetOutputData()
        {
            if (successfulStatuses?.Contains(Status) == true)
            {
                return Data;
            }
            else
            {
                if (ValidationErrors != null)
                {
                    return ValidationErrors;
                }
                else
                {
                    return ServerError;
                }
            }
        }

        public Response(ResponseData response, params int[]? successfulStatuses)
        {
            this.successfulStatuses = successfulStatuses;
            Status = response.StatusCode;
            ResponseHeaders = response.ResponseHeaders;

            if (successfulStatuses?.Contains(response.StatusCode) == true)
            {
                Data = response.ConvertContent<T>();
            }
            else
            {
                try
                {
                    ValidationErrors = response.ConvertContent<ValidationProblemDetails>(new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Error
                    });

                    if (ValidationErrors?.Errors?.Count == 0)
                    {
                        ValidationErrors = null;
                    }

                    ServerError = response.ConvertContent<ProblemDetails>();
                }
                catch (JsonException)
                {
                    ServerError = response.ConvertContent<ProblemDetails>();
                }
                catch (Exception ex)
                {
                    throw new GeneralApplicationException($"Could not parse response for status: {response.StatusCode}, data: {response.Content}.", ex);
                }

                ContainsError = true;
            }
        }

        public Response(ResponseData response, params HttpStatusCode[] successfulStatuses) : this(response, Array.ConvertAll(successfulStatuses, item => (int)item))
        {
        }
    }
}