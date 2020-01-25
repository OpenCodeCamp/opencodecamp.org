using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenCodeCamp.OutgoingCommunications.EmailDocumentsLibrary.Views.Shared
{
    public sealed class EmailButtonViewModel
    {
        public EmailButtonViewModel(string text, string url)
        {
            Text = text;
            Url = url;
        }

        public string Text { get; set; }
        public string Url { get; set; }
    }
}