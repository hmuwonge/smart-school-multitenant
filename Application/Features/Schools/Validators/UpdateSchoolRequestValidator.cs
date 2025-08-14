using Domain.Entities;
using FluentValidation;

namespace Application.Features.Schools.Validators
{
    public class UpdateSchoolRequestValidator : AbstractValidator<UpdateSchoolRequest>
    {
        public UpdateSchoolRequestValidator(ISchoolService schoolService)
        {
            RuleFor(request => request.Id)
                .NotEmpty()
                .MustAsync(async (id, ct) => await schoolService.GetByIdAsync(id) is School
                schoolIndb && schoolIndb.Id == id)
                .WithMessage("School does not exist");

            RuleFor(request => request.Name).NotEmpty()
               .WithMessage("School name is required")
               .MaximumLength(60);

            RuleFor(request => request.EstablishedDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Date established cannot be future date");
        }
    }
}
