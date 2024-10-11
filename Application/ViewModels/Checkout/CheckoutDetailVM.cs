

using Application.Enums;

namespace Application.ViewModels.Checkout
{
    public class CheckoutDetailVM
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string BookName { get; set; }
        public CheckoutStatus? Status { get; set; }

        public int BookCopyID { get; set; }
        public DateTime DueDate { get; set; }
    }
}
