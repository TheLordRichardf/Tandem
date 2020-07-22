using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TandemAPI.ViewModels
{
    public class PersonViewModel
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }
    }
}
