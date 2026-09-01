namespace FormBuilder.Api.Models
{
    public class SubmissionValue
    {
        public int Id { get; set; }

        public int FormSubmissionId { get; set; }

        public int FormFieldId { get; set; }

        public string Value { get; set; } = string.Empty;

        public FormSubmission FormSubmission { get; set; } = null!;

        public FormField FormField { get; set; } = null!;
    }
}
