namespace EBOS.CRM.Application.Features.CRM.CustomerMerge;

public sealed record CustomerMergeFieldContext(
    string? Source,
    string? ChannelKey,
    string? Confidentiality);
