using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Models
{
    public class EmailAttachment
    {
        public string FileName { get; set; }
        public string MediaType { get; set; } = "application";
        public string MediaSubType { get; set; } = "pdf";
        public Stream Content { get; set; }
    }
}
