using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Components
{
    public partial class ApplicationDetailsDialog : CustomDOMComponentBase
    {
        public ApplicationDetailsDialog()
        {
            Random random = new Random();

            Application = new ApplicationViewModel()
            {
                Name = "Fall Guys: Ultimate Knockout",
                Description = "Fall Guys is a massively multiplayer party game with up to 60 players online in a free-for-all struggle through round after round of escalating chaos until one victor remains!",
                Image = "",
                Ratings = 168,
                Rate = 4,
                Publisher = "Very Positive Inc",
                ReleaseDate = new DateTime(2019, 10, 22),
                DateAdded = new DateTime(2020, 5, 13)
            };

            Application.Executables = new List<ExecutableViewModel>();
            Application.Executables.Add(new ExecutableViewModel() { Id = 1, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Loading = Convert.ToBoolean(random.Next(0, 2)), LoadingPercentage = random.Next(0, 100) });
            Application.Executables.Add(new ExecutableViewModel() { Id = 2, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Loading = Convert.ToBoolean(random.Next(0, 2)), LoadingPercentage = random.Next(0, 100) });
            Application.Executables.Add(new ExecutableViewModel() { Id = 3, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Loading = Convert.ToBoolean(random.Next(0, 2)), LoadingPercentage = random.Next(0, 100) });
            Application.Executables.Add(new ExecutableViewModel() { Id = 4, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Loading = Convert.ToBoolean(random.Next(0, 2)), LoadingPercentage = random.Next(0, 100) });
            Application.Executables.Add(new ExecutableViewModel() { Id = 5, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Loading = Convert.ToBoolean(random.Next(0, 2)), LoadingPercentage = random.Next(0, 100) });

            Application.Tags = new List<TagViewModel>();
            Application.Tags.Add(new TagViewModel() { Id = 1, Name = "Multiplayer" });
            Application.Tags.Add(new TagViewModel() { Id = 2, Name = "Free to Play" });
            Application.Tags.Add(new TagViewModel() { Id = 3, Name = "MMO" });
            Application.Tags.Add(new TagViewModel() { Id = 4, Name = "Custom Tag" });
            Application.Tags.Add(new TagViewModel() { Id = 5, Name = "Action" });
            Application.Tags.Add(new TagViewModel() { Id = 6, Name = "Adventure" });
        }

        #region FIELDS

        private bool _isOpen { get; set; }
        private int _selectedTabIndex = 0;

        #endregion

        #region PROPERTIES

        [Parameter]
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;

                _ = IsOpenChanged.InvokeAsync(_isOpen);
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        public ApplicationViewModel Application { get; set; }

        #endregion

        #region METHODS

        private void CloseDialog()
        {
            IsOpen = false;
        }

        private void SelectTab(int index)
        {
            if (index < 3)
                _selectedTabIndex = index;
        }

        private int GetTagsByChars(int numberOfCharacters)
        {
            int spaceBetweenTags = 3;
            int c = 0;
            int index = 0;
            foreach (var item in Application.Tags)
            {
                c += item.Name.Length + spaceBetweenTags;
                if (c > numberOfCharacters)
                {
                    return index;
                }
                index += 1;
            }
            return index;
        }

        #endregion
    }
}