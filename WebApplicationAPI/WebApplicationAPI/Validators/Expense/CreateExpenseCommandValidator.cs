using FluentValidation;
using WebApplicationAPI.Commands.Expense;

namespace WebApplicationAPI.Validators.Expense
{
    public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
    {
        public CreateExpenseCommandValidator()
        {
            RuleFor(x => x.Request.Amount).GreaterThan(0);
            RuleFor(x => x.Request.Month).InclusiveBetween(1, 12);
            RuleFor(x => x.Request.Year).GreaterThan(2000);
            RuleFor(x => x.Request.Description).NotEmpty();
            RuleFor(x => x.Request.Category).NotEmpty();
            RuleFor(x => x.Request.Status).NotEmpty();
            RuleFor(x => x.Request.PaymentMethod).NotEmpty();
        }
    }
}