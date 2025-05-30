using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Flags]
    public enum Parameters
    {
        CashOnDelivery = 1 << 0,
        Prepaid = 1 << 1,
        GiftWrap = 1 << 2,
        Insurance = 1 << 3,
        ExpressShipping = 1 << 4,
        InternationalShipping = 1 << 5,
        SignatureRequired = 1 << 6,
    }
}
