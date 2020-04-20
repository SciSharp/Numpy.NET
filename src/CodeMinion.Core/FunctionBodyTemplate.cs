using System.Text;
using CodeMinion.Core.Helpers;
using CodeMinion.Core.Models;

namespace CodeMinion.Core
{

    public abstract class FunctionBodyTemplate
    {
        public abstract void GenerateBody(Function decl, CodeWriter s);
    }
}
