using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.Order.GetAllOrders
{
    public class GetAllOrderQueryResponse
    {
        public int TotalCount { get; set; }
        public object Orders { get; set; }
        //public string OrderCode { get; set; }
        //public string UserName { get; set; }
        //public float TotalPrice { get; set; }
        //public DateTime CreateDate { get; set; }
    }
}
