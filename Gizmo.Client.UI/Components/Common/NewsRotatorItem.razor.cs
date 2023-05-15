using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class NewsRotatorItem : CustomDOMComponentBase
    {
        //private string _text = "<base href='https://www.gameworld.gr' /><p>Αν ο χαρακτήρας σας πεθάνει φτιάχνετε νέο!</p><br/><p>Η Blizzard ετοιμάζει Hardcore mode servers για το World of Warcraft Classic.</p> <p>Όσοι φτιάξετε χαρακτήρες σε αυτούς τους servers, να ξέρετε ότι αν ο χαρακτήρας σας πεθάνει τότε θα πρέπει να φτιάξετε καινούργιο καθώς δεν υπάρχει revive!</p> <blockquote class=\"twitter-tweet\"> <p lang=\"en\" dir=\"ltr\">The Spirit Healers are Ready. ⚰️ <br /><br />Official Classic Hardcore is coming.<br /><br />Stay tuned for more info. <a href=\"https://t.co/tAsmV9NKDI\">pic.twitter.com/tAsmV9NKDI</a></p> — World of Warcraft (@Warcraft) <a href=\"https://twitter.com/Warcraft/status/1657475304963682306?ref_src=twsrc%5Etfw\">May 13, 2023</a></blockquote> <script async=\"async\" src=\"https://platform.twitter.com/widgets.js\" charset=\"utf-8\"></script> <p>Οι hardcore servers θα είναι έτοιμοι το καλοκαίρι. Αν ο χαρακτήρας σας πεθάνει τότε θα μπορείτε απλά να συνδεθείτε ως ghost για να μεταφέρετε το guild leadership σε άλλο παίκτη ή να στείλετε μηνύματα σε φίλους. Επίσης οι hardcore servers θα είναι εντελώς νέοι άρα όλοι θα ξεκινούν από το αρχικό level. Δεν θα μετατραπούν ήδη υπάρχοντες servers.</p> <p>Επίσης θα υπάρχει και μια νέα έκδοση του Duel (θα ονομάζεται /makgora) η οποία θα σας επιτρέπει να πολεμήσετε μέχρι θανάτου. </p> <p>Διαβάστε περισσότερα για το <a href=\"news/mmorpg/51746-world-of-warcraft-ai-addon-voice-acting-npcs\" target=\"_blank\" rel=\"noopener\">World of Warcraft Classic AddOn</a> που προσθέτει voice acting σε όλους τους NPCs μέσω AI και συζητήστε για αυτό στο <a href=\"forum/30/163212\" target=\"_blank\" rel=\"noopener\">σχετικό forum topic</a>.</p><br/><p></p><br/>";
        //private string _text = "James Gunn has responded to reports that claim Superman: Legacy cast contenders include Nicholas Hoult, David Corenswet, Rachel Brosnahan, and more, saying he would never comment on who is or isn't auditioning and that only one person has been cast in the film so far.";

        private bool _isCurrent;
        private bool _isNext;

        private string _previousText = string.Empty;
        private string _text = string.Empty;

        [CascadingParameter]
        protected NewsRotator Parent { get; set; }

        [Parameter]
        public string Text { get; set; }

        internal void Clear()
        {
            _isCurrent = false;
            _isNext = false;

            InvokeAsync(StateHasChanged);
        }

        internal void SetCurrent()
        {
            _isCurrent = true;

            InvokeAsync(StateHasChanged);
        }

        internal void SetNext()
        {
            _isNext = true;

            InvokeAsync(StateHasChanged);
        }

        #region OVERRIDE

        protected override void OnInitialized()
        {
            if (Parent != null)
            {
                Parent.Register(this);
            }
        }

        public override void Dispose()
        {
            try
            {
                if (Parent != null)
                {
                    Parent.Unregister(this);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        protected override async Task OnParametersSetAsync()
        {
            var textChanged = _previousText != Text;
            if (textChanged)
            {
                _text = _previousText;
                _isCurrent = true;
                _isNext = false;

                _previousText = Text;

                await InvokeAsync(StateHasChanged);

                await Task.Delay(1000);

                _text = Text;
                _isCurrent = false;
                _isNext = true;
                await InvokeAsync(StateHasChanged);
            }

            await base.OnParametersSetAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }
            else
            {
                await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-news-rotator-item")
                .If("giz-news-rotator-item--current fade-out", () => _isCurrent)
                .If("giz-news-rotator-item--next fade-in", () => _isNext)
                .AsString();

        #endregion
    }
}
