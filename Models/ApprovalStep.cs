namespace FormBuilder.Api.Models
{
    public class ApprovalStep
    {
        public int Id { get; set; }

        public int FormTemplateId { get; set; }

        public int StepOrder { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Approver { get; set; } = string.Empty;

        public string ActionType { get; set; } = string.Empty;

        public FormTemplate FormTemplate { get; set; } = null!;

        public ICollection<Approval> Approvals { get; set; }
            = new List<Approval>();
    }
}
