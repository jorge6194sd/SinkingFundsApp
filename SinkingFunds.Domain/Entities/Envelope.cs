using System;
using System.Collections.Generic;

namespace SinkingFunds.Domain.Entities
{
    public class Envelope
    {
        string name { get; set; }
        Boolean isActive { get; set; }

        decimal currentAmount { get; set; }


    }
}
