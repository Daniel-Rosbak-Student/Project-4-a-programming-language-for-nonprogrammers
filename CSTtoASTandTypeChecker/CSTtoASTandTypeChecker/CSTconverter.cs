using System.ComponentModel.DataAnnotations;
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
        return Visit(context.@this);
    }

    public override Node VisitAssignment(SyntaxParser.AssignmentContext context)
    {
        return new AssignNode(Visit(context.id), Visit(context.expr));
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
            default:
                throw new ValidationException("Bad Operator in InfixExpression");
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

    public override Node VisitUseNoInput(SyntaxParser.UseNoInputContext context)
    {
        return new UseNode(Visit(context.id), null);
    }

    public override Node VisitUseWithInput(SyntaxParser.UseWithInputContext context)
    {
        return new UseNode(Visit(context.id), (InputNode)Visit(context.input));
    }

    public override Node VisitNotLastInput(SyntaxParser.NotLastInputContext context)
    {
        return new InputNode(Visit(context.expr), Visit(context.next));
    }

    public override Node VisitLastInput(SyntaxParser.LastInputContext context)
    {
        return Visit(context.expr);
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
        Node identifierNode = Visit(context.lengthOf());
        LengthOfNode node = new LengthOfNode(identifierNode);
        return node;
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

        Node next = Visit(context.next);
        if (next == null)
        {
            return Visit(context.@this);
        }
        return new CommandNode(Visit(context.@this), next);
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
        Node identifierNode = Visit(context.id);
        LengthOfNode node = new LengthOfNode(identifierNode);
        return node;
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
            default:
                throw new Exception("Bad type in VisitType");
        }
    }

    /// <summary>
    /// Identifier visit
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Node VisitIdentifier(SyntaxParser.IdentifierContext context)
    {
        IdentifierNode node = new IdentifierNode();
        node.name = context.id.GetText();
        return node;
    }

    /// <summary>
    /// ListElement visit
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Node VisitListElement(SyntaxParser.ListElementContext context)
    {
        return new ListElementNode(Visit(context.id), Visit(context.index));
    }

    public override Node VisitRead(SyntaxParser.ReadContext context)
    {
        return new ReadNode();
    }

    public override Node VisitPrint(SyntaxParser.PrintContext context)
    {
        return new PrintNode(Visit(context.expr));
    }

    public override Node VisitBreak(SyntaxParser.BreakContext context)
    {
        return new BreakNode();
    }

    public override Node VisitBreakStatement(SyntaxParser.BreakStatementContext context)
    {
        return Visit(context.@this);
    }

    public override Node VisitNotLastParameter(SyntaxParser.NotLastParameterContext context)
    {
        return base.VisitNotLastParameter(context);
    }
    
    public override Node VisitLastParameter(SyntaxParser.LastParameterContext context)
    {
        return base.VisitLastParameter(context);
    }

    public override Node VisitLoop(SyntaxParser.LoopContext context)
    {
        return base.VisitLoop(context);
    }

    public override Node VisitCreateStatement(SyntaxParser.CreateStatementContext context)
    {
        return base.VisitCreateStatement(context);
    }

    public override Node VisitIfWithElse(SyntaxParser.IfWithElseContext context)
    {
        return base.VisitIfWithElse(context);
    }

    public override Node VisitIfNoElse(SyntaxParser.IfNoElseContext context)
    {
        return base.VisitIfNoElse(context);
    }

    public override Node VisitFunctionWithTakes(SyntaxParser.FunctionWithTakesContext context)
    {
        return base.VisitFunctionWithTakes(context);
    }

    public override Node VisitFunctionNoTakes(SyntaxParser.FunctionNoTakesContext context)
    {
        return base.VisitFunctionNoTakes(context);
    }

    
    //------------------------------------------------------------------------------------------------------------------
    public override Node VisitCreateWithInput(SyntaxParser.CreateWithInputContext context)
    {
        return new CreateVariableNode(Visit(context.id), Visit(context.tp), Visit(context.expr));
    }

    public override Node VisitCreateNoInput(SyntaxParser.CreateNoInputContext context)
    {
        return new CreateVariableNode(Visit(context.id), Visit(context.tp));
    }

    public override Node VisitTakesArgument(SyntaxParser.TakesArgumentContext context)
    {
        return Visit(context.param);
    }

    public override Node VisitGivesType(SyntaxParser.GivesTypeContext context)
    {
        return Visit(context.tp);
    }

    public override Node VisitGivesNothing(SyntaxParser.GivesNothingContext context)
    {
        return new NothingNode();
    }

    public override Node VisitUseGive(SyntaxParser.UseGiveContext context)
    {
        return new GiveNode(Visit(context.@this));
    }

    public override Node VisitNothingGive(SyntaxParser.NothingGiveContext context)
    {
        return new GiveNode(null);
    }

    public override Node VisitExpressionGive(SyntaxParser.ExpressionGiveContext context)
    {
        return new GiveNode(Visit(context.expr));
    }
    
}