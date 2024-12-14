using GoParser;

namespace GoInterpreter;

public class Interpreter
{
    private readonly Dictionary<string, object> _variables = [];

    public void Interpret(ASTNode node)
    {
        if (node is AssignmentNode assignment)
        {
            var value = Evaluate(assignment.Value);
            _variables[assignment.VariableName] = value;
        }
    }

    private object Evaluate(ASTNode node)
    {
        throw new NotImplementedException();
    }
}
