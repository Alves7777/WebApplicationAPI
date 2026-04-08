using FluentValidation;
using WebApplicationAPI.Commands.Expense;

namespace WebApplicationAPI.Validators.Expense
{
    public class DeleteExpenseCommandValidator : AbstractValidator<DeleteExpenseCommand>
    {
        public DeleteExpenseCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}