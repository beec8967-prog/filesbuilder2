namespace FormBuilder.Api.Models
{
    public class FormField
    {
        public int Id { get; set; }

        public int FormTemplateId { get; set; }

        public string Label { get; set; } = string.Empty;

        public string FieldType { get; set; } = string.Empty;

        public int Order { get; set; }

        public string? OptionsJson { get; set; }

        public ICollection<SubmissionValue> SubmissionValues { get; set; }
            = new List<SubmissionValue>();

        public FormTemplate FormTemplate { get; set; } = null!;
    }
}
