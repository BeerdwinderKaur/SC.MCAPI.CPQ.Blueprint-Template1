using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure.BigCat
{
    public interface IBigCatRepository
    {
        Task GetBigCatProduct();
    }
}
