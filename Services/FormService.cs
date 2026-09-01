using FormBuilder.Api.Data;
using FormBuilder.Api.DTOs;
using FormBuilder.Api.Models;

namespace FormBuilder.Api.Services
{
    public class FormService
    {
        private readonly AppDbContext _context;

        public FormService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FormTemplate> CreateFormAsync(
            CreateFormRequest request)
        {
            var form = new FormTemplate
            {
                Name = request.Name,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var fieldRequest in request.Fields)
            {
                form.Fields.Add(new FormField
                {
                    Label = fieldRequest.Label,
                    FieldType = fieldRequest.FieldType,
                    Order = fieldRequest.Order,
                    OptionsJson = fieldRequest.OptionsJson
                });
            }

            foreach (var stepRequest in request.ApprovalSteps)
            {
                form.ApprovalSteps.Add(new ApprovalStep
                {
                    StepOrder = stepRequest.StepOrder,
                    Name = stepRequest.Name,
                    Approver = stepRequest.Approver,
                    ActionType = stepRequest.ActionType
                });
            }

            _context.FormTemplates.Add(form);

            await _context.SaveChangesAsync();

            return form;
        }
    }
}
