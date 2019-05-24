using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Dialogs.UserProfile
{
    public class UserProfileDialog : ComponentDialog
    {
        private readonly UserState _userState;

        private const string UserInfo = "value-userInfo";
        public UserProfileDialog(UserState userstate) : base(nameof(UserProfileDialog))
        {
            _userState = userstate;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new FeedbackSelectionDialog());

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitializeStateStepAsync,
                PromptForNameStepAsync,
                PromptForEmailStepAsync,
                PromptForProfessionStepAsync,
                StartFeedbackStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> InitializeStateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Create an object in which to collect the user's information within the dialog.
            stepContext.Values[UserInfo] = new UserProfileState();

            await stepContext.Context.SendActivityAsync("Hi! Nice to see you. I hope you're enjoying the conference. We care to hear what you think. Let's start.");
            return await stepContext.NextAsync();
        }
        private async Task<DialogTurnResult> PromptForNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        private async Task<DialogTurnResult> PromptForEmailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = (UserProfileState)stepContext.Values[UserInfo];
            userProfile.Name = (string)stepContext.Result;

            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text("Please enter your email address if you'd like to hear from us.") };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);

        }
        private async Task<DialogTurnResult> PromptForProfessionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = (UserProfileState)stepContext.Values[UserInfo];
            userProfile.Email = (string)stepContext.Result;

            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text("What's your profession?") };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);

        }
        private async Task<DialogTurnResult> StartFeedbackStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = (UserProfileState)stepContext.Values[UserInfo];
            userProfile.Profession = (string)stepContext.Result;

            // Start the review selection dialog.
            return await stepContext.BeginDialogAsync(nameof(FeedbackSelectionDialog), null, cancellationToken);

        }
    }
}
