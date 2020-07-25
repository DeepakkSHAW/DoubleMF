using System;
using System.Collections.Generic;
using System.Text;

namespace DoubleMF.Data.Services
{
    public class SayHello : ISayHello
    {
        public string Hello(string name)
        {
            return $"Hello {name}, implementation done correctly";
        }
    }
}
