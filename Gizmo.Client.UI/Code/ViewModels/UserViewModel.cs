using Gizmo.Shared.ViewModels;
using System;

namespace Gizmo.Client.UI.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime RegistrationDate { get; set; }
        public decimal Balance { get; set; }
        public string CurrentTimeProduct { get; set; }
        public TimeSpan Time { get; set; }
        public int Points { get; set; }
        public string Picture { get; set; }
    }
}