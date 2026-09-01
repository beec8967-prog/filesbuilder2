using FluentValidation;
using FormBuilder.Api.DTOs;

namespace FormBuilder.Api.Validators
{
    public class CreateFormRequestValidator
        : AbstractValidator<CreateFormRequest>
    {
        public CreateFormRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Form name is required.");

            RuleFor(x => x.CreatedBy)
                .NotEmpty()
                .WithMessage("CreatedBy is required.");

            RuleFor(x => x.Fields)
                .NotNull()
                .WithMessage("Fields are required.");

            RuleForEach(x => x.Fields)
                .ChildRules(field =>
                {
                    field.RuleFor(x => x.Label)
                        .NotEmpty()
                        .WithMessage("Field label is required.");

                    field.RuleFor(x => x.FieldType)
                        .Must(IsValidFieldType)
                        .WithMessage(
                            "Field type must be text, date, or number.");
                });

            RuleFor(x => x.ApprovalSteps)
                .NotNull()
                .WithMessage("ApprovalSteps are required.");

            RuleForEach(x => x.ApprovalSteps)
                .ChildRules(step =>
                {
                    step.RuleFor(x => x.Name)
                        .NotEmpty()
                        .WithMessage("Approval step name is required.");

                    step.RuleFor(x => x.Approver)
                        .NotEmpty()
                        .WithMessage("Approver is required.");

                    step.RuleFor(x => x.ActionType)
                        .NotEmpty()
                        .WithMessage("Action type is required.");
                });
        }

        private static bool IsValidFieldType(string fieldType)
        {
            var validTypes = new[]
            {
                "text",
                "date",
                "number",
                "email",
                "tel",
                "password",
                "textarea",
                "select",
                "checkbox",
                "radio"
            };

            return validTypes.Contains(
                fieldType,
                StringComparer.OrdinalIgnoreCase);
        }
    }
}
