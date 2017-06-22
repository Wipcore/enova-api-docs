namespace Wipcore.Enova.Api.Abstractions.Models
{
    public class ApiResponseHeadersModel
    {
        public int TotalRecordsCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int PageCount { get; set; }
    }
}
