using AutoMapper;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Models;

namespace WebApplicationAPI.Mapping
{
    public class ExpenseProfile : Profile
    {
        public ExpenseProfile()
        {
            CreateMap<CreateExpenseRequest, Expense>();
            CreateMap<UpdateExpenseRequest, Expense>();
            CreateMap<Expense, ExpenseResponse>();
        }
    }
}