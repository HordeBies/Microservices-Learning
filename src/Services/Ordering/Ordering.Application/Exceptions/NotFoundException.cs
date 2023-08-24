using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Exceptions
{
	public class NotFoundException : Exception
    {
		public NotFoundException(string name, object key) : base($"Entity '{name}' ({key}) was not found") { }
		public NotFoundException(string name, object key, Exception inner) : base($"Entity '{name}' ({key}) was not found", inner) { }
	}
}
