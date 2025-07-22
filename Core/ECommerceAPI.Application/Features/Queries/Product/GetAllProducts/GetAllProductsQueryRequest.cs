using ECommerceAPI.Application.RequestParameters;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Features.Queries.Product.GetAllProducts
{
    public class GetAllProductsQueryRequest : IRequest<GetAllProductsQueryResponse> // request nesnesini ve geriye hangi response nesnesini döndüreceğini belirtiyoruz
    {
        //public Pagination Pagination { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}
