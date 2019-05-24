using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Dialogs
{
    public class FeedbackSelectionDialog : ComponentDialog
    {
        private const string DoneOption = "done";

        private const string ItemsSelected = "value-itemsSelected";

        private readonly string[] _reviewOptions = new string[]
        {
            " Speaker",
            " Event",
        };

        private string Choice { get; set; }
        public FeedbackSelectionDialog() : base(nameof(FeedbackSelectionDialog))
        {
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {
                    SelectionStepAsync,
                    ReviewStepAsync,
                    LoopStepAsync,
                }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> SelectionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var list = stepContext.Options as List<string> ?? new List<string>();
            stepContext.Values[ItemsSelected] = list;
            var options = _reviewOptions.ToList();
            options.Add(DoneOption);
            string message;
            if (list.Count is 0)
            {
                message = $"What would you like to review? You can choose `{DoneOption}` to finish.";
            }
            else
            {
                message = $"You have selected **{list[0]}**. You can review something else, " +
                    $"or choose `{DoneOption}` to finish.";
            }

            if (list.Count > 0)
            {
                options.Remove(list[0]);
            }

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(message),
                RetryPrompt = MessageFactory.Text("Please choose an option from the list."),
                Choices = ChoiceFactory.ToChoices(options),
            };

            // Prompt the user for a choice.
            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);

        }
        private async Task<DialogTurnResult> ReviewStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choice = (FoundChoice)stepContext.Result;
            Choice = choice.Value;
            var done = Choice == DoneOption;
            if (!done)
            {
                var promptOptions = new PromptOptions { Prompt = MessageFactory.Text($"How was the {choice.Value}") };

                return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
            }
            else
            {
                return await stepContext.EndDialogAsync(cancellationToken);
            }

        }
        private async Task<DialogTurnResult> LoopStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var list = stepContext.Options as List<string> ?? new List<string>();
            var done = Choice == DoneOption;

            if (done)
            {
                return await stepContext.EndDialogAsync(cancellationToken);
            }
            else
            {
                return await stepContext.ReplaceDialogAsync(nameof(FeedbackSelectionDialog), list, cancellationToken);
            }
        }
    }
}
