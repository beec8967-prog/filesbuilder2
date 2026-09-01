namespace FormBuilder.Api.DTOs
{
    public class FormResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public List<FormFieldResponse> Fields { get; set; } = new();
    }
}
