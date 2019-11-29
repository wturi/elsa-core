using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.Activities.ControlFlow.Activities
{
    [ActivityDefinition(
        Category = "Control Flow",
        Description = "Switch execution based on a given expression.",
        Icon = "far fa-list-alt",
        RuntimeDescription = "x => !!x.state.expression ? `Switch execution based on <strong>${ x.state.expression.expression }</strong>.` : x.definition.description",
        Outcomes = "x => x.state.cases.map(c => c.toString())"
    )]
    public class Switch : Activity
    {
        public Switch(IWorkflowExpressionEvaluator expressionEvaluator)
        {
            Cases = new List<string>()
            {
                OutcomeNames.Default
            };
        }

        [ActivityProperty(Hint = "The expression to evaluate. The evaluated value will be used to switch on.")]
        public WorkflowExpression<string> Expression
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "A comma-separated list of possible outcomes of the expression.")]
        public IReadOnlyCollection<string> Cases
        {
            get => GetState<IReadOnlyCollection<string>>();
            set => SetState(value);
        }

        protected override async Task<IActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext workflowContext, CancellationToken cancellationToken)
        {
            var result = await workflowContext.EvaluateAsync(Expression, cancellationToken);

            if (ContainsCase(result) || !ContainsCase(OutcomeNames.Default))
                return Outcome(result, result);

            return Outcome(OutcomeNames.Default, result);
        }

        private bool ContainsCase(string @case) => Cases.Contains(@case, StringComparer.OrdinalIgnoreCase);
    }
}