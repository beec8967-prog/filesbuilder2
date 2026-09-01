namespace FormBuilder.Api.Models
{
    public class Approval
    {
        public int Id { get; set; }

        public int FormSubmissionId { get; set; }

        public int ApprovalStepId { get; set; }

        public string Approver { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public string? Comment { get; set; }

        public DateTime? ActionAt { get; set; }

        public FormSubmission FormSubmission { get; set; } = null!;

        public ApprovalStep ApprovalStep { get; set; } = null!;
    }
}
