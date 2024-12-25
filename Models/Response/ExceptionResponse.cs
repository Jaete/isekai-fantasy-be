using IsekaiFantasyBE.Services;

namespace IsekaiFantasyBE.Models.Response;

public class ExceptionResponse
{
        public Exception? Exception { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }

        public ExceptionResponse(Exception ex)
        {
            Exception = ex.GetBaseException().InnerException;
            Message = ex.Message;
            StatusCode = ExceptionService.GetStatusCode(ex);
        }
}