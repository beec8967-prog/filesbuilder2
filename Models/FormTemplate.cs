namespace FormBuilder.Api.Models
{
    public class FormTemplate
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public ICollection<FormField> Fields { get; set; } = new List<FormField>();

        public ICollection<ApprovalStep> ApprovalSteps { get; set; } = new List<ApprovalStep>();

        public ICollection<FormSubmission> Submissions { get; set; }
            = new List<FormSubmission>();
    }
}
