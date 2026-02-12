using EBOS.CRM.Contracts.Requests.CRM.Forecast;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Forecast.Queries.GetForecastSummary;

public record GetForecastSummaryQuery(GetForecastRequest ForecastRequest) : IRequest<ForecastSummaryResponse>;
