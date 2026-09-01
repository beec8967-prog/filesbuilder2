namespace FormBuilder.Api.DTOs
{
    public class FormFieldResponse
    {
        public int Id { get; set; }

        public string Label { get; set; } = string.Empty;

        public string FieldType { get; set; } = string.Empty;

        public int Order { get; set; }

        public List<FieldOptionDto> Options { get; set; } = new();
    }
}
