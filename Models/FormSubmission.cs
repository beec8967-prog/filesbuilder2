namespace FormBuilder.Api.Models
{
    public class FormSubmission
    {
        public int Id { get; set; }

        public int FormTemplateId { get; set; }

        public string SubmittedBy { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; }

        public string Status { get; set; } = "Pending";

        public FormTemplate FormTemplate { get; set; } = null!;

        public ICollection<SubmissionValue> Values { get; set; }
            = new List<SubmissionValue>();

        public ICollection<Approval> Approvals { get; set; }
            = new List<Approval>();
    }
}
