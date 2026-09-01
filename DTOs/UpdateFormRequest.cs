namespace FormBuilder.Api.DTOs
{
    public class UpdateFormRequest
    {
        public string Name { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;

        public List<CreateFieldRequest> Fields { get; set; } = new();

        public List<CreateApprovalStepRequest> ApprovalSteps { get; set; } = new();
    }
}
