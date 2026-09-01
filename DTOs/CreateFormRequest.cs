namespace FormBuilder.Api.DTOs
{
    public class CreateFormRequest
    {
        public string Name { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;

        public List<CreateFieldRequest> Fields { get; set; } = new();

        public List<CreateApprovalStepRequest> ApprovalSteps { get; set; } = new();
    }

    public class CreateFieldRequest
    {
        public string Label { get; set; } = string.Empty;

        public string FieldType { get; set; } = string.Empty;

        public int Order { get; set; }

        public string? OptionsJson { get; set; }
    }

    public class CreateApprovalStepRequest
    {
        public int StepOrder { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Approver { get; set; } = string.Empty;

        public string ActionType { get; set; } = string.Empty;
    }

    public class ImportHtmlRequest
    {
        public string Html { get; set; } = string.Empty;
    }
}
