namespace EBOS.CRM.Application.Options;

public sealed class CaseWorkflowOptions
{
    public const string SectionName = "CaseWorkflow";

    public bool AllowCloseWithOpenActivities { get; set; }
}
