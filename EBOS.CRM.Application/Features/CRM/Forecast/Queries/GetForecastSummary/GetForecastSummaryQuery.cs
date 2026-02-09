using EBOS.CRM.Application.Contracts.Requests.CRM.Forecast;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Forecast.Queries.GetForecastSummary;

public record GetForecastSummaryQuery(GetForecastRequest ForecastRequest) : IRequest<ForecastSummaryResponse>;
