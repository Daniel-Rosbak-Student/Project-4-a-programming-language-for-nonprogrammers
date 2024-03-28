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
        Visit(context.expr);
        return node;
    }

    /// <summary>
    /// Expression Visits
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Node VisitInfixExpression(SyntaxParser.InfixExpressionContext context)
    {
        return base.VisitInfixExpression(context);
    }

    public override Node VisitParensExpression(SyntaxParser.ParensExpressionContext context)
    {
        return base.VisitParensExpression(context);
    }

    public override Node VisitUseExpression(SyntaxParser.UseExpressionContext context)
    {
        return base.VisitUseExpression(context);
    }

    public override Node VisitConvertExpression(SyntaxParser.ConvertExpressionContext context)
    {
        return base.VisitConvertExpression(context);
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
        return new TextNode(context.@this.GetText());
    }

    public override Node VisitReadValue(SyntaxParser.ReadValueContext context)
    {
        return new TextNode(context.@this.GetText());
    }

    public override Node VisitFlagValue(SyntaxParser.FlagValueContext context)
    {
        return new TextNode(context.@this.GetText());
    }

    public override Node VisitLengthOfValue(SyntaxParser.LengthOfValueContext context)
    {
        return new TextNode(context.@this.GetText());
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
        return new TextNode(context.@this.GetText());
    }

    public override Node VisitIdentifierValue(SyntaxParser.IdentifierValueContext context)
    {
        return new TextNode(context.@this.GetText());
    }

/// <summary>
/// Command visits
/// </summary>
/// <param name="context"></param>
/// <returns></returns>
    public override Node VisitNotLastCommand(SyntaxParser.NotLastCommandContext context)
    {
        Visit(context.@this);
        return Visit(context.@next);
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
        return Visit(context.@this);
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

    public override Node VisitCommentTerm(SyntaxParser.CommentTermContext context)
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
        return Visit(context.@tp);
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