using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMangement.MVC.Model
{
    public class OrderFramViewModel
    {
        public string Address { get; private set; } = string.Empty;
        public OrderFramViewModel(string address)
        {
            Address = address;
        }
    }
}
