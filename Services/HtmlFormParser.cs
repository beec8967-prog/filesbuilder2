using FormBuilder.Api.DTOs;
using HtmlAgilityPack;
using System.Text.Json;

namespace FormBuilder.Api.Services
{
    public class HtmlFormParser
    {
        public CreateFormRequest Parse(string html)
        {
            var document = new HtmlDocument();

            document.LoadHtml(html);

            var formNode = document.DocumentNode.SelectSingleNode("//form");

            if (formNode == null)
            {
                throw new ArgumentException("HTML does not contain a form element.");
            }

            var formName =
                formNode.GetAttributeValue("data-form-name", "Untitled Form");

            var request = new CreateFormRequest
            {
                Name = formName,
                CreatedBy = "html-import"
            };

            ParseFields(formNode, request);

            ParseApprovalSteps(formNode, request);

            return request;
        }

        private void ParseFields(
            HtmlNode formNode,
            CreateFormRequest request)
        {
            var fieldNodes = formNode.SelectNodes(
                ".//input | .//textarea | .//select");

            if (fieldNodes == null)
            {
                return;
            }

            var order = 1;

            foreach (var field in fieldNodes)
            {
                var name = field.GetAttributeValue("name", "");

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                var fieldType = GetFieldType(field);

                // checkbox / radio יכולים להיות קבוצת אפשרויות.
                // כרגע נשמור כל אלמנט כשדה נפרד.
                var label = GetLabelForInput(field, formNode);

                var optionsJson = GetOptionsJson(field);

                request.Fields.Add(new CreateFieldRequest
                {
                    Label = label,
                    FieldType = fieldType,
                    Order = order++,
                    OptionsJson = optionsJson
                });
            }
        }

        private string GetFieldType(HtmlNode field)
        {
            if (field.Name.Equals(
                "textarea",
                StringComparison.OrdinalIgnoreCase))
            {
                return "textarea";
            }

            if (field.Name.Equals(
                "select",
                StringComparison.OrdinalIgnoreCase))
            {
                return "select";
            }

            var inputType = field.GetAttributeValue(
                "type",
                "text");

            return inputType.ToLowerInvariant() switch
            {
                "checkbox" => "checkbox",
                "radio" => "radio",
                "number" => "number",
                "date" => "date",
                "email" => "email",
                "tel" => "tel",
                "password" => "password",
                _ => "text"
            };
        }

        private string? GetOptionsJson(HtmlNode field)
        {
            if (!field.Name.Equals(
                "select",
                StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var optionNodes = field.SelectNodes(".//option");

            if (optionNodes == null)
            {
                return null;
            }

            var options = optionNodes
                .Select(option => new FieldOptionDto
                {
                    Value = option.GetAttributeValue("value", ""),
                    Label = option.InnerText.Trim()
                })
                .ToList();

            return JsonSerializer.Serialize(options);
        }

        private string GetLabelForInput(
            HtmlNode input,
            HtmlNode formNode)
        {
            var inputId = input.GetAttributeValue("id", "");

            // אפשרות 1:
            // <label for="fullName">שם מלא</label>
            // <input id="fullName" ... />

            if (!string.IsNullOrWhiteSpace(inputId))
            {
                var labelNode = formNode.SelectSingleNode(
                    $".//label[@for='{inputId}']");

                if (labelNode != null)
                {
                    return labelNode.InnerText.Trim();
                }
            }

            // אפשרות 2:
            // <label>שם מלא <input ... /></label>

            var parent = input.ParentNode;

            if (parent != null &&
                parent.Name.Equals(
                    "label",
                    StringComparison.OrdinalIgnoreCase))
            {
                var labelText = parent.InnerText.Trim();

                if (!string.IsNullOrWhiteSpace(labelText))
                {
                    return labelText;
                }
            }

            // אפשרות 3:
            // <label>שם מלא</label>
            // <input ... />

            var previousNode = input.PreviousSibling;

            while (previousNode != null)
            {
                if (previousNode.NodeType == HtmlNodeType.Element &&
                    previousNode.Name.Equals(
                        "label",
                        StringComparison.OrdinalIgnoreCase))
                {
                    var labelText = previousNode.InnerText.Trim();

                    if (!string.IsNullOrWhiteSpace(labelText))
                    {
                        return labelText;
                    }
                }

                previousNode = previousNode.PreviousSibling;
            }

            // אם לא נמצא label - נחזור ל-name

            return input.GetAttributeValue("name", "Unnamed Field");
        }

        private void ParseApprovalSteps(
            HtmlNode formNode,
            CreateFormRequest request)
        {
            var stepNodes =
                formNode.SelectNodes(".//*[@data-approval-step]");

            if (stepNodes == null)
            {
                return;
            }

            foreach (var step in stepNodes)
            {
                var orderText =
                    step.GetAttributeValue(
                        "data-approval-step",
                        "1");

                int.TryParse(orderText, out var order);

                var approverNode =
                    step.SelectSingleNode(".//*[@data-approver]");

                var actionNode =
                    step.SelectSingleNode(".//*[@data-action]");

                var approver =
                    approverNode?
                        .GetAttributeValue("data-approver", "")
                    ?? "";

                var action =
                    actionNode?
                        .GetAttributeValue("data-action", "")
                    ?? "";

                var name =
                    step.SelectSingleNode(".//span")?
                        .InnerText
                        .Trim()
                    ?? $"Approval Step {order}";

                request.ApprovalSteps.Add(
                    new CreateApprovalStepRequest
                    {
                        StepOrder = order,
                        Name = name,
                        Approver = approver,
                        ActionType = action
                    });
            }
        }
    }
}
