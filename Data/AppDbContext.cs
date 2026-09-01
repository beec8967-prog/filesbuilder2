using FormBuilder.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<FormTemplate> FormTemplates { get; set; }

        public DbSet<FormField> FormFields { get; set; }

        public DbSet<ApprovalStep> ApprovalSteps { get; set; }

        public DbSet<FormSubmission> FormSubmissions { get; set; }

        public DbSet<SubmissionValue> SubmissionValues { get; set; }

        public DbSet<Approval> Approvals { get; set; }
    }
}
