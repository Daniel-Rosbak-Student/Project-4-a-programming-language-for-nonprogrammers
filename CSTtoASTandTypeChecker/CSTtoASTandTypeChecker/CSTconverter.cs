using System.Security.Cryptography;

namespace CSTtoASTandTypeChecker;

internal class CSTconverter : SyntaxBaseVisitor<Node>
{
    /// <summary>
    /// Program Visit (visit all children)
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Node VisitProgram(SyntaxParser.ProgramContext context)
    {
        return Visit(context.commands());
    }

    /// <summary>
    /// Assignment Visits
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Node VisitAssignStatement(SyntaxParser.AssignStatementContext context)
    {
        return VisitAssignment(context.@this);
    }

    public override Node VisitAssignment(SyntaxParser.AssignmentContext context)
    {
        IdentifierNode node = new IdentifierNode();
        node.name = context.id.GetText();
        node.value = Visit(context.expr);
        return node;
    }

    /// <summary>
    /// Expression Visits
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Node VisitInfixExpression(SyntaxParser.InfixExpressionContext context)
    {
        InFixNode node = null;
        switch (context.op.GetText().ToLower())
        {
            case "+":
                node = new AdditionNode();
                break;
            case "-":
                node = new SubtractNode();
                break;
            case "*":
                node = new MultiplyNode();
                break;
            case "/":
                node = new DivideNode();
                break;
            case "modulo":
                node = new ModuloNode();
                break;
            case "=":
                node = new EqualsNode();
                break;
            case "<":
                node = new LessNode();
                break;
            case "<=":
                node = new LessEqualsNode();
                break;
            case ">":
                node = new GreaterNode();
                break;
            case ">=":
                node = new GreaterEqualsNode();
                break;
            case "and":
                node = new AndNode();
                break;
            case "or":
                node = new OrNode();
                break;
            case "not":
                node = new NotNode();
                break;
        }

        if (node != null)
        {
            node.left = Visit(context.left);
            node.right = Visit(context.right);
        }

        return node;
    }

    public override Node VisitParensExpression(SyntaxParser.ParensExpressionContext context)
    {
        return Visit(context.expr);
    }

    public override Node VisitUseExpression(SyntaxParser.UseExpressionContext context)
    {
        
        return Visit(context.@this);
    }

    public override Node VisitConvertExpression(SyntaxParser.ConvertExpressionContext context)
    {
        TypeConvertNode node = new TypeConvertNode();
        node.value = Visit(context.expr);
        node.type = (TypeNode)Visit(context.tp);
        return node;
    }

    public override Node VisitValueExpression(SyntaxParser.ValueExpressionContext context)
    {
        return Visit(context.@this);
    }

    /// <summary>
    /// All Value Visits
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Node VisitUseValue(SyntaxParser.UseValueContext context)
    {
        return Visit(context.@this);
    }

    public override Node VisitReadValue(SyntaxParser.ReadValueContext context)
    {
        return Visit(context.@this);
    }

    public override Node VisitFlagValue(SyntaxParser.FlagValueContext context)
    {
        return new FlagNode(context.@this.GetText().ToLower() == "true");
    }

    public override Node VisitLengthOfValue(SyntaxParser.LengthOfValueContext context)
    {
        return Visit(context.@this);
    }

    public override Node VisitNumberValue(SyntaxParser.NumberValueContext context)
    {
        return new NumberNode(double.Parse(context.@this.GetText()));
    }

    public override Node VisitTextValue(SyntaxParser.TextValueContext context)
    {
        return new TextNode(context.@this.GetText());
    }

    public override Node VisitListElementValue(SyntaxParser.ListElementValueContext context)
    {
        return Visit(context.@this);
    }

    public override Node VisitIdentifierValue(SyntaxParser.IdentifierValueContext context)
    {
        IdentifierNode node = new IdentifierNode();
        node.name = context.@this.GetText();
        return node;
    }

/// <summary>
/// Command visits
/// </summary>
/// <param name="context"></param>
/// <returns></returns>
    public override Node VisitNotLastCommand(SyntaxParser.NotLastCommandContext context)
    {
        return new CommandNode(Visit(context.@this), Visit(context.next));
    }

    public override Node VisitLastCommand(SyntaxParser.LastCommandContext context)
    {
        return Visit(context.@this);
    }

    public override Node VisitFunctionCommand(SyntaxParser.FunctionCommandContext context)
    {
        return Visit(context.@this);
    }

    public override Node VisitTermsCommand(SyntaxParser.TermsCommandContext context)
    {
        return Visit(context.@this);
    }

    /// <summary>
    /// Terms visits
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Node VisitNotLastTerm(SyntaxParser.NotLastTermContext context)
    {
        if (context.@this.RuleIndex == 3)
        {
            return Visit(context.next);
        }
        return new CommandNode(Visit(context.@this), Visit(context.next));
    }

    public override Node VisitLastTerm(SyntaxParser.LastTermContext context)
    {
        return Visit(context.@this);
    }

    public override Node VisitStatementTerm(SyntaxParser.StatementTermContext context)
    {
        return Visit(context.@this);
    }

    public override Node VisitControlTerm(SyntaxParser.ControlTermContext context)
    {
        return Visit(context.@this);
    }

    /// <summary>
    /// Length of visit
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Node VisitLengthOf(SyntaxParser.LengthOfContext context)
    {
        return Visit(context.@id);
    }

    /// <summary>
    /// Type visit
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Node VisitType(SyntaxParser.TypeContext context)
    {
        switch (context.tp.RuleIndex)
        {
            //number
            case 1:
                return new NumberTypeNode();
            //text
            case 2:
                return new TextTypeNode();
            //flag
            case 3:
                return new FlagTypeNode();
            //list
            case 4:
                ListTypeNode node = new ListTypeNode();
                node.type = (TypeNode)Visit(context.tp);
                return node;
        }
        return null;
    }

    /// <summary>
    /// Identifier visit
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Node VisitIdentifier(SyntaxParser.IdentifierContext context)
    {
        return Visit(context.@id);
    }

    /// <summary>
    /// ListElement visit
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Node VisitListElement(SyntaxParser.ListElementContext context)
    {
        return Visit(context.@id);
    }
}