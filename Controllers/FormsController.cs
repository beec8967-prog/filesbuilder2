using FormBuilder.Api.Data;
using FormBuilder.Api.DTOs;
using FormBuilder.Api.Models;
using FormBuilder.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FormBuilder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HtmlFormParser _htmlFormParser;
        private readonly FormService _formService;

        public FormsController(
            AppDbContext context,
            HtmlFormParser htmlFormParser,
            FormService formService)
        {
            _context = context;
            _htmlFormParser = htmlFormParser;
            _formService = formService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateForm(
            CreateFormRequest request)
        {
            var form = await _formService.CreateFormAsync(request);

            return Ok(new
            {
                message = "Form created successfully",
                id = form.Id
            });
        }

        [HttpPost("from-html")]
        public async Task<IActionResult> CreateFormFromHtml(
            HtmlFormRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Html))
            {
                return BadRequest(new
                {
                    message = "HTML is required"
                });
            }

            CreateFormRequest formRequest;

            try
            {
                formRequest = _htmlFormParser.Parse(request.Html);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }

            var form = new FormTemplate
            {
                Name = formRequest.Name,
                CreatedBy = formRequest.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var fieldRequest in formRequest.Fields)
            {
                form.Fields.Add(new FormField
                {
                    Label = fieldRequest.Label,
                    FieldType = fieldRequest.FieldType,
                    Order = fieldRequest.Order
                });
            }

            foreach (var stepRequest in formRequest.ApprovalSteps)
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

            return Ok(new
            {
                message = "Form created from HTML successfully",
                id = form.Id
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(int id)
        {
            var form = await _context.FormTemplates
                .FirstOrDefaultAsync(x => x.Id == id);

            if (form == null)
            {
                return NotFound(new
                {
                    message = "Form not found"
                });
            }

            _context.FormTemplates.Remove(form);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Form deleted successfully",
                id
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForm(
            int id,
            UpdateFormRequest request)
        {
            var form = await _context.FormTemplates
                .Include(x => x.Fields)
                .Include(x => x.ApprovalSteps)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (form == null)
            {
                return NotFound(new
                {
                    message = "Form not found"
                });
            }

            form.Name = request.Name;
            form.CreatedBy = request.CreatedBy;

            _context.FormFields.RemoveRange(form.Fields);
            _context.ApprovalSteps.RemoveRange(form.ApprovalSteps);

            form.Fields.Clear();
            form.ApprovalSteps.Clear();

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

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Form updated successfully",
                id = form.Id
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetForms()
        {
            var forms = await _context.FormTemplates
                .Include(x => x.Fields)
                .Include(x => x.ApprovalSteps)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var response = forms.Select(form => new FormResponse
            {
                Id = form.Id,
                Name = form.Name,
                CreatedAt = form.CreatedAt,
                CreatedBy = form.CreatedBy,

                Fields = form.Fields
                    .OrderBy(x => x.Order)
                    .Select(field => new FormFieldResponse
                    {
                        Id = field.Id,
                        Label = field.Label,
                        FieldType = field.FieldType,
                        Order = field.Order,

                        Options = string.IsNullOrWhiteSpace(field.OptionsJson)
                            ? new List<FieldOptionDto>()
                            : JsonSerializer.Deserialize<List<FieldOptionDto>>(
                                field.OptionsJson) ?? new List<FieldOptionDto>()
                    })
                    .ToList()
            }).ToList();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetForm(int id)
        {
            var form = await _context.FormTemplates
                .Include(x => x.Fields)
                .Include(x => x.ApprovalSteps)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (form == null)
            {
                return NotFound(new
                {
                    message = "Form not found"
                });
            }

            var response = new FormResponse
            {
                Id = form.Id,
                Name = form.Name,
                CreatedAt = form.CreatedAt,
                CreatedBy = form.CreatedBy,

                Fields = form.Fields
                    .OrderBy(x => x.Order)
                    .Select(field => new FormFieldResponse
                    {
                        Id = field.Id,
                        Label = field.Label,
                        FieldType = field.FieldType,
                        Order = field.Order,

                        Options = string.IsNullOrWhiteSpace(field.OptionsJson)
                            ? new List<FieldOptionDto>()
                            : JsonSerializer.Deserialize<List<FieldOptionDto>>(
                                field.OptionsJson) ?? new List<FieldOptionDto>()
                    })
                    .ToList()
            };

            return Ok(response);
        }
    }
}
