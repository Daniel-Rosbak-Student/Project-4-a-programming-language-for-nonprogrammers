namespace CSTtoASTandTypeChecker;

internal class CSTconverter : SyntaxBaseVisitor<Node>
{
    public override Node VisitProgram(SyntaxParser.ProgramContext context)
    {
        return Visit(context.commands());
    }

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

    public override Node VisitNumberValue(SyntaxParser.NumberValueContext context)
    {
        return new NumberNode(double.Parse(context.@this.GetText()));
    }

    public override Node VisitTextValue(SyntaxParser.TextValueContext context)
    {
        return new TextNode(context.@this.GetText());
    }
}